using Packaging.Targets.Deb;
using System;
using System.IO;
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
            }
        }
    }
}
