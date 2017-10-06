using DiscUtils.Internal;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Packaging.Targets.IO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace Packaging.Targets.Rpm
{
    /// <summary>
    /// Supports creating <see cref="RpmPackage"/> objects based on a <see cref="CpioFile"/>.
    /// </summary>
    internal class RpmPackageCreator
    {
        /// <summary>
        /// The <see cref="IFileAnalyzer"/> which analyzes the files in this package and provides required
        /// metadata for the files.
        /// </summary>
        private readonly IFileAnalyzer analyzer;

        /// <summary>
        /// Initializes a new instance of the <see cref="RpmPackageCreator"/> class.
        /// </summary>
        public RpmPackageCreator()
            : this(new FileAnalyzer())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RpmPackageCreator"/> class.
        /// </summary>
        /// <param name="analyzer">
        /// An <see cref="IFileAnalyzer"/> used to analyze the files in this package and provides required
        /// meatata for the files.
        /// </param>
        public RpmPackageCreator(IFileAnalyzer analyzer)
        {
            if (analyzer == null)
            {
                throw new ArgumentNullException(nameof(analyzer));
            }

            this.analyzer = analyzer;
        }

        /// <summary>
        /// Creates a RPM Package.
        /// </summary>
        /// <param name="archiveEntries">
        /// The archive entries which make up the RPM package.
        /// </param>
        /// <param name="payloadStream">
        /// A <see cref="Stream"/> which contains the CPIO archive for the RPM package.
        /// </param>
        /// <param name="name">
        /// The name of the package.
        /// </param>
        /// <param name="version">
        /// The version of the software.
        /// </param>
        /// <param name="arch">
        /// The architecture targetted by the package.
        /// </param>
        /// <param name="release">
        /// The release version.
        /// </param>
        /// <param name="additionalMetadata">
        /// Any additional metadata.
        /// </param>
        /// <param name="privateKey">
        /// The private key to use when signing the package.
        /// </param>
        /// <param name="targetStream">
        /// The <see cref="Stream"/> to which to write the package.
        /// </param>
        public void CreatePackage(
            List<ArchiveEntry> archiveEntries,
            Stream payloadStream,
            string name,
            string version,
            string arch,
            string release,
            bool createUser,
            string userName,
            bool installService,
            string serviceName,
            string prefix,
            IEnumerable<PackageDependency> additionalDependencies,
            Action<RpmMetadata> additionalMetadata,
            PgpPrivateKey privateKey,
            Stream targetStream)
        {
            // This routine goes roughly like:
            // 1. Calculate all the metadata, including a signature,
            //    but use an empty compressed payload to calculate
            //    the signature
            // 2. Write out the rpm file, and compress the payload
            // 3. Update the signature
            //
            // This way, we avoid having to compress the payload into a temporary
            // file.

            // Core routine to populate files and dependencies (part of the metadata
            // in the header)
            RpmPackage package = new RpmPackage();
            var metadata = new RpmMetadata(package)
            {
                Name = name,
                Version = version,
                Arch = arch,
                Release = release,
            };

            this.AddPackageProvides(metadata);
            this.AddLdDependencies(metadata);

            var files = this.CreateFiles(archiveEntries);
            metadata.Files = files;

            this.AddRpmDependencies(metadata, additionalDependencies);

            // Try to define valid defaults for most metadata
            metadata.Locales = new Collection<string> { "C" }; // Should come before any localizable data.
            metadata.BuildHost = "dotnet-rpm";
            metadata.BuildTime = DateTimeOffset.Now;
            metadata.Cookie = "dotnet-rpm";
            metadata.FileDigetsAlgo = PgpHashAlgo.PGPHASHALGO_SHA256;
            metadata.Group = "System Environment/Libraries";
            metadata.OptFlags = string.Empty;
            metadata.Os = "linux";
            metadata.PayloadCompressor = "xz";
            metadata.PayloadFlags = "2";
            metadata.PayloadFormat = "cpio";
            metadata.Platform = "x86_64-redhat-linux-gnu";
            metadata.RpmVersion = "4.11.3";
            metadata.SourcePkgId = new byte[0x10];
            metadata.SourceRpm = $"{name}-{version}-{release}.src.rpm";

            // Scripts which run before & after installation and removal.

            metadata.PreIn = string.Empty;
            metadata.PostIn = string.Empty;
            metadata.PreUn = string.Empty;
            metadata.PostUn = string.Empty;

            if (createUser)
            {
                // Add the user and group, under which the service runs.
                // These users are never removed because UIDs are re-used on Linux.
                metadata.PreIn += $"/usr/sbin/groupadd -r {userName} 2>/dev/null || :\n" +
                    $"/usr/sbin/useradd -g {userName} -s /sbin/nologin -r -d {prefix} {userName} 2>/dev/null || :\n";
            }

            if (installService)
            {
                // Install and activate the service.
                metadata.PostIn +=
                    $"if [ $1 -eq 1 ] ; then \n" +
                    $"    systemctl enable --now {serviceName}.service >/dev/null 2>&1 || : \n" +
                    $"fi\n";

                metadata.PreUn +=
                    $"if [ $1 -eq 0 ] ; then \n" +
                    $"    # Package removal, not upgrade \n" +
                    $"    systemctl --no-reload disable --now {serviceName}.service > /dev/null 2>&1 || : \n" +
                    $"fi\n";

                metadata.PostUn +=
                    $"if [ $1 -ge 1 ] ; then \n" +
                    $"    # Package upgrade, not uninstall \n" +
                    $"    systemctl try-restart {serviceName}.service >/dev/null 2>&1 || : \n" +
                    $"fi\n";
            }

            // Remove all directories marked as such (these are usually directories which contain temporary files)
            foreach (var entryToRemove in archiveEntries.Where(e => e.RemoveOnUninstall))
            {
                metadata.PreUn += $"/usr/bin/rm -rf {entryToRemove.TargetPath}\n";
            }

            // All these actions are shell scripts.
            metadata.PreInProg = "/bin/sh";
            metadata.PostInProg = "/bin/sh";
            metadata.PreUnProg = "/bin/sh";
            metadata.PostUnProg = "/bin/sh";

            // Not providing these (or setting empty values) would cause rpmlint errors
            metadata.Description = $"{name} version {version}-{release}";
            metadata.Summary = $"{name} version {version}-{release}";
            metadata.License = $"{name} License";

            metadata.Distribution = string.Empty;
            metadata.DistUrl = string.Empty;
            metadata.Url = string.Empty;
            metadata.Vendor = string.Empty;

            metadata.ChangelogEntries = new Collection<ChangelogEntry>()
            {
                new ChangelogEntry(DateTimeOffset.Now, "dotnet-rpm", "Created a RPM package using dotnet-rpm")
            };

            // User-set metadata
            if (additionalMetadata != null)
            {
                additionalMetadata(metadata);
            }

            this.CalculateHeaderOffsets(package);

            using (MemoryStream dummyCompressedPayload = new MemoryStream())
            {
                using (XZOutputStream dummyPayloadCompressor = new XZOutputStream(dummyCompressedPayload, 1, XZOutputStream.DefaultPreset, leaveOpen: true))
                {
                    dummyPayloadCompressor.Write(new byte[] { 0 }, 0, 1);
                }

                this.CalculateSignature(package, privateKey, dummyCompressedPayload);
            }

            this.CalculateSignatureOffsets(package);

            // Write out all the data - includes the lead
            byte[] nameBytes = new byte[66];

            Encoding.UTF8.GetBytes(name, 0, name.Length, nameBytes, 0);

            var lead = new RpmLead()
            {
                ArchNum = 1,
                Magic = 0xedabeedb,
                Major = 0x03,
                Minor = 0x00,
                NameBytes = nameBytes,
                OsNum = 0x0001,
                Reserved = new byte[16],
                SignatureType = 0x0005,
                Type = 0x0000,
            };

            // Write out the lead, signature and header
            targetStream.Position = 0;
            targetStream.SetLength(0);

            targetStream.WriteStruct(lead);
            this.WriteSignature(package, targetStream);
            this.WriteHeader(package, targetStream);

            // Write out the compressed payload
            int compressedPayloadOffset = (int)targetStream.Position;

            using (XZOutputStream compressor = new XZOutputStream(targetStream, 1, XZOutputStream.DefaultPreset, leaveOpen: true))
            {
                payloadStream.Position = 0;
                payloadStream.CopyTo(compressor);
            }

            using (SubStream compressedPayloadStream = new SubStream(
                targetStream,
                compressedPayloadOffset,
                targetStream.Length - compressedPayloadOffset,
                leaveParentOpen: true,
                readOnly: true))
            {
                this.CalculateSignature(package, privateKey, compressedPayloadStream);
                this.CalculateSignatureOffsets(package);
            }

            // Update the lead and signature
            targetStream.Position = 0;

            targetStream.WriteStruct(lead);
            this.WriteSignature(package, targetStream);
        }

        /// <summary>
        /// Creates the metadata for all files in the <see cref="CpioFile"/>.
        /// </summary>
        /// <param name="archiveEntries">
        /// The archive entries for which to generate the metadata.
        /// </param>
        /// <returns>
        /// A <see cref="Collection{RpmFile}"/> which contains all the metadata.
        /// </returns>
        public Collection<RpmFile> CreateFiles(List<ArchiveEntry> archiveEntries)
        {
            Collection<RpmFile> files = new Collection<RpmFile>();

            foreach (var entry in archiveEntries)
            {
                var size = entry.FileSize;

                if (entry.Mode.HasFlag(LinuxFileMode.S_IFDIR))
                {
                    size = 0x1000;
                }

                RpmFile file = new RpmFile()
                {
                    Size = (int)size,
                    Mode = entry.Mode,
                    Rdev = 0,
                    ModifiedTime = entry.Modified,
                    MD5Hash = entry.Sha256, // Yes, the MD5 hash does not actually contain a MD5 hash
                    LinkTo = entry.LinkTo,
                    Flags = this.analyzer.DetermineFlags(entry),
                    UserName = entry.Owner,
                    GroupName = entry.Group,
                    VerifyFlags = RpmVerifyFlags.RPMVERIFY_ALL,
                    Device = 1,
                    Inode = (int)entry.Inode,
                    Lang = "",
                    Color = this.analyzer.DetermineColor(entry),
                    Class = this.analyzer.DetermineClass(entry),
                    Requires = this.analyzer.DetermineRequires(entry),
                    Provides = this.analyzer.DetermineProvides(entry),
                    Name = entry.TargetPath
                };

                files.Add(file);
            }

            return files;
        }

        /// <summary>
        /// Adds the package-level provides to the metadata. These are basically statements
        /// indicating that the package provides, well, itself.
        /// </summary>
        /// <param name="metadata">
        /// The package to which to add the provides.
        /// </param>
        public void AddPackageProvides(RpmMetadata metadata)
        {
            var provides = metadata.Provides.ToList();

            var packageProvides = new PackageDependency(metadata.Name, RpmSense.RPMSENSE_EQUAL, $"{metadata.Version}-{metadata.Release}");

            var normalizedArch = metadata.Arch;
            if (normalizedArch == "x86_64")
            {
                normalizedArch = "x86-64";
            }

            var packageArchProvides = new PackageDependency($"{metadata.Name}({normalizedArch})", RpmSense.RPMSENSE_EQUAL, $"{metadata.Version}-{metadata.Release}");

            if (!provides.Contains(packageProvides))
            {
                provides.Add(packageProvides);
            }

            if (!provides.Contains(packageArchProvides))
            {
                provides.Add(packageArchProvides);
            }

            metadata.Provides = provides;
        }

        /// <summary>
        /// Adds the dependency on ld to the RPM package. These dependencies cause <c>ldconfig</c> to run post installation
        /// and uninstallation of the RPM package.
        /// </summary>
        /// <param name="metadata">
        /// The <see cref="RpmMetadata"/> to which to add the dependencies.
        /// </param>
        public void AddLdDependencies(RpmMetadata metadata)
        {
            Collection<PackageDependency> ldDependencies = new Collection<PackageDependency>()
            {
                new PackageDependency("/sbin/ldconfig", RpmSense.RPMSENSE_INTERP | RpmSense.RPMSENSE_SCRIPT_POST, string.Empty),
                new PackageDependency("/sbin/ldconfig", RpmSense.RPMSENSE_INTERP | RpmSense.RPMSENSE_SCRIPT_POSTUN,string.Empty)
            };

            var dependencies = metadata.Dependencies.ToList();
            dependencies.AddRange(ldDependencies);
            metadata.Dependencies = dependencies;
        }

        /// <summary>
        /// Adds the RPM dependencies to the package. These dependencies express dependencies on specific RPM features, such as compressed file names,
        /// file digets, and xz-compressed payloads.
        /// </summary>
        /// <param name="metadata">
        /// The <see cref="RpmMetadata"/> to which to add the dependencies.
        /// </param>
        public void AddRpmDependencies(RpmMetadata metadata, IEnumerable<PackageDependency> additionalDependencies)
        {
            // Somehow, three rpmlib dependencies come before the rtld(GNU_HASH) dependency and one after.
            // The rtld(GNU_HASH) indicates that hashes are stored in the .gnu_hash instead of the .hash section
            // in the ELF file, so it is a file-level dependency that bubbles up
            // http://lists.rpm.org/pipermail/rpm-maint/2014-September/003764.html
            //9:34 PM 10/6/2017
            // To work around it, we remove the rtld(GNU_HASH) dependency on the files, remove it as a dependency,
            // and add it back once we're done.
            //
            // The sole purpose of this is to ensure binary compatibility, which is probably not required at runtime,
            // but makes certain regression tests more stable.
            //
            // Here we go:
            var files = metadata.Files.ToArray();

            var gnuHashFiles =
                files
                .Where(f => f.Requires.Any(r => string.Equals(r.Name, "rtld(GNU_HASH)", StringComparison.Ordinal)))
                .ToArray();

            foreach (var file in gnuHashFiles)
            {
                var rtldDependency = file.Requires.Where(r => string.Equals(r.Name, "rtld(GNU_HASH)", StringComparison.Ordinal)).Single();
                file.Requires.Remove(rtldDependency);
            }

            // Refresh
            metadata.Files = files;

            Collection<PackageDependency> rpmDependencies = new Collection<PackageDependency>()
            {
                new PackageDependency("rpmlib(CompressedFileNames)",RpmSense.RPMSENSE_LESS | RpmSense.RPMSENSE_EQUAL | RpmSense.RPMSENSE_RPMLIB, "3.0.4-1"),
                new PackageDependency("rpmlib(FileDigests)",RpmSense.RPMSENSE_LESS | RpmSense.RPMSENSE_EQUAL | RpmSense.RPMSENSE_RPMLIB, "4.6.0-1"),
                new PackageDependency("rpmlib(PayloadFilesHavePrefix)", RpmSense.RPMSENSE_LESS | RpmSense.RPMSENSE_EQUAL | RpmSense.RPMSENSE_RPMLIB,"4.0-1"),
                new PackageDependency("rtld(GNU_HASH)",RpmSense.RPMSENSE_FIND_REQUIRES, string.Empty),
            };

            // Inject any additional dependencies the user may have specified.
            if (additionalDependencies != null)
            {
                rpmDependencies.AddRange(additionalDependencies);
            }

            rpmDependencies.Add(new PackageDependency("rpmlib(PayloadIsXz)", RpmSense.RPMSENSE_LESS | RpmSense.RPMSENSE_EQUAL | RpmSense.RPMSENSE_RPMLIB, "5.2-1"));

            var dependencies = metadata.Dependencies.ToList();
            var last = dependencies.Last();

            if (last.Name == "rtld(GNU_HASH)")
            {
                dependencies.Remove(last);
            }

            dependencies.AddRange(rpmDependencies);
            metadata.Dependencies = dependencies;

            // Add the rtld(GNU_HASH) dependency back to the files
            foreach (var file in gnuHashFiles)
            {
                file.Requires.Add(new PackageDependency("rtld(GNU_HASH)", RpmSense.RPMSENSE_FIND_REQUIRES, string.Empty));
            }

            // Refresh
            metadata.Files = files;
        }

        /// <summary>
        /// Determines the offsets for all records in the header of a package.
        /// </summary>
        /// <param name="package">
        /// The package for which to generate the offsets.
        /// </param>
        public void CalculateHeaderOffsets(RpmPackage package)
        {
            var metadata = new RpmMetadata(package);
            metadata.ImmutableRegionSize = -1 * Marshal.SizeOf<IndexHeader>() * (package.Header.Records.Count + 1);

            CalculateSectionOffsets(package.Header, k => (int)k);
        }

        /// <summary>
        /// Determines the offsets for all records in the signature of a package.
        /// </summary>
        /// <param name="package">
        /// The package for which to generate the offsets.
        /// </param>
        public void CalculateSignatureOffsets(RpmPackage package)
        {
            var signature = new RpmSignature(package);
            signature.ImmutableRegionSize = -1 * Marshal.SizeOf<IndexHeader>() * (package.Signature.Records.Count);

            CalculateSectionOffsets(package.Signature, k => (int)k);
        }

        /// <summary>
        /// Gets a <see cref="MemoryStream"/> wich represents the entire header.
        /// </summary>
        /// <param name="package">
        /// The package for which to get the header stream.
        /// </param>
        /// <returns>
        /// A <see cref="MemoryStream"/> wich represents the entire header.
        /// </returns>
        public MemoryStream GetHeaderStream(RpmPackage package)
        {
            MemoryStream stream = new MemoryStream();
            WriteHeader(package, stream);
            stream.Position = 0;
            return stream;
        }

        /// <summary>
        /// Writes the header to a <see cref="Stream"/>.
        /// </summary>
        /// <param name="package">
        /// The package for which to write the header.
        /// </param>
        /// <param name="targetStream">
        /// The <see cref="Stream"/> to which to write the header.
        /// </param>
        public void WriteHeader(RpmPackage package, Stream targetStream)
        {
            RpmPackageWriter.WriteSection(targetStream, package.Header, DefaultOrder.Header);
        }

        /// <summary>
        /// Gets a <see cref="MemoryStream"/> wich represents the entire signature.
        /// </summary>
        /// <param name="package">
        /// The package for which to get the header stream.
        /// </param>
        /// <returns>
        /// A <see cref="MemoryStream"/> wich represents the entire signature.
        /// </returns>
        public MemoryStream GetSignatureStream(RpmPackage package)
        {
            MemoryStream stream = new MemoryStream();
            WriteSignature(package, stream);
            stream.Position = 0;
            return stream;
        }

        /// <summary>
        /// Writes the signature to a <see cref="Stream"/>.
        /// </summary>
        /// <param name="package">
        /// The package for which to write the signature.
        /// </param>
        /// <param name="targetStream">
        /// The <see cref="Stream"/> tow chih to write the package.
        /// </param>
        public void WriteSignature(RpmPackage package, Stream targetStream)
        {
            RpmPackageWriter.WriteSection(targetStream, package.Signature, DefaultOrder.Signature);
        }

        /// <summary>
        /// Calculates the signature for this package.
        /// </summary>
        /// <param name="package">
        /// The package for whcih to calculate the signature.
        /// </param>
        /// <param name="privateKey">
        /// The private key to use.
        /// </param>
        /// <param name="compressedPayloadStream">
        /// The compressed payload.
        /// </param>
        public void CalculateSignature(RpmPackage package, PgpPrivateKey privateKey, Stream compressedPayloadStream)
        {
            RpmSignature signature = new RpmSignature(package);

            using (MemoryStream headerStream = this.GetHeaderStream(package))
            using (ConcatStream headerAndPayloadStream = new ConcatStream(leaveOpen: true, streams: new Stream[] { headerStream, compressedPayloadStream }))
            {
                SHA1 sha = SHA1.Create();
                signature.Sha1Hash = sha.ComputeHash(headerStream);

                MD5 md5 = MD5.Create();
                signature.MD5Hash = md5.ComputeHash(headerAndPayloadStream);

                // Verify the PGP signatures
                // 3 for the header
                headerStream.Position = 0;
                signature.HeaderPgpSignature = PgpSigner.Sign(privateKey, headerStream);

                headerAndPayloadStream.Position = 0;
                signature.HeaderAndPayloadPgpSignature = PgpSigner.Sign(privateKey, headerAndPayloadStream);

                // Verify the signature size (header + compressed payload)
                signature.HeaderAndPayloadSize = (int)headerAndPayloadStream.Length;
            }

            // Verify the payload size (header + uncompressed payload)

            using (Stream payloadStream = RpmPayloadReader.GetDecompressedPayloadStream(package, compressedPayloadStream))
            {
                signature.UncompressedPayloadSize = (int)payloadStream.Length;
            }
        }

        protected void CalculateSectionOffsets<T>(Section<T> section, Func<T, int> getSortOrder)
        {
            var records = section.Records;
            int offset = 0;

            T immutableKey = default(T);

            var sortedRecords = records.OrderBy(r => getSortOrder(r.Key)).ToArray();

            foreach (var record in sortedRecords)
            {
                var indexTag = record.Key as IndexTag?;
                var signatureTag = record.Key as SignatureTag?;

                if (indexTag == IndexTag.RPMTAG_HEADERIMMUTABLE
                    || signatureTag == SignatureTag.RPMTAG_HEADERSIGNATURES)
                {
                    immutableKey = record.Key;
                    continue;
                }

                var header = record.Value.Header;

                // Determine the size
                int size = 0;
                int align = 0;

                switch (header.Type)
                {
                    case IndexType.RPM_BIN_TYPE:
                    case IndexType.RPM_CHAR_TYPE:
                    case IndexType.RPM_INT8_TYPE:
                        // These are all single-byte types
                        size = header.Count;
                        break;

                    case IndexType.RPM_I18NSTRING_TYPE:
                    case IndexType.RPM_STRING_ARRAY_TYPE:
                        foreach (var value in (IEnumerable<string>)record.Value.Value)
                        {
                            size += Encoding.UTF8.GetByteCount(value) + 1;
                        }
                        break;

                    case IndexType.RPM_STRING_TYPE:
                        size = Encoding.UTF8.GetByteCount((string)record.Value.Value) + 1;
                        break;

                    case IndexType.RPM_INT16_TYPE:
                        size = 2 * header.Count;
                        break;

                    case IndexType.RPM_INT32_TYPE:
                        size = 4 * header.Count;
                        align = 4;
                        break;

                    case IndexType.RPM_INT64_TYPE:
                        size = 8 * header.Count;
                        break;

                    case IndexType.RPM_NULL_TYPE:
                        size = 0;
                        break;
                }

                if (align != 0 && offset % align != 0)
                {
                    offset += align - (offset % align);
                }

                header.Offset = offset;
                offset += size;

                record.Value.Header = header;
            }

            if (!object.Equals(immutableKey, default(T)))
            {
                var record = records[immutableKey];
                var header = record.Header;
                header.Offset = offset;
                record.Header = header;

                offset += Marshal.SizeOf<IndexHeader>();
            }

            // This is also a good time to refresh the header
            section.Header = new RpmHeader()
            {
                HeaderSize = (uint)offset,
                IndexCount = (uint)section.Records.Count,
                Magic = 0x8eade801,
                Reserved = 0
            };
        }
    }
}
