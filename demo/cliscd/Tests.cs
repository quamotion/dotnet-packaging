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
