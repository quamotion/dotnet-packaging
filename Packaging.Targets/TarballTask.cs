using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.IO;

namespace Packaging.Targets
{
    public class TarballTask : Task
    {
        [Required]
        public string PublishDir
        {
            get;
            set;
        }

        public string TarballPath
        {
            get;
            set;
        }

        public string TargetRuntime
        {
            get;
            set;
        }

        public string TargetFramework
        {
            get;
            set;
        }

        public override bool Execute()
        {
            this.Log.LogMessage(MessageImportance.Normal, "Creating tarball '{0}' from folder '{1}'", this.TarballPath, this.PublishDir);

            CreateWindowsTarball();

            this.Log.LogMessage(MessageImportance.Normal, "Created tarball '{0}' from folder '{1}'", this.TarballPath, this.PublishDir);
            return true;
        }

        private void CreateWindowsTarball()
        {
            using (var stream = File.Create(this.TarballPath))
            using (var zipFile = ZipFile.Create(stream))
            {
                zipFile.AddDirectory(this.PublishDir);
            }
        }

        private void CreateLinuxTarball()
        {
            using (var stream = File.Create(this.TarballPath))
            using (var gzipStream = new GZipOutputStream(stream))
            using (var archive = TarArchive.CreateOutputTarArchive(gzipStream))
            {
                var entry = TarEntry.CreateEntryFromFile(this.PublishDir);
                archive.WriteEntry(entry, true);
            }
        }
    }
}
