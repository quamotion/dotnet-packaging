using Mono.Unix;
using System.IO;
using Xunit;

namespace cliscd
{
    public class Tests
    {
        [Fact]
        public void UserExists()
        {
            var user = new UnixUserInfo("quamotion");
            Assert.NotEqual(0, user.UserId);
            Assert.NotEqual(0, user.GroupId);
            Assert.Equal("quamotion", user.GroupName);
            Assert.Equal("/home/quamotion", user.HomeDirectory);
        }

        [Fact]
        public void MachineFileExists()
        {
            Assert.True(File.Exists("/etc/quamotion/cliscd.machine.config"));
        }

        [Fact]
        public void UserFileExists()
        {
            Assert.True(File.Exists("/home/quamotion/.cliscd/cliscd.user.config"));
        }

        [Fact]
        public void ReadmeFileExists()
        {
            Assert.True(File.Exists("/etc/dotnet-packaging/README.md"));
        }

        [Fact]
        public void LinuxFileModeIsCorrect_Directory()
        {
            var directoryInfo = new UnixDirectoryInfo("/etc/dotnet-packaging");
            Assert.Equal(0b111_111_000, (int)directoryInfo.FileAccessPermissions & 0b111_111_111);
        }

        [Fact]
        public void LinuxFileModeIsCorrect_File()
        {
            var fileInfo = new UnixFileInfo("/etc/dotnet-packaging/LICENSE");
            Assert.Equal(0b111_101_101, (int)fileInfo.FileAccessPermissions & 0b111_111_111);
        }

        [Theory]
        [InlineData("/var/log/quamotion")]
        [InlineData("/var/run/quamotion")]
        [InlineData("/var/lib/quamotion")]
        [InlineData("/etc/quamotion")]
        public void LinuxFolderExists(string path)
        {
            Assert.True(Directory.Exists(path));

            var directoryInfo = new UnixDirectoryInfo(path);
            Assert.Equal("quamotion", directoryInfo.OwnerGroup.GroupName);
            Assert.Equal("quamotion", directoryInfo.OwnerUser.UserName);
        }
    }
}
