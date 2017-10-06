using Packaging.Targets.Deb;
using Packaging.Targets.IO;
using System;
using System.IO;
using System.Security.Cryptography;
using Xunit;

namespace Packaging.Targets.Tests.Deb
{
    /// <summary>
    /// Tests the <see cref="DebPackageReader"/> class.
    /// </summary>
    public class DebPackageReaderTests
    {
        /// <summary>
        /// Tests the <see cref="DebPackageReader.Read(Stream)"/> method using the libplist package as an example.
        /// </summary>
        [Fact]
        public void ReadDebPackageTest()
        {
            using (Stream stream = File.OpenRead("Deb/libplist3_1.12-3.1_amd64.deb"))
            {
                var package = DebPackageReader.Read(stream);

                Assert.Equal(new Version(2, 0), package.PackageFormatVersion);

                Assert.NotNull(package.ControlFile);
                Assert.Equal(13, package.ControlFile.Count);

                Assert.Equal("libplist3", package.ControlFile["Package"]);
                Assert.Equal("libplist", package.ControlFile["Source"]);
                Assert.Equal("1.12-3.1", package.ControlFile["Version"]);
                Assert.Equal("amd64", package.ControlFile["Architecture"]);

                stream.Seek(0, SeekOrigin.Begin);
                using (var payload = DebPackageReader.GetPayloadStream(stream))
                using (var tarFile = new TarFile(payload, leaveOpen: true))
                {
                    while (tarFile.Read())
                    {
                        var tarHeader = (TarHeader)tarFile.FileHeader;
                        Assert.Equal(tarHeader.Checksum, tarHeader.ComputeChecksum());
                        if (tarHeader.TypeFlag != TarTypeFlag.RegType)
                        {
                            tarFile.Skip();
                        }
                        else
                        {
                            var fname = tarFile.FileName;
                            Assert.StartsWith("./", fname);
                            fname = fname.Substring(2);
                            if (!package.Md5Sums.TryGetValue(fname, out var sum))
                            {
                                throw new Exception($"Checksum for {fname} not found");
                            }

                            string hash;
                            using (var fileStream = tarFile.Open())
                            using (var md5 = MD5.Create())
                                hash = BitConverter.ToString(md5.ComputeHash(fileStream)).Replace("-", string.Empty)
                                    .ToLower();
                            Assert.Equal(sum, hash);
                        }
                    }
                }
            }
        }
    }
}
