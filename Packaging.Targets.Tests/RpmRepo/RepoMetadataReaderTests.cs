using Packaging.Targets.RpmRepo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace Packaging.Targets.Tests.RpmRepo
{
    public class RepoMetadataReaderTests
    {
        [Fact]
        public void ReadMetadataTest()
        {
            using (Stream stream = FileMetadata.OpenRead("RpmRepo/repo/repodata/repomd.xml"))
            {
                RepoMetadataReader reader = new RepoMetadataReader();
                var metadata = reader.ReadMetadata(stream);

                Assert.Equal(1510178649, metadata.Revision);
                Assert.Equal(6, metadata.Data.Count);

                var file = metadata.Data[0];
                Assert.Equal("filelists", file.Type);
                Assert.Equal("sha256", file.ChecksumType);
                Assert.Equal("536af944b68b546dc62f401e5739774fbe2c913034cf3a1bc8db669369e63de9", file.Checksum);
                Assert.Equal("sha256", file.OpenChecksumType);
                Assert.Equal("2cc8a5377008354b6ae5aa5f9c05a534f0b0481f82922d0378efa926230d8927", file.OpenChecksum);
                Assert.Equal(1510178650, file.Timestamp);
                Assert.Equal(421, file.Size);
                Assert.Equal(976, file.OpenSize);
            }
        }

        [Fact]
        public void ReadPrimaryTest()
        {
            using (Stream stream = FileMetadata.OpenRead("RpmRepo/repo/repodata/4a156a3946b1f1b952c49dfcaf60942182b46ccfd2187db737ce83d777cb9254-primary.xml"))
            {
                RepoMetadataReader reader = new RepoMetadataReader();
                var primary = reader.ReadPrimary(stream);

                Assert.Equal(2, primary.Packages.Count);

                var rpm = primary.Packages[0];
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
                Assert.Equal(70264, rpm.ArchiveSize);
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

                rpm = primary.Packages[1];

                Assert.Equal(4, rpm.Provides.Count);
                Assert.Equal("libplist", rpm.Provides[0].Name);
                Assert.Equal("EQ", rpm.Provides[0].Flags);
                Assert.Equal(0, rpm.Provides[0].Epoch);
                Assert.Equal("1.2", rpm.Provides[0].Version);
                Assert.Equal("1.el6", rpm.Provides[0].Release);

                Assert.Equal(2, rpm.Files.Count);
                Assert.Equal("/usr/bin/plutil", rpm.Files[0]);
            }
        }

        [Fact]
        public void ReadFileListsTest()
        {
            using (Stream stream = FileMetadata.OpenRead("RpmRepo/repo/repodata/536af944b68b546dc62f401e5739774fbe2c913034cf3a1bc8db669369e63de9-filelists.xml"))
            {
                RepoMetadataReader reader = new RepoMetadataReader();
                var filelists = reader.ReadFileLists(stream);

                Assert.Equal(2, filelists.Packages.Count);

                var package = filelists.Packages[0];
                Assert.Equal("0c318f3146cc6206d88746326d3ad86b5c53b358add9c80cabc8de5b47270e8b", package.PkgId);
                Assert.Equal("libplist", package.Name);
                Assert.Equal("src", package.Arch);
                Assert.Equal(0, package.VersionEpoch);
                Assert.Equal("1.2", package.Version);
                Assert.Equal("1.el6", package.Release);

                Assert.Equal(2, package.Files.Count);
                Assert.Equal("libplist-1.2.tar.bz2", package.Files[0]);
            }
        }

        [Fact]
        public void ReadOtherTest()
        {
            using (Stream stream = FileMetadata.OpenRead("RpmRepo/repo/repodata/836440dafa52834295281fcda40810262f5c01cfef374a0e57858372ab84d600-other.xml"))
            {
                RepoMetadataReader reader = new RepoMetadataReader();
                var other = reader.ReadOther(stream);
                Assert.Equal(2, other.Packages.Count);

                var package = other.Packages[0];
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
}
