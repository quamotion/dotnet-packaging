using Packaging.Targets.RpmRepo;
using Xunit;

namespace Packaging.Targets.Tests.RpmRepo
{
    public class MetadataHarvesterTests
    {
        [Fact]
        public void ExtractMetadataTest()
        {
            MetadataHarvester harvester = new MetadataHarvester();
            var metadata = harvester.Harvest("RpmRepo/repo/RPMS");

            Assert.Equal(2, metadata.Packages.Count);

            // Primary metadata
            var rpm = metadata.Packages[0];
            Assert.Equal("rpm", rpm.Type);
            Assert.Equal("src", rpm.Arch);
            Assert.Equal(0, rpm.VersionEpoch);
            Assert.Equal("1.2", rpm.Version);
            Assert.Equal("1.el6", rpm.Release);
            Assert.Equal("sha256", rpm.ChecksumType);
            Assert.True(rpm.ChecksumIsPkgId);
            Assert.Equal("0c318f3146cc6206d88746326d3ad86b5c53b358add9c80cabc8de5b47270e8b", rpm.Checksum);
            Assert.Equal("Library for manipulating Apple Binary and XML Property Lists", rpm.Summary);
            Assert.Equal("libplist is a library for manipulating Apple Binary and XML Property Lists", rpm.Description);
            Assert.Equal("CentOS BuildSystem <http://bugs.centos.org>", rpm.Packager);
            Assert.Equal("http://matt.colyer.name/projects/iphone-linux/", rpm.Url);
            Assert.Equal(1323184405, rpm.FileTime);
            Assert.Equal(1282480066, rpm.BuildTime);
            Assert.Equal(69327, rpm.PackageSize);
            Assert.Equal(69879, rpm.InstalledSize);
            //Assert.Equal(70264, rpm.ArchiveSize);
            Assert.Equal("RPMS/libplist-1.2-1.el6.src.rpm", rpm.Location);
            Assert.Equal("LGPLv2+", rpm.License);
            Assert.Equal("CentOS", rpm.Vendor);
            Assert.Equal("System Environment/Libraries", rpm.Group);
            Assert.Equal("c6b6.bsys.dev.centos.org", rpm.BuildHost);
            Assert.Equal(string.Empty, rpm.SourceRpm);
            Assert.Equal(1384, rpm.HeaderStart);
            Assert.Equal(3632, rpm.HeaderEnd);
            Assert.Equal(5, rpm.Requires.Count);
            Assert.Equal("cmake", rpm.Requires[0].Name);

            rpm = metadata.Packages[1];

            Assert.Equal(4, rpm.Provides.Count);
            Assert.Equal("libplist", rpm.Provides[0].Name);
            Assert.Equal("EQ", rpm.Provides[0].Flags);
            Assert.Equal(0, rpm.Provides[0].Epoch);
            Assert.Equal("1.2", rpm.Provides[0].Version);
            Assert.Equal("1.el6", rpm.Provides[0].Release);

            Assert.Equal(2, rpm.Files.Count);
            Assert.Equal("/usr/bin/plutil", rpm.Files[0]);

            // File list
            var package = metadata.Packages[0];
            Assert.Equal("0c318f3146cc6206d88746326d3ad86b5c53b358add9c80cabc8de5b47270e8b", package.PkgId);
            Assert.Equal("libplist", package.Name);
            Assert.Equal("src", package.Arch);
            Assert.Equal(0, package.VersionEpoch);
            Assert.Equal("1.2", package.Version);
            Assert.Equal("1.el6", package.Release);

            Assert.Equal(2, package.Files.Count);
            Assert.Equal("libplist-1.2.tar.bz2", package.Files[0]);

            // Other metadata
            package = metadata.Packages[0];
            Assert.Equal("0c318f3146cc6206d88746326d3ad86b5c53b358add9c80cabc8de5b47270e8b", package.PkgId);
            Assert.Equal("libplist", package.Name);
            Assert.Equal("src", package.Arch);
            Assert.Equal(0, package.VersionEpoch);
            Assert.Equal("1.2", package.Version);
            Assert.Equal("1.el6", package.Release);

            var changelog = package.ChangeLog[0];
            Assert.Equal("Peter Robinson <pbrobinson@gmail.com> 0.12.0-0.1", changelog.Author);
            Assert.Equal(1241956800, changelog.Date);
            Assert.Equal("- Initial package", changelog.Text);
        }
    }
}
