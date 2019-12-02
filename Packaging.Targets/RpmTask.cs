using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Packaging.Targets.IO;
using Packaging.Targets.Rpm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

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
        /// Gets or sets the runtime identifier for which we are currently building.
        /// Used to determine the target architecture of the package.
        /// </summary>
        public string RuntimeIdentifier { get; set; }

        /// <summary>
        /// Gets or sets the package architecture (x86_64, i686,...). When not set,
        /// the target architecture will be derived based on the <see cref="RuntimeIdentifier"/>
        /// </summary>
        public string RpmPackageArchitecture { get; set; }

        /// <summary>
        /// Gets or sets the path to the app host. This is a native executable which loads
        /// the .NET Core runtime, and invokes the manage assembly. On Linux, it is symlinked
        /// into ${prefix}/bin.
        /// </summary>
        public string AppHost { get; set; }

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
        /// Gets or sets a list of RPM packages on which the version of .NET Core
        /// embedded in this RPM package dpeends.
        /// </summary>
        public ITaskItem[] RpmDotNetDependencies
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
            // Some architectures are listed here:
            // - https://wiki.centos.org/Download
            // - https://docs.fedoraproject.org/ro/Fedora_Draft_Documentation/0.1/html/RPM_Guide/ch01s03.html
            RuntimeIdentifiers.ParseRuntimeId(runtimeIdentifier, out _, out _, out Architecture? architecture, out _);

            if (architecture != null)
            {
                switch (architecture.Value)
                {
                    case Architecture.Arm:
                        return "armhfp";

                    case Architecture.Arm64:
                        return "aarch64";

                    case Architecture.X64:
                        return "x86_64";

                    case Architecture.X86:
                        return "i386";
                }
            }

            return "noarch";
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
                    this.AppHost,
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
                List<PackageDependency> dependencies = new List<PackageDependency>();

                if (this.RpmDotNetDependencies != null)
                {
                    dependencies.AddRange(
                        this.RpmDotNetDependencies.Select(
                            d => GetPackageDependency(d)));
                }

                if (this.RpmDependencies != null)
                {
                    dependencies.AddRange(
                        this.RpmDependencies.Select(
                            d => GetPackageDependency(d)));
                }

                RpmPackageCreator rpmCreator = new RpmPackageCreator();
                rpmCreator.CreatePackage(
                    archiveEntries,
                    cpioStream,
                    this.PackageName,
                    this.Version,
                    !string.IsNullOrEmpty(this.RpmPackageArchitecture) ? this.RpmPackageArchitecture : GetPackageArchitecture(this.RuntimeIdentifier),
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

        private static PackageDependency GetPackageDependency(ITaskItem dependency)
        {
            if (dependency == null)
            {
                return null;
            }

            return new PackageDependency(
                dependency.ItemSpec,
                RpmSense.RPMSENSE_EQUAL | RpmSense.RPMSENSE_GREATER,
                dependency.GetVersion());
        }
    }
}
