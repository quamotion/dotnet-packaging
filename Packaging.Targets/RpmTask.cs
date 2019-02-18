using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Packaging.Targets.IO;
using Packaging.Targets.Rpm;
using System;
using System.IO;
using System.Linq;

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

        [Required]
        public ITaskItem[] Content
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a list of empty folders to create when
        /// installing this package.
        /// </summary>
        public ITaskItem[] LinuxFolders
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a list of RPM packages on which this RPM
        /// package dpeends.
        /// </summary>
        public ITaskItem[] RpmDependencies
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to create a Linux
        /// user and group when installing the package.
        /// </summary>
        public bool CreateUser
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the Linux user and group to create.
        /// </summary>
        public string UserName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to install
        /// and launch as systemd service when installing the package.
        /// </summary>
        public bool InstallService
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the SystemD service to create.
        /// </summary>
        public string ServiceName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the package vendor.
        /// </summary>
        public string Vendor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the package description.
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the package URL.
        /// </summary>
        public string Url
        {
            get;
            set;
        }

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
                    this.Prefix,
                    this.Content);

                archiveEntries.AddRange(archiveBuilder.FromLinuxFolders(this.LinuxFolders));
                archiveEntries = archiveEntries
                    .OrderBy(e => e.TargetPathWithFinalSlash, StringComparer.Ordinal)
                    .ToList();

                CpioFileCreator cpioCreator = new CpioFileCreator();
                cpioCreator.FromArchiveEntries(
                    archiveEntries,
                    cpioStream);
                cpioStream.Position = 0;

                // Prepare the list of dependencies
                PackageDependency[] dependencies = Array.Empty<PackageDependency>();

                if (this.RpmDependencies != null)
                {
                    dependencies = this.RpmDependencies.Select(
                        d => new PackageDependency(
                            d.ItemSpec,
                            RpmSense.RPMSENSE_EQUAL | RpmSense.RPMSENSE_GREATER,
                            d.GetVersion()))
                        .ToArray();
                }

                RpmPackageCreator rpmCreator = new RpmPackageCreator();
                rpmCreator.CreatePackage(
                    archiveEntries,
                    cpioStream,
                    this.PackageName,
                    this.Version,
                    "x86_64",
                    this.Release,
                    this.CreateUser,
                    this.UserName,
                    this.InstallService,
                    this.ServiceName,
                    this.Vendor,
                    this.Description,
                    this.Url,
                    this.Prefix,
                    this.PreInstallScript,
                    this.PostInstallScript,
                    this.PreRemoveScript,
                    this.PostRemoveScript,
                    dependencies,
                    null,
                    privateKey,
                    targetStream);
            }

            this.Log.LogMessage(MessageImportance.Normal, "Created RPM package '{0}' from folder '{1}'", this.RpmPath, this.PublishDir);
            return true;
        }
    }
}
