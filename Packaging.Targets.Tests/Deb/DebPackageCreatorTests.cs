using Packaging.Targets.Deb;
using Packaging.Targets.IO;
using System;
using System.Collections.Generic;
using Xunit;

namespace Packaging.Targets.Tests.Deb
{
    /// <summary>
    /// Tests the <see cref="DebPackageCreator"/> class.
    /// </summary>
    public class DebPackageCreatorTests
    {
        /// <summary>
        /// Tests the <see cref="DebPackageCreator.BuildDebPackage"/> method in a basic scenario.
        /// </summary>
        [Fact]
        public void BuildDebPackageTest()
        {
            List<ArchiveEntry> entries = new List<ArchiveEntry>()
            {
                new ArchiveEntry()
                {
                     TargetPath = "/usr/bin/demo",
                     FileSize = 1024,
                     Md5Hash = new byte[]{0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F}
                },

                new ArchiveEntry()
                {
                    TargetPath = "/opt/local/test",
                    FileSize = 4096,
                    Md5Hash = new byte[]{0x0F, 0x0E, 0x0D, 0x0C, 0x0B, 0x0A, 0x09, 0x08, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F}
                }
            };

            var pkg = DebPackageCreator.BuildDebPackage(
                entries,
                "demo",
                "Demo Package",
                "Demo User",
                "1.0.0",
                "x86-64",
                createUser: false,
                userName: null,
                installService: false,
                serviceName: null,
                prefix: "/opt/local",
                section: null,
                priority: null,
                homepage: null,
                preInstallScript: null,
                postInstallScript: null,
                preRemoveScript: null,
                postRemoveScript: null,
                additionalDependencies: null,
                additionalMetadata: null);

            Assert.NotNull(pkg.Md5Sums);
            Assert.Null(pkg.ControlExtras);
            Assert.NotNull(pkg.ControlFile);

            Assert.Equal(2, pkg.Md5Sums.Count);
            Assert.Equal(6, pkg.ControlFile.Count);

            Assert.Equal("000102030405060708090a0b0c0d0e0f", pkg.Md5Sums["usr/bin/demo"]);
            Assert.Equal("0f0e0d0c0b0a090808090a0b0c0d0e0f", pkg.Md5Sums["opt/local/test"]);

            Assert.Equal("demo", pkg.ControlFile["Package"]);
            Assert.Equal("1.0.0", pkg.ControlFile["Version"]);
            Assert.Equal("x86-64", pkg.ControlFile["Architecture"]);
            Assert.Equal("Demo User", pkg.ControlFile["Maintainer"]);
            Assert.Equal("Demo Package", pkg.ControlFile["Description"]);
            Assert.Equal("5", pkg.ControlFile["Installed-Size"]);

            Assert.Equal(new Version(2, 0), pkg.PackageFormatVersion);
            Assert.Null(pkg.PostInstallScript);
            Assert.Null(pkg.PostRemoveScript);
            Assert.Null(pkg.PreInstallScript);
            Assert.Null(pkg.PreRemoveScript);
        }

        /// <summary>
        /// Tests the <see cref="DebPackageCreator.BuildDebPackage"/> method where pre- and post-install scripts
        /// are being used
        /// </summary>
        [Fact]
        public void BuildDebPackageWithScriptsTest()
        {
            List<ArchiveEntry> entries = new List<ArchiveEntry>()
            {
                new ArchiveEntry()
                {
                     TargetPath = "/usr/bin/demo",
                     FileSize = 1024,
                     Md5Hash = new byte[]{0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F},
                     RemoveOnUninstall = true
                },

                new ArchiveEntry()
                {
                    TargetPath = "/opt/local/test",
                    FileSize = 4096,
                    Md5Hash = new byte[]{0x0F, 0x0E, 0x0D, 0x0C, 0x0B, 0x0A, 0x09, 0x08, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F},
                    RemoveOnUninstall = true
                }
            };

            var pkg = DebPackageCreator.BuildDebPackage(
                entries,
                "demo",
                "Demo Package",
                "Demo User",
                "1.0.0",
                "x86-64",
                createUser: true,
                userName: "demouser",
                installService: true,
                serviceName: "demoservice",
                prefix: "/opt/local",
                section: null,
                priority: null,
                homepage: null,
                preInstallScript: "echo 'Hello from pre install'",
                postInstallScript: "echo 'Hello from post install'",
                preRemoveScript: "echo 'Hello from pre remove'",
                postRemoveScript: "echo 'Hello from post remove'",
                additionalDependencies: null,
                additionalMetadata: null);

            Assert.Equal(@"/usr/sbin/groupadd -r demouser 2>/dev/null || :
/usr/sbin/useradd -g demouser -s /sbin/nologin -r demouser 2>/dev/null || :
echo 'Hello from pre install'
", pkg.PreInstallScript, ignoreLineEndingDifferences: true);
            Assert.Equal(@"systemctl daemon-reload
systemctl enable --now demoservice.service
echo 'Hello from post install'
", pkg.PostInstallScript, ignoreLineEndingDifferences: true);
            Assert.Equal(@"systemctl --no-reload disable --now demoservice.service
echo 'Hello from pre remove'
", pkg.PreRemoveScript, ignoreLineEndingDifferences: true);
            Assert.Equal(@"/bin/rm -rf /usr/bin/demo
/bin/rm -rf /opt/local/test
echo 'Hello from post remove'
", pkg.PostRemoveScript, ignoreLineEndingDifferences: true);
        }

        /// <summary>
        /// Tests the <see cref="DebPackageCreator.BuildDebPackage"/> method where dependencies are being used
        /// </summary>
        [Fact]
        public void BuildDebPackageWithDependenciesTest()
        {
            List<ArchiveEntry> entries = new List<ArchiveEntry>();
            var dependencies = new string[]
            {
                "libc6",
                "libcurl3 | libcurl4",
                "libgcc1",
                "libgssapi-krb5-2",
                "liblttng-ust0",
                "libssl0.9.8 | libssl1.0.0 | libssl1.0.1 | libssl1.0.2",
                "libstdc++6",
                "libunwind8",
                "libuuid1",
                "zlib1g",
                "libicu52 | libicu53 | libicu54 | libicu55 | libicu56 | libicu57 | libicu58 | libicu59"
            };

            var pkg = DebPackageCreator.BuildDebPackage(
                entries,
                "demo",
                "Demo Package",
                "Demo User",
                "1.0.0",
                "x86-64",
                createUser: false,
                userName: null,
                installService: false,
                serviceName: null,
                prefix: "/opt/local",
                section: null,
                priority: null,
                homepage: null,
                preInstallScript: null,
                postInstallScript: null,
                preRemoveScript: null,
                postRemoveScript: null,
                additionalDependencies: dependencies,
                additionalMetadata: null);

            Assert.NotNull(pkg.ControlFile);
            Assert.Equal("libc6, libcurl3 | libcurl4, libgcc1, libgssapi-krb5-2, liblttng-ust0, libssl0.9.8 | libssl1.0.0 | libssl1.0.1 | libssl1.0.2, libstdc++6, libunwind8, libuuid1, zlib1g, libicu52 | libicu53 | libicu54 | libicu55 | libicu56 | libicu57 | libicu58 | libicu59", pkg.ControlFile["Depends"]);
        }
    }
}
