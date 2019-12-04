using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Packaging.Targets.IO;
using System;
using System.IO;
using System.IO.Compression;
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
            ArchiveBuilder archiveBuilder = new ArchiveBuilder()
            {
                Log = this.Log,
            };

            var archiveEntries = archiveBuilder.FromDirectory(
                this.PublishDir,
                null,
                this.Prefix,
                this.Content);

            DebTask.EnsureDirectories(archiveEntries, includeRoot: false);

            archiveEntries = archiveEntries
                .OrderBy(e => e.TargetPathWithFinalSlash, StringComparer.Ordinal)
                .ToList();

            using (var stream = File.Create(this.TarballPath))
            using (var gzipStream = new GZipStream(stream, CompressionMode.Compress))
            {
                TarFileCreator.FromArchiveEntries(archiveEntries, gzipStream);
            }
        }
    }
}
