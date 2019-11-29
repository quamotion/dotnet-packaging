using ICSharpCode.SharpZipLib.GZip;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Packaging.Targets.IO;
using System;
using System.IO;
using System.Linq;

namespace Packaging.Targets
{
    public class TarballTask : Task
    {
        [Required]
        public string PublishDir
        { get;  set; }

        [Required]
        public string TarballPath
        { get; set; }

        [Required]
        public ITaskItem[] Content
        { get; set; }

        [Required]
        public string Prefix
        { get; set; }

        public override bool Execute()
        {
            this.Log.LogMessage(MessageImportance.Normal, "Creating tarball '{0}' from folder '{1}'", this.TarballPath, this.PublishDir);

            this.CreateLinuxTarball();

            this.Log.LogMessage(MessageImportance.Normal, "Created tarball '{0}' from folder '{1}'", this.TarballPath, this.PublishDir);
            return true;
        }

        private void CreateLinuxTarball()
        {
            ArchiveBuilder archiveBuilder = new ArchiveBuilder();
            var archiveEntries = archiveBuilder.FromDirectory(
                this.PublishDir,
                this.Prefix,
                this.Content);

            DebTask.EnsureDirectories(archiveEntries);

            archiveEntries = archiveEntries
                .OrderBy(e => e.TargetPathWithFinalSlash, StringComparer.Ordinal)
                .ToList();

            using (var stream = File.Create(this.TarballPath))
            using (var gzipStream = new GZipOutputStream(stream))
            {
                TarFileCreator.FromArchiveEntries(archiveEntries, gzipStream);
            }
        }
    }
}
