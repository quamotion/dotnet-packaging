using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Packaging.Targets.IO;

namespace Packaging.Targets.Deb
{
    internal class DebPackageCreator
    {
        private const LinuxFileMode ArFileMode = LinuxFileMode.S_IRUSR | LinuxFileMode.S_IWUSR | LinuxFileMode.S_IRGRP |
                                                 LinuxFileMode.S_IROTH | LinuxFileMode.S_IFREG;

        public static void BuildDebPackage(
            List<ArchiveEntry> archiveEntries,
            Stream tarXzStream,
            string name,
            string description,
            string maintainer,
            string version,
            string arch,
            bool createUser,
            string userName,
            bool installService,
            string serviceName,
            string prefix,
            IEnumerable<string> additionalDependencies,
            Action<DebPackage> additionalMetadata,
            Stream targetStream)
        {
            var pkg = new DebPackage
            {
                Md5Sums = archiveEntries.Where(e => e.Md5Hash != null)
                    .ToDictionary(
                        e => e.TargetPath.TrimStart('.', '/'),
                        e => BitConverter.ToString(e.Md5Hash).ToLower().Replace("-", string.Empty)),
                PackageFormatVersion = new Version(2, 0),
                ControlFile = new Dictionary<string, string>
                {
                    ["Package"] = name,
                    ["Version"] = version,
                    ["Architecture"] = arch,
                    ["Maintainer"] = maintainer,
                    ["Description"] = description
                }
            };

            if (createUser)
            {
                // Add the user and group, under which the service runs.
                // These users are never removed because UIDs are re-used on Linux.
                pkg.PreInstallScript += $"/usr/sbin/groupadd -r {userName} 2>/dev/null || :\n" +
                                        $"/usr/sbin/useradd -g {userName} -s /sbin/nologin -r -d {prefix} {userName} 2>/dev/null || :\n";
            }

            if (installService)
            {
                // Install and activate the service.
                pkg.PostInstallScript += $"systemctl reload";
                pkg.PostInstallScript += $"systemctl enable --now {serviceName}.service";
                pkg.PreRemoveScript += $"systemctl --no-reload disable --now {serviceName}.service";
            }

            // Remove all directories marked as such (these are usually directories which contain temporary files)
            foreach (var entryToRemove in archiveEntries.Where(e => e.RemoveOnUninstall))
            {
                pkg.PostRemoveScript += $"/usr/bin/rm -rf {entryToRemove.TargetPath}\n";
            }

            string deps = string.Empty;

            if (additionalDependencies != null)
            {
                foreach (var dependency in additionalDependencies)
                {
                    deps += ", " + dependency;
                }
            }

            pkg.ControlFile["Depends"] = deps;

            additionalMetadata?.Invoke(pkg);

            ArFileCreator.WriteMagic(targetStream);
            ArFileCreator.WriteEntry(targetStream, "debian-binary", ArFileMode, pkg.PackageFormatVersion + "\n");
            WriteControl(targetStream, pkg, archiveEntries);
            ArFileCreator.WriteEntry(targetStream, "data.tar.xz", ArFileMode, tarXzStream);
        }

        private static void WriteControl(Stream targetStream, DebPackage pkg, List<ArchiveEntry> entries)
        {
            var controlTar = new MemoryStream();
            WriteControlEntry(controlTar, "./");
            WriteControlEntry(
                controlTar,
                "./control",
                string.Join("\n", pkg.ControlFile
                    .OrderByDescending(x => x.Key == "Package").ThenBy(x => x.Key)
                    .Select(x => $"{x.Key}: {x.Value}")) + "\n");

            WriteControlEntry(
                controlTar,
                "./md5sums",
                string.Join("\n", pkg.Md5Sums.Select(x => $"{x.Value}  {x.Key}")) + "\n");

            var execMode = LinuxFileMode.S_IRUSR | LinuxFileMode.S_IWUSR | LinuxFileMode.S_IXUSR |
                           LinuxFileMode.S_IRGRP | LinuxFileMode.S_IROTH;

            if (string.IsNullOrWhiteSpace(pkg.PreInstallScript))
            {
                WriteControlEntry(controlTar, "./preinst", $"#!/bin/sh\n{pkg.PreInstallScript}\n", execMode);
            }

            if (string.IsNullOrWhiteSpace(pkg.PostInstallScript))
            {
                WriteControlEntry(controlTar, "./postinst", $"#!/bin/sh\n{pkg.PostInstallScript}\n", execMode);
            }

            if (string.IsNullOrWhiteSpace(pkg.PreRemoveScript))
            {
                WriteControlEntry(controlTar, "./prerm", $"#!/bin/sh\n{pkg.PreRemoveScript}\n", execMode);
            }

            if (string.IsNullOrWhiteSpace(pkg.PostRemoveScript))
            {
                WriteControlEntry(controlTar, "./postrm", $"#!/bin/sh\n{pkg.PostRemoveScript}\n", execMode);
            }

            var confFiles = entries
                .Where(e => e.Mode.HasFlag(LinuxFileMode.S_IFREG) && e.TargetPath.StartsWith("/etc/"))
                .Select(e => e.TargetPath).ToList();
            if (confFiles.Any())
            {
                WriteControlEntry(controlTar, "./conffiles", string.Join("\n", confFiles) + "\n");
            }

            TarFileCreator.WriteTrailer(controlTar);
            controlTar.Seek(0, SeekOrigin.Begin);

            var controlTarGz = new MemoryStream();
            using (var gzStream = new GZipStream(controlTarGz, CompressionMode.Compress, true))
            {
                controlTar.CopyTo(gzStream);
            }

            controlTarGz.Seek(0, SeekOrigin.Begin);
            ArFileCreator.WriteEntry(targetStream, "control.tar.gz", ArFileMode, controlTarGz);
        }

        private static void WriteControlEntry(Stream tar, string name, string data = null, LinuxFileMode? fileMode = null)
        {
            var s = (data != null) ? new MemoryStream(Encoding.UTF8.GetBytes(data)) : new MemoryStream();
            var mode = fileMode ?? LinuxFileMode.S_IRUSR | LinuxFileMode.S_IWUSR |
                       LinuxFileMode.S_IRGRP | LinuxFileMode.S_IROTH;
            mode |= data == null
                ? LinuxFileMode.S_IFDIR | LinuxFileMode.S_IXUSR | LinuxFileMode.S_IXGRP | LinuxFileMode.S_IXOTH
                : LinuxFileMode.S_IFREG;
            var hdr = new TarHeader
            {
                FileMode = mode,
                FileName = name,
                FileSize = (uint)s.Length,
                GroupName = "root",
                UserName = "root",
                LastModified = DateTimeOffset.UtcNow,
                Magic = "ustar",
                TypeFlag = data == null ? TarTypeFlag.DirType : TarTypeFlag.RegType,
            };
            TarFileCreator.WriteEntry(tar, hdr, s);
        }
    }
}