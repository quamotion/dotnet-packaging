using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Packaging.Targets.IO;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace Packaging.Targets.Tests
{
    /// <summary>
    /// Tests the <see cref="ArchiveBuilder"/> class.
    /// </summary>
    public class ArchiveBuilderTests
    {
        /// <summary>
        /// Tests the <see cref="ArchiveBuilder.FromDirectory(string, string, ITaskItem[])"/> method
        /// </summary>
        [Fact]
        public void FromDirectoryTest()
        {
            ArchiveBuilder builder = new ArchiveBuilder();
            var entries = builder.FromDirectory("archive", "/opt/demo", Array.Empty<ITaskItem>());

            Assert.Equal(2, entries.Count);

            var readme = entries[0];
            Assert.Equal("root", readme.Group);
            Assert.Equal(1L, readme.Inode);
            Assert.False(readme.IsAscii);
            Assert.Equal(string.Empty, readme.LinkTo);
            Assert.Equal(LinuxFileMode.S_IROTH |  LinuxFileMode.S_IRGRP |  LinuxFileMode.S_IRUSR | LinuxFileMode.S_IFREG, readme.Mode);
            Assert.Equal("root", readme.Owner);
            Assert.False(readme.RemoveOnUninstall);
            Assert.Equal(Path.Combine("archive", "README.md"), readme.SourceFilename);
            Assert.Equal("/opt/demo/README.md", readme.TargetPath);
            Assert.Equal("/opt/demo/README.md", readme.TargetPathWithFinalSlash);
            Assert.Equal(ArchiveEntryType.None, readme.Type);

            var script = entries[1];
            Assert.Equal("root", script.Group);
            Assert.Equal(2L, script.Inode);
            Assert.False(script.IsAscii);
            Assert.Equal(string.Empty, script.LinkTo);
            Assert.Equal(LinuxFileMode.S_IROTH | LinuxFileMode.S_IRGRP | LinuxFileMode.S_IRUSR | LinuxFileMode.S_IFREG, script.Mode);
            Assert.Equal("root", script.Owner);
            Assert.False(script.RemoveOnUninstall);
            Assert.Equal(Path.Combine("archive", "script.sh"), script.SourceFilename);
            Assert.Equal("/opt/demo/script.sh", script.TargetPath);
            Assert.Equal("/opt/demo/script.sh", script.TargetPathWithFinalSlash);
            Assert.Equal(ArchiveEntryType.None, script.Type);
        }

        /// <summary>
        /// Tests the <see cref="ArchiveBuilder.FromDirectory(string, string, ITaskItem[])"/> method
        /// </summary>
        [Fact]
        public void FromDirectoryWithMetadataTest()
        {
            ArchiveBuilder builder = new ArchiveBuilder();

            Dictionary<string, string> metadata = new Dictionary<string, string>()
            {
                { "CopyToPublishDirectory", "Always" },
                { "LinuxPath", "/bin/script.sh" },
                { "LinuxFileMode", "755" },
            };

            var taskItem = new TaskItem("script.sh", metadata);
            var taskItems = new ITaskItem[] { taskItem };
            var entries = builder.FromDirectory("archive", "/opt/demo", taskItems);

            Assert.Equal(2, entries.Count);

            var readme = entries[0];
            Assert.Equal("root", readme.Group);
            Assert.Equal(1L, readme.Inode);
            Assert.False(readme.IsAscii);
            Assert.Equal(string.Empty, readme.LinkTo);
            Assert.Equal(LinuxFileMode.S_IROTH | LinuxFileMode.S_IRGRP | LinuxFileMode.S_IRUSR | LinuxFileMode.S_IFREG, readme.Mode);
            Assert.Equal("root", readme.Owner);
            Assert.False(readme.RemoveOnUninstall);
            Assert.Equal(Path.Combine("archive", "README.md"), readme.SourceFilename);
            Assert.Equal("/opt/demo/README.md", readme.TargetPath);
            Assert.Equal("/opt/demo/README.md", readme.TargetPathWithFinalSlash);
            Assert.Equal(ArchiveEntryType.None, readme.Type);

            var script = entries[1];
            Assert.Equal("root", script.Group);
            Assert.Equal(2L, script.Inode);
            Assert.False(script.IsAscii);
            Assert.Equal(string.Empty, script.LinkTo);

            // -rwxr-xr-x
            Assert.Equal(LinuxFileMode.S_IXOTH | LinuxFileMode.S_IROTH | LinuxFileMode.S_IXGRP | LinuxFileMode.S_IRGRP | LinuxFileMode.S_IXUSR | LinuxFileMode.S_IWUSR | LinuxFileMode.S_IRUSR, script.Mode);
            Assert.Equal("root", script.Owner);
            Assert.False(script.RemoveOnUninstall);
            Assert.Equal(Path.Combine("archive", "script.sh"), script.SourceFilename);
            Assert.Equal("/bin/script.sh", script.TargetPath);
            Assert.Equal("/bin/script.sh", script.TargetPathWithFinalSlash);
            Assert.Equal(ArchiveEntryType.None, script.Type);
        }
    }
}
