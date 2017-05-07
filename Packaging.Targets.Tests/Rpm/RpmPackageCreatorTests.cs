﻿using Packaging.Targets.IO;
using Packaging.Targets.Rpm;
using System.IO;
using System.Linq;
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
        [Fact]
        public void CreateFiles()
        {
            using (Stream stream = File.OpenRead(@"Rpm\libplist-2.0.1.151-1.1.x86_64.rpm"))
            {
                var originalPackage = RpmPackageReader.Read(stream);

                using (var payloadStream = RpmPayloadReader.GetDecompressedPayloadStream(originalPackage))
                using (var cpio = new CpioFile(payloadStream, false))
                {
                    RpmPackageCreator creator = new RpmPackageCreator(new PlistFileAnalyzer());
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

        private void AssertTagOffsetEqual(IndexTag tag, RpmPackage originalPackage, RpmPackage package)
        {
            var originalRecord = originalPackage.Header.Records[tag];
            var record = package.Header.Records[tag];

            Assert.Equal(originalRecord.Header.Offset, record.Header.Offset);
        }
    }
}