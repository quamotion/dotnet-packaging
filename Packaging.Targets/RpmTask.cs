using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Packaging.Targets.IO;
using Packaging.Targets.Rpm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Packaging.Targets
{
    public class RpmTask : Task
    {
        [Required]
        public string PublishDir
        {
            get;
            set;
        }

        [Required]
        public string RpmPath
        {
            get;
            set;
        }

        [Required]
        public string CpioPath
        {
            get;
            set;
        }

        [Required]
        public string Prefix
        {
            get;
            set;
        }

        [Required]
        public string Version
        {
            get;
            set;
        }

        [Required]
        public string Release
        {
            get;
            set;
        }

        [Required]
        public string PackageName
        {
            get;
            set;
        }

        public override bool Execute()
        {
            this.Log.LogMessage(MessageImportance.Normal, "Creating RPM package '{0}' from folder '{1}'", this.RpmPath, this.PublishDir);

            var krgen = PgpSigner.GenerateKeyRingGenerator("dotnet", "dotnet");
            var secretKeyRing = krgen.GenerateSecretKeyRing();
            var privateKey = secretKeyRing.GetSecretKey().ExtractPrivateKey("dotnet".ToCharArray());
            var publicKey = secretKeyRing.GetPublicKey();

            using (var targetStream = File.Open(this.RpmPath, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
            using (var cpioStream = File.Open(this.CpioPath, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
            {
                ArchiveBuilder archiveBuilder = new ArchiveBuilder();
                var archiveEntries = archiveBuilder.FromDirectory(
                    this.PublishDir,
                    this.Prefix);

                CpioFileCreator cpioCreator = new CpioFileCreator();
                cpioCreator.FromArchiveEntries(
                    archiveEntries,
                    cpioStream);
                cpioStream.Position = 0;

                RpmPackageCreator rpmCreator = new RpmPackageCreator();
                rpmCreator.CreatePackage(
                    archiveEntries,
                    cpioStream,
                    this.PackageName,
                    this.Version,
                    "x86_64",
                    this.Release,
                    null,
                    privateKey,
                    targetStream);
            }

            this.Log.LogMessage(MessageImportance.Normal, "Created RPM package '{0}' from folder '{1}'", this.RpmPath, this.PublishDir);
            return true;
        }
    }
}
