using Packaging.Targets.Rpm;
using System;
using System.IO;
using System.Security.Cryptography;

namespace Packaging.Targets.RpmRepo
{
    /// <summary>
    /// Harvests metadata from a physical RPM package and generates the RPM metadta files.
    /// </summary>
    public class MetadataHarvester
    {
        public PrimaryMetadata Harvest(string directory)
        {
            var value = new PrimaryMetadata();

            foreach (var file in Directory.GetFiles(directory, "*.rpm"))
            {
                var metadata = this.HarvestPackage(file);
                value.Packages.Add(metadata);
            }

            return value;
        }

        protected PackageMetadata HarvestPackage(string path)
        {
            using (Stream stream = File.OpenRead(path))
            {
                RpmPackage package = RpmPackageReader.Read(stream);
                var metadata = new RpmMetadata(package);

                // The architecture embedded in the source packages seems wrong, we get 'x86_64'
                // for libplist-1.2-1.el6.src.rpm instead of 'src', so get it from the file name instead
                var fileParts = path.Split('.');
                var arch = fileParts[fileParts.Length - 2];

                var packageVersion = RpmVersion.Parse(metadata.Version);
                var fileinfo = new FileInfo(path);

                PackageMetadata packageMetadata = new PackageMetadata()
                {
                    Arch = arch,
                    ArchiveSize = (int)fileinfo.Length,
                    BuildHost = metadata.BuildHost,
                    BuildTime = metadata.BuildTime.ToUnixTimeSeconds(),
                    Description = metadata.Description,
                    Group = metadata.Group,
                    HeaderStart = package.HeaderOffset,
                    HeaderEnd = package.PayloadOffset,
                    InstalledSize = metadata.Size,
                    License = metadata.License,
                    Name = metadata.Name,
                    Packager = metadata.Packager,
                    Release = metadata.Release,
                    SourceRpm = metadata.SourceRpm ?? string.Empty,
                    Summary = metadata.Summary,
                    Url = metadata.Url,
                    Vendor = metadata.Vendor,
                    Version = packageVersion.Version,
                    VersionEpoch = packageVersion.Epoch ?? 0,
                    FileTime = new DateTimeOffset(fileinfo.LastWriteTimeUtc).ToUnixTimeSeconds(),
                    PackageSize = fileinfo.Length,
                    Location = $"RPMS/{fileinfo.Name}"
                };

                foreach (var changelogEntry in metadata.ChangelogEntries)
                {
                    packageMetadata.ChangeLog.Add(
                        new ChangeLogEntry()
                        {
                            Author = changelogEntry.Name,
                            Date = changelogEntry.Date.ToUnixTimeSeconds(),
                            Text = changelogEntry.Text
                        });
                }

                foreach (var file in metadata.Files)
                {
                    packageMetadata.Files.Add(file.Name);
                }

                foreach (var provide in metadata.Provides)
                {
                    var version = RpmVersion.Parse(provide.Version);

                    packageMetadata.Provides.Add(
                        new RpmEntry()
                        {
                            Name = provide.Name,
                            Flags = this.GetFlag(provide.Flags),
                            Epoch = version.Epoch ?? 0,
                            Release = version.Release,
                            Version = version.Version
                        });
                }

                packageMetadata.Provides.Sort((x, y) => string.Compare(x.Name, y.Name));

                foreach (var require in metadata.Requires)
                {
                    if (require.Flags.HasFlag(RpmSense.RPMSENSE_RPMLIB))
                    {
                        // These are requirements for RPM and are not included
                        continue;
                    }

                    var version = RpmVersion.Parse(require.Version);

                    var entry =
                        new RpmEntry()
                        {
                            Name = require.Name,
                            Flags = this.GetFlag(require.Flags),
                            Epoch = version.Epoch ?? 0,
                            Release = version.Release,
                            Version = version.Version
                        };

                    if (require.Flags.HasFlag(RpmSense.RPMSENSE_SCRIPT_PRE))
                    {
                        entry.Pre = 1;
                    }

                    packageMetadata.Requires.Add(entry);
                }

                packageMetadata.Requires.Sort((x, y) => string.Compare(x.Name, y.Name));

                packageMetadata.Type = "rpm";

                packageMetadata.ChecksumType = "sha256";
                packageMetadata.ChecksumIsPkgId = true;

                using (var sha = SHA256.Create())
                {
                    stream.Position = 0;
                    var hash = sha.ComputeHash(stream);
                    packageMetadata.Checksum = BitConverter.ToString(hash).Replace("-", string.Empty).ToLower();
                }

                return packageMetadata;
            }
        }

        private string GetFlag(RpmSense sense)
        {
            switch (sense)
            {
                case RpmSense.RPMSENSE_EQUAL:
                    return "EQ";

                default:
                    return null;
            }
        }
    }
}
