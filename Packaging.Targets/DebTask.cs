using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
                    DebPackageCreator.BuildDebPackage(
                        archiveEntries,
                        tarXzStream,
                        this.PackageName,
                        this.Description,
                        this.Maintainer,
                        this.Version,
                        "amd64",
                        this.CreateUser,
                        this.UserName,
                        this.InstallService,
                        this.ServiceName,
                        this.Prefix,
                        dependencies,
                        null,
                        targetStream);
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