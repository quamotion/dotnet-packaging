using Packaging.Targets.Rpm;
using System.Collections.ObjectModel;
using System.IO;
using Xunit;

namespace Packaging.Targets.Tests.Rpm
{
    /// <summary>
    /// Tests the <see cref="RpmMetadata"/> class.
    /// </summary>
    public class RpmMetadataTests
    {
        /// <summary>
        /// Tests the various properties of the <see cref="RpmMetadata"/> class.
        /// </summary>
        [Fact]
        public void MetadataPropertiesTest()
        {

            using (Stream stream = File.OpenRead(@"Rpm\libplist-2.0.1.151-1.1.x86_64.rpm"))
            {
                var package = RpmPackageReader.Read(stream);
                var metadata = new RpmMetadata(package);
                var headerRecords = package.Header.Records;

                Assert.Equal(package, metadata.Package);

                Assert.Equal("x86_64", metadata.Arch);
                Assert.Equal("lamb11", metadata.BuildHost);
                Assert.Equal(0x58fa71fc, metadata.BuildTime);
                Assert.Equal("lamb11 1492808188", metadata.Cookie);
                Assert.Equal("libplist is a library for manipulating Apple Binary and XML Property Lists", metadata.Description);
                Assert.Equal("home:qmfrederik / CentOS_7", metadata.Distribution);
                Assert.Equal("obs://build.opensuse.org/home:qmfrederik/CentOS_7/adfeea138cd469466e6fa13a3c88fb8f-libplist", metadata.DistUrl);
                Assert.Equal(8, metadata.FileDigetsAlgo);
                Assert.Equal("System Environment/Libraries", metadata.Group);
                Assert.Equal(-976, metadata.ImmutableRegionSize);
                Assert.Equal("LGPLv2+", metadata.License);
                Assert.Equal(new Collection<string>() { "C" }, metadata.Locales);
                Assert.Equal("libplist", metadata.Name);
                Assert.Equal("-O2 -g -pipe -Wall -Wp,-D_FORTIFY_SOURCE=2 -fexceptions -fstack-protector-strong --param=ssp-buffer-size=4 -grecord-gcc-switches   -m64 -mtune=generic", metadata.OptFlags);
                Assert.Equal("linux", metadata.Os);
                Assert.Equal("xz", metadata.PayloadCompressor);
                Assert.Equal("2", metadata.PayloadFlags);
                Assert.Equal("cpio", metadata.PayloadFormat);
                Assert.Equal("x86_64-redhat-linux-gnu", metadata.Platform);
                Assert.Equal("/sbin/ldconfig", metadata.PostInProg);
                Assert.Equal("/sbin/ldconfig", metadata.PostUnProg);
                Assert.Equal("1.1", metadata.Release);
                Assert.Equal("4.11.3", metadata.RpmVersion);
                Assert.Equal(0x26e6d, metadata.Size);
                Assert.Equal(new byte[] { 0x45, 0xc0, 0x86, 0x80, 0x77, 0x4e, 0xf4, 0xc0, 0x37, 0xf1, 0x1e, 0xb1, 0xd3, 0x47, 0xf0, 0xbf }, metadata.SourcePkgId);
                Assert.Equal("libplist-2.0.1.151-1.1.src.rpm", metadata.SourceRpm);
                Assert.Equal("Library for manipulating Apple Binary and XML Property Lists", metadata.Summary);
                Assert.Equal("http://www.libimobiledevice.org/", metadata.Url);
                Assert.Equal("obs://build.opensuse.org/home:qmfrederik", metadata.Vendor);
                Assert.Equal("2.0.1.151", metadata.Version);
            }
        }
    }
}
