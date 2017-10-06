using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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
            using (var stream = File.Create(this.ZipPath))
            using (var zipFile = ZipFile.Create(stream))
            {
                this.AddDirectory(zipFile, this.PublishDir, string.Empty);
            }
        }

        private void AddDirectory(ZipFile zipFile, string directory, string directoryEntryName)
        {
            if (directoryEntryName != string.Empty)
            {
                zipFile.BeginUpdate();
                zipFile.AddDirectory(directoryEntryName);
                zipFile.CommitUpdate();
            }

            foreach (var file in Directory.GetFiles(directory))
            {
                zipFile.BeginUpdate();
                zipFile.Add(file, Path.Combine(directoryEntryName, Path.GetFileName(file)));
                zipFile.CommitUpdate();
            }

            foreach (var child in Directory.GetDirectories(directory))
            {
                this.AddDirectory(zipFile, child, Path.Combine(directoryEntryName, Path.GetFileName(child)));
            }
        }
    }
}
