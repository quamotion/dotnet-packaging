using System.Collections.Generic;
using Microsoft.Build.Framework;
using Moq;
using Xunit;

namespace Packaging.Targets.Tests
{
    public class TaskItemExtensionsTests
    {
        [Theory]
        [InlineData(null, false)]
        [InlineData("DoNotCopy", false)]
        [InlineData("Always", true)]
        [InlineData("PreserveNewest", true)]
        [InlineData("Invalid", false)]
        public void IsPublishedTest(string copyToPublishDirectory, bool shouldPublish)
        {
            var taskItemMock = new Mock<ITaskItem>();
            Dictionary<string, string> metadata = new Dictionary<string, string>();

            if (copyToPublishDirectory != null)
            {
                metadata.Add("CopyToPublishDirectory", copyToPublishDirectory);
            }

            taskItemMock.Setup(m => m.MetadataNames).Returns(metadata.Keys);
            taskItemMock.Setup(m => m.GetMetadata("CopyToPublishDirectory")).Returns(() => metadata["CopyToPublishDirectory"]);

            Assert.Equal(shouldPublish, taskItemMock.Object.IsPublished());
        }

        [Theory]
        [InlineData("", "", "README", "", "README")]
        [InlineData("", "", "README", ".md", "README.md")]
        [InlineData("", "dir\\subdir", "README", ".md", "dir/subdir/README.md")]
        [InlineData("somelink", "dir\\subdir", "README", ".md", "somelink")]
        public void GetPublishedPathTest(string link, string relativeDir, string fileName, string extension, string expectedPath)
        {
            var taskItemMock = new Mock<ITaskItem>();
            Dictionary<string, string> metadata = new Dictionary<string, string>();

            metadata.Add("Link", link);
            metadata.Add("RelativeDir", relativeDir);
            metadata.Add("FileName", fileName);
            metadata.Add("Extension", extension);

            taskItemMock.Setup(m => m.MetadataNames).Returns(metadata.Keys);
            taskItemMock
                .Setup(m => m.GetMetadata(It.IsAny<string>()))
                .Returns<string>((k) => metadata[k]);

            Assert.Equal(expectedPath, taskItemMock.Object.GetPublishedPath());
        }
    }
}