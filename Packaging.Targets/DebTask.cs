using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Packaging.Targets.Deb;
using Packaging.Targets.IO;

namespace Packaging.Targets
{
    public class DebTask : Task
    {
        [Required]
        public string PublishDir { get; set; }

        [Required]
        public string DebPath { get; set; }

        [Required]
        public string DebTarPath { get; set; }

        [Required]
        public string DebTarXzPath { get; set; }

        [Required]
        public string Prefix { get; set; }

        [Required]
        public string Version { get; set; }

        [Required]
        public string PackageName { get; set; }

        [Required]
        public ITaskItem[] Content { get; set; }

        [Required]
        public string Maintainer { get; set; }

        [Required]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the runtime identifier for which we are currently building.
        /// Used to determine the target architecture of the package.
        /// </summary>
        [Required]
        public string RuntimeIdentifier { get; set; }

        /// <summary>
        /// Gets or sets the package architecture (amd64, i386,...). When not set,
        /// the target architecture will be derived based on the <see cref="RuntimeIdentifier"/>
        /// </summary>
        public string DebPackageArchitecture { get; set; }

        public string Section { get; set; }

        public string Homepage { get; set; }

        public string Priority { get; set; }

        /// <summary>
        /// Gets or sets a list of empty folders to create when
        /// installing this package.
        /// </summary>
        public ITaskItem[] LinuxFolders { get; set; }

        /// <summary>
        /// Gets or sets a list of Debian packages on which the version of .NET
        /// Core embedded in this package depends.
        /// </summary>
        public ITaskItem[] DebDotNetDependencies { get; set; }

        /// <summary>
        /// Gets or sets a list of Debian packages on which this Debian
        /// package depends.
        /// </summary>
        public ITaskItem[] DebDependencies { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to create a Linux
        /// user and group when installing the package.
        /// </summary>
        public bool CreateUser { get; set; }

        /// <summary>
        /// Gets or sets the name of the Linux user and group to create.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to install
        /// and launch as systemd service when installing the package.
        /// </summary>
        public bool InstallService { get; set; }

        /// <summary>
        /// Gets or sets the name of the SystemD service to create.
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// Gets or sets an additional pre-installation script to execute.
        /// </summary>
        /// <remarks>
        /// This variable must contain the script itself, and not a path to a file
        /// which contains the script.
        /// </remarks>
        public string PreInstallScript { get; set; }

        /// <summary>
        /// Gets or sets an additional post-installation script to execute.
        /// </summary>
        /// <remarks>
        /// This variable must contain the script itself, and not a path to a file
        /// which contains the script.
        /// </remarks>
        public string PostInstallScript { get; set; }

        /// <summary>
        /// Gets or sets an additional pre-removal script to execute.
        /// </summary>
        /// <remarks>
        /// This variable must contain the script itself, and not a path to a file
        /// which contains the script.
        /// </remarks>
        public string PreRemoveScript { get; set; }

        /// <summary>
        /// Gets or sets an additional post-removal script to execute.
        /// </summary>
        /// <remarks>
        /// This variable must contain the script itself, and not a path to a file
        /// which contains the script.
        /// </remarks>
        public string PostRemoveScript { get; set; }

        /// <summary>
        /// Derives the package architecture from a .NET runtime identiifer.
        /// </summary>
        /// <param name="runtimeIdentifier">
        /// The runtime identifier.
        /// </param>
        /// <returns>
        /// The equivalent package architecture.
        /// </returns>
        public static string GetPackageArchitecture(string runtimeIdentifier)
        {
            // Valid architectures can be obtained by running "dpkg-architecture --list-known"
            RuntimeIdentifiers.ParseRuntimeId(runtimeIdentifier, out _, out _, out Architecture? architecture, out _);

            if (architecture != null)
            {
                switch (architecture.Value)
                {
                    case Architecture.Arm:
                        return "armhf";

                    case Architecture.Arm64:
                        return "arm64";

                    case Architecture.X64:
                        return "amd64";

                    case Architecture.X86:
                        return "i386";
                }
            }

            return "any";
        }

        public override bool Execute()
        {
            this.Log.LogMessage(
                MessageImportance.High,
                "Creating DEB package '{0}' from folder '{1}'",
                this.DebPath,
                this.PublishDir);

            using (var targetStream = File.Open(this.DebPath, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
            using (var tarStream = File.Open(this.DebTarPath, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
            {
                ArchiveBuilder archiveBuilder = new ArchiveBuilder();
                var archiveEntries = archiveBuilder.FromDirectory(
                    this.PublishDir,
                    this.Prefix,
                    this.Content);

                archiveEntries.AddRange(archiveBuilder.FromLinuxFolders(this.LinuxFolders));
                this.EnsureDirectories(archiveEntries);

                archiveEntries = archiveEntries
                    .OrderBy(e => e.TargetPathWithFinalSlash, StringComparer.Ordinal)
                    .ToList();

                TarFileCreator.FromArchiveEntries(archiveEntries, tarStream);
                tarStream.Position = 0;

                // Prepare the list of dependencies
                List<string> dependencies = new List<string>();

                if (this.DebDependencies != null)
                {
                    var debDependencies = this.DebDependencies.Select(d => d.ItemSpec).ToArray();

                    dependencies.AddRange(debDependencies);
                }

                if (this.DebDotNetDependencies != null)
                {
                    var debDotNetDependencies = this.DebDotNetDependencies.Select(d => d.ItemSpec).ToArray();

                    dependencies.AddRange(debDotNetDependencies);
                }

                // XZOutputStream class has low quality (doesn't even know it's current position,
                // needs to be disposed to finish compression, etc),
                // So we are doing compression in a separate step
                using (var tarXzStream = File.Open(this.DebTarXzPath, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
                using (var xzStream = new XZOutputStream(tarXzStream))
                {
                    tarStream.CopyTo(xzStream);
                }

                using (var tarXzStream = File.Open(this.DebTarXzPath, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    var pkg = DebPackageCreator.BuildDebPackage(
                        archiveEntries,
                        this.PackageName,
                        this.Description,
                        this.Maintainer,
                        this.Version,
                        !string.IsNullOrWhiteSpace(this.DebPackageArchitecture) ? this.DebPackageArchitecture : GetPackageArchitecture(this.RuntimeIdentifier),
                        this.CreateUser,
                        this.UserName,
                        this.InstallService,
                        this.ServiceName,
                        this.Prefix,
                        this.Section,
                        this.Priority,
                        this.Homepage,
                        this.PreInstallScript,
                        this.PostInstallScript,
                        this.PreRemoveScript,
                        this.PostRemoveScript,
                        dependencies,
                        null);

                    DebPackageCreator.WriteDebPackage(
                        archiveEntries,
                        tarXzStream,
                        targetStream,
                        pkg);
                }

                this.Log.LogMessage(
                    MessageImportance.High,
                    "Created DEB package '{0}' from folder '{1}'",
                    this.DebPath,
                    this.PublishDir);

                return true;
            }
        }

        private void EnsureDirectories(List<ArchiveEntry> entries)
        {
            var dirs = new HashSet<string>(entries.Where(x => x.Mode.HasFlag(LinuxFileMode.S_IFDIR))
                .Select(d => d.TargetPathWithFinalSlash));

            var toAdd = new List<ArchiveEntry>();

            string GetDirPath(string path)
            {
                path = path.TrimEnd('/');
                if (path == string.Empty)
                {
                    return "/";
                }

                if (!path.Contains("/"))
                {
                    return "/";
                }

                return path.Substring(0, path.LastIndexOf('/'));
            }

            void EnsureDir(string dirPath)
            {
                if (dirPath == string.Empty)
                {
                    return;
                }

                if (!dirs.Contains(dirPath))
                {
                    if (dirPath != "/")
                    {
                        EnsureDir(GetDirPath(dirPath));
                    }

                    dirs.Add(dirPath);
                    toAdd.Add(new ArchiveEntry()
                    {
                        Mode = LinuxFileMode.S_IXOTH | LinuxFileMode.S_IROTH | LinuxFileMode.S_IXGRP |
                               LinuxFileMode.S_IRGRP | LinuxFileMode.S_IXUSR | LinuxFileMode.S_IWUSR |
                               LinuxFileMode.S_IRUSR | LinuxFileMode.S_IFDIR,
                        Modified = DateTime.Now,
                        Group = "root",
                        Owner = "root",
                        TargetPath = dirPath,
                        LinkTo = string.Empty,
                    });
                }
            }

            foreach (var entry in entries)
            {
                EnsureDir(GetDirPath(entry.TargetPathWithFinalSlash));
            }

            EnsureDir("/");
            entries.AddRange(toAdd);
        }
    }
}