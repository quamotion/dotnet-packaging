using Packaging.Targets.Pkg;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Xunit;

namespace Packaging.Targets.Tests.Pkg
{
    public class XarFileTests
    {
        [Fact]
        public void TestEntryNames()
        {
            using (Stream stream = File.OpenRead("Pkg/Sample.pkg"))
            using (XarFile file = new XarFile(stream, leaveOpen: true))
            {
                // Check the top-level entries
                var entryNames = file.Entries.Select(e => e.Name).ToArray();
                string[] expectedEntryNames = new string[]
                {
                    "base.pkg",
                    "Resources",
                    "Distribution"
                };

                Assert.Equal(entryNames, expectedEntryNames);

                // Test an individual entry.
                var distribution = file.Entries.Single(e => e.Name == "Distribution");
                Assert.Equal(1, distribution.Id);
                Assert.Equal(1060u, distribution.DataLength);
                Assert.Equal("application/octet-stream", distribution.Encoding);
                Assert.Equal(20u, distribution.DataOffset);
                Assert.Equal(1060u, distribution.DataSize);
                Assert.Equal("6a595d627bd46d908ef3fd5013ffc670257f9ce5", distribution.ExtractedChecksum);
                Assert.Equal("sha1", distribution.ExtractedChecksumStyle);
                Assert.Equal("6a595d627bd46d908ef3fd5013ffc670257f9ce5", distribution.ArchivedChecksum);
                Assert.Equal("sha1", distribution.ArchivedChecksumStyle);

                Assert.Equal(281474976710656, distribution.FinderCreateTime.NanoSeconds);
                Assert.Equal(new DateTime(1970, 01, 01), distribution.FinderCreateTime.Time);

                Assert.Equal(new DateTime(2017, 10, 13, 15, 31, 02), distribution.CreateTime);
                Assert.Equal(new DateTime(2017, 10, 13, 15, 31, 02), distribution.ModifiedTime);
                Assert.Equal(new DateTime(2017, 10, 13, 15, 31, 02), distribution.ArchivedTime);

                Assert.Equal("staff", distribution.Group);
                Assert.Equal(20, distribution.GroupId);
                Assert.Equal("vagrant", distribution.User);
                Assert.Equal(502, distribution.UserId);
                Assert.Equal(0644, distribution.Mode);

                Assert.Equal(16777218, distribution.DeviceNumber);
                Assert.Equal(3004770, distribution.Inode);
                Assert.Equal("file", distribution.Type);
                Assert.Equal("Distribution", distribution.Name);
            }
        }

        [Fact]
        public void GetEntryTest()
        {
            using (Stream stream = File.OpenRead("Pkg/Sample.pkg"))
            using (XarFile file = new XarFile(stream, leaveOpen: true))
            {
                Assert.Throws<FileNotFoundException>(() => file.Open("invalid"));
                Assert.Throws<InvalidOperationException>(() => file.Open("base.pkg"));
                Assert.Throws<InvalidOperationException>(() => file.Open("/base.pkg"));
                Assert.Throws<InvalidOperationException>(() => file.Open("base.pkg/"));
                Assert.Throws<InvalidOperationException>(() => file.Open("/base.pkg/"));

                using (var bom = file.Open("base.pkg/Bom"))
                {
                    Assert.NotNull(bom);
                }

                using (var bom = file.Open("/base.pkg/Bom"))
                using (FileStream bomStream = File.Open("Bom", FileMode.Create, FileAccess.ReadWrite))
                {
                    Assert.NotNull(bom);
                    bom.CopyTo(bomStream);
                }

                using (var packageInfo = file.Open("/base.pkg/PackageInfo"))
                using (FileStream packageInfoStream = File.Open("PackageInfo", FileMode.Create, FileAccess.ReadWrite))
                {
                    Assert.NotNull(packageInfo);
                    packageInfo.CopyTo(packageInfoStream);
                }

                using (var payload = file.Open("/base.pkg/Payload"))
                using (FileStream payloadStream = File.Open("Payload.gz", FileMode.Create, FileAccess.ReadWrite))
                {
                    Assert.NotNull(payload);
                    payload.CopyTo(payloadStream);
                }

                using (var payload = file.Open("/base.pkg/Payload"))
                using (var gzipStream = new GZipStream(payload, CompressionMode.Decompress, leaveOpen: false))
                using (FileStream payloadStream = File.Open("Payload", FileMode.Create, FileAccess.ReadWrite))
                {
                    gzipStream.CopyTo(payloadStream);
                }

                using (var distribution = file.Open("/Distribution"))
                using (FileStream distributionStream = File.Open("Distribution", FileMode.Create, FileAccess.ReadWrite))
                {
                    Assert.NotNull(distribution);
                    distribution.CopyTo(distributionStream);
                }
            }
        }
    }
}
