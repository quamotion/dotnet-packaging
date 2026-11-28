using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.IO;
using System.IO.Compression;

namespace Packaging.Targets
{
    public class ZipTask : Task
    {
        [Required]
        public string PublishDir
        {
            get;
            set;
        }

        [Required]
        public string ZipPath
        {
            get;
            set;
        }

        public override bool Execute()
        {
            this.Log.LogMessage(MessageImportance.Normal, "Creating zip archive '{0}' from folder '{1}'", this.ZipPath, this.PublishDir);

            this.CreateWindowsTarball();

            this.Log.LogMessage(MessageImportance.Normal, "Created zip archive '{0}' from folder '{1}'", this.ZipPath, this.PublishDir);
            return true;
        }

        private void CreateWindowsTarball()
        {
            // ZipFile.CreateFromDirectory fails if the file exists; the previous implementation overwrote it.
            File.Delete(this.ZipPath);
            ZipFile.CreateFromDirectory(this.PublishDir, this.ZipPath, CompressionLevel.Optimal, includeBaseDirectory: false);
        }
    }
}
