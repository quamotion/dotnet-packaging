using Packaging.Targets.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Packaging.Targets.Tests.Deb
{
    /// <summary>
    /// Tests the <see cref="DebTask"/> class.
    /// </summary>
    public class DebTaskTests
    {
        /// <summary>
        /// Tests the <see cref="DebTask.GetPackageArchitecture(string)"/> method.
        /// </summary>
        /// <param name="runtimeIdentifier">
        /// The .NET runtime identifier.
        /// </param>
        /// <param name="packageAchitecture">
        /// The expected package architecture.
        /// </param>
        [InlineData(null, "all")]
        [InlineData("ubuntu.18.04", "all")]
        [InlineData("ubuntu.18.04-x86", "i386")]
        [InlineData("ubuntu.18.04-x64", "amd64")]
        [InlineData("ubuntu.18.04-arm", "armhf")]
        [InlineData("ubuntu.18.04-arm64", "arm64")]
        [InlineData("linux", "all")]
        [InlineData("linux-x86", "i386")]
        [InlineData("linux-x64", "amd64")]
        [InlineData("linux-arm", "armhf")]
        [InlineData("linux-arm64", "arm64")]
        [Theory]
        public void GetPackageArchitectureTest(string runtimeIdentifier, string packageAchitecture)
        {
            Assert.Equal(packageAchitecture, DebTask.GetPackageArchitecture(runtimeIdentifier));
        }

        [Fact]
        public void EnsureDirectoriesTest()
        {
            List<ArchiveEntry> archiveEntries = new List<ArchiveEntry>();
            archiveEntries.Add(
                new ArchiveEntry()
                {
                    Mode = LinuxFileMode.S_IROTH | LinuxFileMode.S_IRGRP | LinuxFileMode.S_IRUSR | LinuxFileMode.S_IFREG,
                    TargetPath = "./ConsoleApp1.deps.json",
                });

            DebTask.EnsureDirectories(archiveEntries, includeRoot: false);

            // This example contains one entry in the current directory, so no new directory entries should
            // have been created
            Assert.Single(archiveEntries);
        }

        [Fact]
        public void EnsureDirectoriesTest2()
        {
            List<ArchiveEntry> archiveEntries = new List<ArchiveEntry>();
            archiveEntries.Add(
                new ArchiveEntry()
                {
                    Mode = LinuxFileMode.S_IROTH | LinuxFileMode.S_IRGRP | LinuxFileMode.S_IRUSR | LinuxFileMode.S_IFREG,
                    TargetPath = "/usr/local/share/consoleapp/ConsoleApp1.deps.json",
                });

            DebTask.EnsureDirectories(archiveEntries, includeRoot: true);

            archiveEntries = archiveEntries
                .OrderBy(e => e.TargetPathWithFinalSlash, StringComparer.Ordinal)
                .ToList();

            // This example contains one entry in the current directory, so no new directory entries should
            // have been created
            Assert.Collection(
                archiveEntries,
                (e) => Assert.Equal("/", e.TargetPath),
                (e) => Assert.Equal("/usr", e.TargetPath),
                (e) => Assert.Equal("/usr/local", e.TargetPath),
                (e) => Assert.Equal("/usr/local/share", e.TargetPath),
                (e) => Assert.Equal("/usr/local/share/consoleapp", e.TargetPath),
                (e) => Assert.Equal("/usr/local/share/consoleapp/ConsoleApp1.deps.json", e.TargetPath));
        }

        [Fact]
        public void EnsureDirectoriesTest3()
        {
            List<ArchiveEntry> archiveEntries = new List<ArchiveEntry>();
            archiveEntries.Add(
                new ArchiveEntry()
                {
                    Mode = LinuxFileMode.S_IROTH | LinuxFileMode.S_IRGRP | LinuxFileMode.S_IRUSR | LinuxFileMode.S_IFREG,
                    TargetPath = "ConsoleApp1.deps.json",
                });

            DebTask.EnsureDirectories(archiveEntries, includeRoot: false);

            // This example contains one entry in the current directory, so no new directory entries should
            // have been created
            Assert.Single(archiveEntries);
        }

        [Fact]
        public void EnsureDirectoriesTest4()
        {
            List<ArchiveEntry> archiveEntries = new List<ArchiveEntry>();
            archiveEntries.Add(
                new ArchiveEntry()
                {
                    Mode = LinuxFileMode.S_IROTH | LinuxFileMode.S_IRGRP | LinuxFileMode.S_IRUSR | LinuxFileMode.S_IFREG,
                    TargetPath = "/etc/dotnet-packaging/LICENSE",
                });

            archiveEntries.Add(
                new ArchiveEntry()
                {
                    Mode = LinuxFileMode.S_IROTH | LinuxFileMode.S_IRGRP | LinuxFileMode.S_IRUSR | LinuxFileMode.S_IFDIR,
                    TargetPath = "/etc/dotnet-packaging",
                });

            DebTask.EnsureDirectories(archiveEntries, includeRoot: false);

            // This example contains an explicit entry (usually coming from LinuxFolder) for a folder for which a leaf
            // node also exists. Make sure the folder entry was created only once, and the permissions for that folder
            // are those which were set explicitly.
            Assert.Collection(
                archiveEntries,
                (e) => Assert.Equal("/etc/dotnet-packaging/LICENSE", e.TargetPath),
                (e) => 
                {   
                    Assert.Equal("/etc/dotnet-packaging", e.TargetPath);
                    Assert.Equal(LinuxFileMode.S_IROTH | LinuxFileMode.S_IRGRP | LinuxFileMode.S_IRUSR | LinuxFileMode.S_IFDIR, e.Mode);
                },
                (e) => Assert.Equal("/etc", e.TargetPath));
        }
    }
}
