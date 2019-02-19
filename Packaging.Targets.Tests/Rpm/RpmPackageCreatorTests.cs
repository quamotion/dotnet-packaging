using Org.BouncyCastle.Bcpg.OpenPgp;
using Packaging.Targets.IO;
using Packaging.Targets.Rpm;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Xunit;

namespace Packaging.Targets.Tests.Rpm
{
    /// <summary>
    /// Tests the <see cref="RpmPackageCreator"/> class.
    /// </summary>
    public class RpmPackageCreatorTests
    {
        /// <summary>
        /// Tests the <see cref="RpmPackageCreator.CreateFiles(CpioFile)"/> method, making sure the list of files which is returned
        /// for a given CPIO archive is correct. Uses the libplist RPM package as a test payload.
        /// </summary>
        [InlineData(@"Rpm/libplist-2.0.1.151-1.1.x86_64.rpm", "plist")]
        [InlineData(@"Rpm/dotnet_test-1.0-0.noarch.rpm", "dotnet")]
        [Theory]
        public void CreateFiles(string rpm, string analyzerName)
        {
            using (Stream stream = File.OpenRead(rpm))
            {
                var originalPackage = RpmPackageReader.Read(stream);

                using (var payloadStream = RpmPayloadReader.GetDecompressedPayloadStream(originalPackage))
                using (var cpio = new CpioFile(payloadStream, false))
                {
                    IFileAnalyzer analyzer = new FileAnalyzer();

                    switch (analyzerName)
                    {
                        case "plist":
                            analyzer = new PlistFileAnalyzer();
                            break;

                        case "dotnet":
                            analyzer = new DotnetFileAnalyzer();
                            break;
                    }

                    RpmPackageCreator creator = new RpmPackageCreator(analyzer);
                    ArchiveBuilder builder = new ArchiveBuilder(analyzer);
                    var entries = builder.FromCpio(cpio);
                    var files = creator.CreateFiles(entries);

                    var originalMetadata = new RpmMetadata(originalPackage);

                    var originalFiles = originalMetadata.Files.ToArray();

                    Assert.Equal(originalFiles.Length, files.Count);

                    for (int i = 0; i < originalFiles.Length; i++)
                    {
                        var originalFile = originalFiles[i];
                        var file = files[i];

                        Assert.Equal(originalFile.Class, file.Class);
                        Assert.Equal(originalFile.Color, file.Color);
                        Assert.Equal(originalFile.Requires, file.Requires);
                        Assert.Equal(originalFile.Provides, file.Provides);
                        Assert.Equal(originalFile.Device, file.Device);
                        Assert.Equal(originalFile.Flags, file.Flags);
                        Assert.Equal(originalFile.GroupName, file.GroupName);
                        Assert.Equal(originalFile.Inode, file.Inode);
                        Assert.Equal(originalFile.Lang, file.Lang);
                        Assert.Equal(originalFile.LinkTo, file.LinkTo);
                        Assert.Equal(originalFile.MD5Hash, file.MD5Hash);
                        Assert.Equal(originalFile.Mode, file.Mode);
                        Assert.Equal(originalFile.ModifiedTime, file.ModifiedTime);
                        Assert.Equal(originalFile.Name, file.Name);
                        Assert.Equal(originalFile.Rdev, file.Rdev);
                        Assert.Equal(originalFile.Size, file.Size);
                        Assert.Equal(originalFile.UserName, file.UserName);
                        Assert.Equal(originalFile.VerifyFlags, file.VerifyFlags);
                    }
                }
            }
        }

        /// <summary>
        /// Tests the <see cref="RpmPackageCreator.CalculateHeaderOffsets(RpmPackage)"/> method.
        /// </summary>
        [Fact]
        public void CalculateOffsetTest()
        {
            using (Stream stream = File.OpenRead(@"Rpm/libplist-2.0.1.151-1.1.x86_64.rpm"))
            {
                var originalPackage = RpmPackageReader.Read(stream);

                using (var payloadStream = RpmPayloadReader.GetDecompressedPayloadStream(originalPackage))
                using (var cpio = new CpioFile(payloadStream, false))
                {
                    ArchiveBuilder builder = new ArchiveBuilder(new PlistFileAnalyzer());
                    RpmPackageCreator creator = new RpmPackageCreator(new PlistFileAnalyzer());
                    var entries = builder.FromCpio(cpio);
                    var files = creator.CreateFiles(entries);

                    // Core routine to populate files and dependencies
                    RpmPackage package = new RpmPackage();
                    var metadata = new PublicRpmMetadata(package);
                    metadata.Name = "libplist";
                    metadata.Version = "2.0.1.151";
                    metadata.Arch = "x86_64";
                    metadata.Release = "1.1";

                    creator.AddPackageProvides(metadata);
                    creator.AddLdDependencies(metadata);

                    metadata.Files = files;
                    creator.AddRpmDependencies(metadata, null);

                    PlistMetadata.ApplyDefaultMetadata(metadata);

                    metadata.Vendor = "obs://build.opensuse.org/home:qmfrederik";
                    metadata.Description = "libplist is a library for manipulating Apple Binary and XML Property Lists";
                    metadata.Url = "http://www.libimobiledevice.org/";
                    metadata.Size = 0x26e6d;
                    metadata.ImmutableRegionSize = -976;

                    creator.CalculateHeaderOffsets(package);

                    foreach (var record in originalPackage.Header.Records)
                    {
                        if (record.Key == IndexTag.RPMTAG_HEADERIMMUTABLE)
                        {
                            continue;
                        }

                        this.AssertTagOffsetEqual(record.Key, originalPackage, package);
                    }

                    this.AssertTagOffsetEqual(IndexTag.RPMTAG_HEADERIMMUTABLE, originalPackage, package);
                }
            }
        }

        [Fact]
        public void CalculateSignatureTest()
        {
            using (Stream stream = File.OpenRead(@"Rpm/libplist-2.0.1.151-1.1.x86_64.rpm"))
            {
                var originalPackage = RpmPackageReader.Read(stream);

                RpmPackageCreator creator = new RpmPackageCreator(new PlistFileAnalyzer());
                Collection<RpmFile> files;

                using (var payloadStream = RpmPayloadReader.GetDecompressedPayloadStream(originalPackage))
                using (var cpio = new CpioFile(payloadStream, false))
                {
                    ArchiveBuilder builder = new ArchiveBuilder(new PlistFileAnalyzer());
                    var entries = builder.FromCpio(cpio);
                    files = creator.CreateFiles(entries);
                }

                // Core routine to populate files and dependencies
                RpmPackage package = new RpmPackage();
                var metadata = new PublicRpmMetadata(package);
                metadata.Name = "libplist";
                metadata.Version = "2.0.1.151";
                metadata.Arch = "x86_64";
                metadata.Release = "1.1";

                creator.AddPackageProvides(metadata);
                creator.AddLdDependencies(metadata);

                metadata.Files = files;
                creator.AddRpmDependencies(metadata, null);

                PlistMetadata.ApplyDefaultMetadata(metadata);

                metadata.Vendor = "obs://build.opensuse.org/home:qmfrederik";
                metadata.Description = "libplist is a library for manipulating Apple Binary and XML Property Lists";
                metadata.Url = "http://www.libimobiledevice.org/";

                creator.CalculateHeaderOffsets(package);

                // Make sure the header is really correct
                using (Stream originalHeaderStream = new SubStream(
                    originalPackage.Stream,
                    originalPackage.HeaderOffset,
                    originalPackage.PayloadOffset - originalPackage.HeaderOffset,
                    leaveParentOpen: true,
                    readOnly: true))
                using (Stream headerStream = creator.GetHeaderStream(package))
                {
                    byte[] originalData = new byte[originalHeaderStream.Length];
                    originalHeaderStream.Read(originalData, 0, originalData.Length);

                    byte[] data = new byte[headerStream.Length];
                    headerStream.Read(data, 0, data.Length);

                    int delta = 0;
                    int dataDelta = 0;
                    IndexTag tag;
                    for (int i = 0; i < data.Length; i++)
                    {
                        if (originalData[i] != data[i])
                        {
                            delta = i;
                            dataDelta = delta - package.Header.Records.Count * Marshal.SizeOf<IndexHeader>();
                            tag = package.Header.Records.OrderBy(r => r.Value.Header.Offset).Last(r => r.Value.Header.Offset <= dataDelta).Key;

                            break;
                        }
                    }

                    Assert.Equal(originalData, data);
                }

                var krgen = PgpSigner.GenerateKeyRingGenerator("dotnet", "dotnet");
                var secretKeyRing = krgen.GenerateSecretKeyRing();
                var privateKey = secretKeyRing.GetSecretKey().ExtractPrivateKey("dotnet".ToCharArray());

                using (var payload = RpmPayloadReader.GetCompressedPayloadStream(originalPackage))
                {
                    // Header should be OK now (see previous test), so now get the signature block and the
                    // trailer
                    creator.CalculateSignature(package, privateKey, payload);
                    creator.CalculateSignatureOffsets(package);

                    foreach (var record in originalPackage.Signature.Records)
                    {
                        if (record.Key == SignatureTag.RPMTAG_HEADERSIGNATURES)
                        {
                            continue;
                        }

                        this.AssertTagEqual(record.Key, originalPackage, package);
                    }

                    this.AssertTagEqual(SignatureTag.RPMTAG_HEADERSIGNATURES, originalPackage, package);
                }
            }
        }

        /// <summary>
        /// Tests the <see cref="RpmPackageCreator.CreatePackage(Stream, string, string, string, string, Action{RpmMetadata}, PgpPrivateKey, Stream)"/>
        /// </summary>
        [Fact]
        public void CreatePackageTest()
        {
            var krgen = PgpSigner.GenerateKeyRingGenerator("dotnet", "dotnet");
            var secretKeyRing = krgen.GenerateSecretKeyRing();
            var privateKey = secretKeyRing.GetSecretKey().ExtractPrivateKey("dotnet".ToCharArray());
            var publicKey = secretKeyRing.GetPublicKey();
            const string preInstScript = "echo preinst\n";
            const string postInstScript = "echo postinst\n";
            const string preRemoveScript = "echo preremove\n";
            const string postRemoveScript = "echo postremove\n";
            const string nameString = "libplist";
            const string versionString = "2.0.1.151";
            const string releaseString = "1.1";
            const string archString = "x86_64";

            using (Stream stream = File.OpenRead($"Rpm/{nameString}-{versionString}-{releaseString}.{archString}.rpm"))
            using (var targetStream = File.Open(@"RpmPackageCreatorTests_CreateTest.rpm", FileMode.Create, FileAccess.ReadWrite, FileShare.None))
            {
                var originalPackage = RpmPackageReader.Read(stream);
                List<ArchiveEntry> archive = null;

                using (var decompressedPayloadStream = RpmPayloadReader.GetDecompressedPayloadStream(originalPackage))
                using (CpioFile cpio = new CpioFile(decompressedPayloadStream, leaveOpen: false))
                {
                    ArchiveBuilder builder = new ArchiveBuilder();
                    archive = builder.FromCpio(cpio);
                }

                using (var decompressedPayloadStream = RpmPayloadReader.GetDecompressedPayloadStream(originalPackage))
                using (var payloadStream = new MemoryStream())
                {
                    decompressedPayloadStream.CopyTo(payloadStream);
                    payloadStream.Position = 0;

                    RpmPackageCreator creator = new RpmPackageCreator(new PlistFileAnalyzer());
                    creator.CreatePackage(
                        archive,
                        payloadStream,
                        nameString,
                        versionString,
                        archString,
                        releaseString,
                        false,
                        null,
                        false,
                        null,
                        "obs://build.opensuse.org/home:qmfrederik",
                        "libplist is a library for manipulating Apple Binary and XML Property Lists",
                        "http://www.libimobiledevice.org/",
                        null,
                        preInstScript,
                        postInstScript,
                        preRemoveScript,
                        postRemoveScript,
                        null,
                        (metadata) => PlistMetadata.ApplyDefaultMetadata(metadata),
                        privateKey,
                        targetStream);
                }
            }

            using (var targetStream = File.Open(@"RpmPackageCreatorTests_CreateTest.rpm", FileMode.Open, FileAccess.Read, FileShare.None))
            {
                var package = RpmPackageReader.Read(targetStream);

                var metadata = new RpmMetadata(package);
                Assert.Equal(metadata.Version, versionString);
                Assert.Equal(metadata.Name, nameString);
                Assert.Equal(metadata.Arch, archString);
                Assert.Equal(metadata.Release, releaseString);
                Assert.StartsWith(preInstScript, metadata.PreIn);
                Assert.StartsWith(postInstScript, metadata.PostIn);
                Assert.StartsWith(preRemoveScript, metadata.PreUn);
                Assert.StartsWith(postRemoveScript, metadata.PostUn);
                var signature = new RpmSignature(package);

                Assert.True(signature.Verify(publicKey));
            }
        }

        /// <summary>
        /// Tests the <see cref="RpmPackageCreator.CreatePackage(Stream, string, string, string, string, Action{RpmMetadata}, PgpPrivateKey, Stream)"/>
        /// method. Mocks the signature and performs binary comparison of the results.
        /// </summary>
        [Fact]
        public void CreatePackageBinaryTest()
        {
            var pgpSignatureData = File.ReadAllBytes("Rpm/RpmSigTag_Pgp.bin");
            var rsaSignatureData = File.ReadAllBytes("Rpm/RpmSigTag_Rsa.bin");

            var signer = new DummySigner();
            signer.Add("D72D1E8A9326431472A1E39E1B7916AD07CCC31B", rsaSignatureData);
            signer.Add("A0A33779FBBED565A15FA85BAFBD1473E00F1257", pgpSignatureData);
            signer.Add("5E24172F773FEB67FC0BF4831BB4E8A75B4554EF", pgpSignatureData);

            using (Stream stream = File.OpenRead(@"Rpm/libplist-2.0.1.151-1.1.x86_64.rpm"))
            using (var targetStream = File.Open(@"RpmPackageCreatorTests_CreateBinaryTest.rpm", FileMode.Create, FileAccess.ReadWrite, FileShare.None))
            {
                var originalPackage = RpmPackageReader.Read(stream);
                List<ArchiveEntry> archive = null;

                using (var decompressedPayloadStream = RpmPayloadReader.GetDecompressedPayloadStream(originalPackage))
                using (CpioFile cpio = new CpioFile(decompressedPayloadStream, leaveOpen: false))
                {
                    ArchiveBuilder builder = new ArchiveBuilder();
                    archive = builder.FromCpio(cpio);
                }

                using (var compressedPayloadStream = RpmPayloadReader.GetCompressedPayloadStream(originalPackage))
                {
                    RpmPackageCreator creator = new RpmPackageCreator(new PlistFileAnalyzer());
                    creator.CreatePackage(
                        archive,
                        compressedPayloadStream,
                        "libplist",
                        "2.0.1.151",
                        "x86_64",
                        "1.1",
                        false,
                        null,
                        false,
                        null,
                        "obs://build.opensuse.org/home:qmfrederik",
                        "libplist is a library for manipulating Apple Binary and XML Property Lists",
                        "http://www.libimobiledevice.org/",
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        (metadata) => PlistMetadata.ApplyDefaultMetadata(metadata),
                        signer,
                        targetStream,
                        includeVersionInName: true,
                        payloadIsCompressed: true);
                }
            }

            using (var originalStream = File.OpenRead(@"Rpm/libplist-2.0.1.151-1.1.x86_64.rpm"))
            using (var targetStream = File.Open(@"RpmPackageCreatorTests_CreateBinaryTest.rpm", FileMode.Open, FileAccess.Read, FileShare.None))
            {
                var originalPackage = RpmPackageReader.Read(originalStream);
                var package = RpmPackageReader.Read(targetStream);

                RpmDumper.Dump(originalPackage, "RpmPackageCreatorTests_CreateBinaryTest_original.txt");
                RpmDumper.Dump(package, "RpmPackageCreatorTests_CreateBinaryTest_reconstructed.txt");

                var metadata = new RpmMetadata(package);
                var signature = new RpmSignature(package);

                foreach (var record in originalPackage.Signature.Records)
                {
                    this.AssertTagEqual(record.Key, originalPackage, package);
                }

                originalStream.Position = 0;
                targetStream.Position = 0;

                int index = 0;
                byte[] originalBuffer = new byte[1024];
                byte[] targetBuffer = new byte[1024];

                while (originalStream.Position < originalStream.Length)
                {
                    originalStream.Read(originalBuffer, 0, originalBuffer.Length);
                    targetStream.Read(targetBuffer, 0, targetBuffer.Length);

                    Assert.Equal(originalBuffer, targetBuffer);

                    index += originalBuffer.Length;
                }
            }
        }

        private void AssertTagEqual(SignatureTag tag, RpmPackage originalPackage, RpmPackage package)
        {
            var originalRecord = originalPackage.Signature.Records[tag];
            var record = package.Signature.Records[tag];

            // Don't have the private key
            if (tag != SignatureTag.RPMSIGTAG_PGP && tag != SignatureTag.RPMSIGTAG_RSA && tag != SignatureTag.RPMTAG_HEADERSIGNATURES)
            {
                Assert.Equal(originalRecord.Value, record.Value);
            }

            Assert.Equal(originalRecord.Header.Count, record.Header.Count);
            Assert.Equal(originalRecord.Header.Tag, record.Header.Tag);
            Assert.Equal(originalRecord.Header.Type, record.Header.Type);
            Assert.Equal(originalRecord.Header.Offset, record.Header.Offset);
        }

        private void AssertTagOffsetEqual(IndexTag tag, RpmPackage originalPackage, RpmPackage package)
        {
            var originalRecord = originalPackage.Header.Records[tag];
            var record = package.Header.Records[tag];

            Assert.Equal(originalRecord.Header.Offset, record.Header.Offset);
        }
    }
}
