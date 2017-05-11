using Packaging.Targets.IO;
using Packaging.Targets.Rpm;
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
        [InlineData(@"Rpm\libplist-2.0.1.151-1.1.x86_64.rpm", "plist")]
        [InlineData(@"Rpm\dotnet_test-1.0-0.noarch.rpm", "dotnet")]
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
                    var files = creator.CreateFiles(cpio);

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
            using (Stream stream = File.OpenRead(@"Rpm\libplist-2.0.1.151-1.1.x86_64.rpm"))
            {
                var originalPackage = RpmPackageReader.Read(stream);

                using (var payloadStream = RpmPayloadReader.GetDecompressedPayloadStream(originalPackage))
                using (var cpio = new CpioFile(payloadStream, false))
                {
                    RpmPackageCreator creator = new RpmPackageCreator(new PlistFileAnalyzer());
                    var files = creator.CreateFiles(cpio);

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
                    creator.AddRpmDependencies(metadata);

                    PlistMetadata.ApplyDefaultMetadata(metadata);

                    metadata.Size = 0x26e6d;
                    metadata.ImmutableRegionSize = -976;

                    creator.CalculateHeaderOffsets(package);

                    foreach (var record in originalPackage.Header.Records)
                    {
                        if (record.Key == IndexTag.RPMTAG_HEADERIMMUTABLE)
                        {
                            continue;
                        }

                        AssertTagOffsetEqual(record.Key, originalPackage, package);
                    }

                    AssertTagOffsetEqual(IndexTag.RPMTAG_HEADERIMMUTABLE, originalPackage, package);
                }
            }
        }

        [Fact]
        public void CalculateSignatureTest()
        {
            using (Stream stream = File.OpenRead(@"Rpm\libplist-2.0.1.151-1.1.x86_64.rpm"))
            {
                var originalPackage = RpmPackageReader.Read(stream);

                RpmPackageCreator creator = new RpmPackageCreator(new PlistFileAnalyzer());
                Collection<RpmFile> files;

                using (var payloadStream = RpmPayloadReader.GetDecompressedPayloadStream(originalPackage))
                using (var cpio = new CpioFile(payloadStream, false))
                {
                    files = creator.CreateFiles(cpio);
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
                creator.AddRpmDependencies(metadata);

                PlistMetadata.ApplyDefaultMetadata(metadata);

                creator.CalculateHeaderOffsets(package);

                // Make sure the header is really correct

                using (Stream originalHeaderStream = new SubStream(
                    originalPackage.Stream,
                    originalPackage.HeaderOffset,
                    originalPackage.PayloadOffset - originalPackage.HeaderOffset,
                    leaveParentOpen: true, readOnly: true))
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

                        AssertTagEqual(record.Key, originalPackage, package);
                    }

                    AssertTagEqual(SignatureTag.RPMTAG_HEADERSIGNATURES, originalPackage, package);
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

            using (Stream stream = File.OpenRead(@"Rpm\libplist-2.0.1.151-1.1.x86_64.rpm"))
            using (var targetStream = File.Open(@"RpmPackageCreatorTests_CreateTest.rpm", FileMode.Create, FileAccess.ReadWrite, FileShare.None))
            {
                var originalPackage = RpmPackageReader.Read(stream);

                using (var payloadStream = RpmPayloadReader.GetDecompressedPayloadStream(originalPackage))
                {
                    RpmPackageCreator creator = new RpmPackageCreator(new PlistFileAnalyzer());
                    creator.CreatePackage(
                        payloadStream,
                        "libplist",
                        "2.0.1.151",
                        "x86_64",
                        "1.1",
                        (metadata) => PlistMetadata.ApplyDefaultMetadata(metadata),
                        privateKey,
                        targetStream);
                }
            }

            using (var targetStream = File.Open(@"RpmPackageCreatorTests_CreateTest.rpm", FileMode.Open, FileAccess.Read, FileShare.None))
            {
                var package = RpmPackageReader.Read(targetStream);

                var metadata = new RpmMetadata(package);
                var signature = new RpmSignature(package);

                Assert.True(signature.Verify(publicKey));
            }
        }

        [Fact]
        public void CreateDemoPackageTest()
        {
            // To install:
            //  sudo rpm -i dotnet-demo.rpm
            // To uninstall:
            //  sudo rpm -e dotnet-demo-0.1-0.1.x86_64
            var krgen = PgpSigner.GenerateKeyRingGenerator("dotnet", "dotnet");
            var secretKeyRing = krgen.GenerateSecretKeyRing();
            var privateKey = secretKeyRing.GetSecretKey().ExtractPrivateKey("dotnet".ToCharArray());
            var publicKey = secretKeyRing.GetPublicKey();

            using (var targetStream = File.Open(@"dotnet-demo.rpm", FileMode.Create, FileAccess.ReadWrite, FileShare.None))
            using (var cpioStream = File.Open("dotnet-demo.cpio", FileMode.Create, FileAccess.ReadWrite, FileShare.None))
            {
                // Create the CPIO file which will (hopefully) contain all the required files
                CpioFileCreator cpioCreator = new CpioFileCreator();
                cpioCreator.FromDirectory(
                    @"C:\Users\frede\Source\Repos\dotnet-packaging\demo\bin\Debug\netcoreapp1.0\rhel.7-x64\publish\",
                    "./usr/share/quamotion",
                    cpioStream);
                cpioStream.Position = 0;

                RpmPackageCreator rpmCreator = new RpmPackageCreator();
                rpmCreator.CreatePackage(
                    cpioStream,
                    "dotnet-demo",
                    "0.1",
                    "x86_64",
                    "0.1",
                    null,
                    privateKey,
                    targetStream);
            }

            using (var targetStream = File.Open(@"dotnet-demo.rpm", FileMode.Open, FileAccess.Read, FileShare.None))
            {
                var package = RpmPackageReader.Read(targetStream);

                var metadata = new RpmMetadata(package);
                var signature = new RpmSignature(package);

                Assert.True(signature.Verify(publicKey));
            }
        }

        private void AssertTagEqual(SignatureTag tag, RpmPackage originalPackage, RpmPackage package)
        {
            var originalRecord = originalPackage.Signature.Records[tag];
            var record = package.Signature.Records[tag];

            // Don't have the private key
            if (tag != SignatureTag.RPMSIGTAG_PGP && tag != SignatureTag.RPMSIGTAG_RSA)
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
