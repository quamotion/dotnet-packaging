using System.Runtime.InteropServices;
using Xunit;

namespace Packaging.Targets.Tests
{
    public class RuntimeIdentifierTests
    {
        /// <summary>
        /// Tests the <see cref="RuntimeIdentifiers.ParseRuntimeId(string, out string, out string, out Architecture?, out string)"/> method.
        /// </summary>
        /// <param name="rid">
        /// The runtime identifier to parse.
        /// </param>
        /// <param name="expectedOsName">
        /// The expected operating system name.
        /// </param>
        /// <param name="expectedVersion">
        /// The expected operating system version.
        /// </param>
        /// <param name="expectedArchitecture">
        /// The epxected processor achitecture.
        /// </param>
        /// <param name="expectedQualifiers">
        /// Any expected additional qualifiers.
        /// </param>
        [Theory]
        [InlineData(null, null, null, null, null)]
        [InlineData("any", "any", null, null, null)]
        [InlineData("win", "win", null, null, null)]
        [InlineData("win-x86", "win", null, Architecture.X86, null)]
        [InlineData("win-x64", "win", null, Architecture.X64, null)]
        [InlineData("win7", "win", "7", null, null)]
        [InlineData("win7-x86", "win", "7", Architecture.X86, null)]
        [InlineData("win7-x64", "win", "7", Architecture.X64, null)]
        [InlineData("win8-x86", "win", "8", Architecture.X86, null)]
        [InlineData("win8-x64", "win", "8", Architecture.X64, null)]
        [InlineData("win81-x86", "win", "81", Architecture.X86, null)]
        [InlineData("win81-x64", "win", "81", Architecture.X64, null)]
        [InlineData("win10-x86", "win", "10", Architecture.X86, null)]
        [InlineData("win10-x64", "win", "10", Architecture.X64, null)]
        [InlineData("alpine-x64", "alpine", null, Architecture.X64, null)]
        [InlineData("alpine.3.10-x64", "alpine", "3.10", Architecture.X64, null)]
        [InlineData("linux", "linux", null, null, null)]
        [InlineData("linux-x86", "linux", null, Architecture.X86, null)]
        [InlineData("linux-x64", "linux", null, Architecture.X64, null)]
        [InlineData("linux-arm", "linux", null, Architecture.Arm, null)]
        [InlineData("linux-arm64", "linux", null, Architecture.Arm64, null)]
        [InlineData("linux-armel", "linux", null, Architecture.Arm, null)]
        [InlineData("linux-musl", "linux-musl", null, null, null)]
        [InlineData("linux-musl-x64", "linux-musl", null, Architecture.X64, null)]
        [InlineData("ubuntu", "ubuntu", null, null, null)]
        [InlineData("ubuntu.18.04", "ubuntu", "18.04", null, null)]
        [InlineData("ubuntu.18.04-x86", "ubuntu", "18.04", Architecture.X86, null)]
        [InlineData("ubuntu.18.04-x64", "ubuntu", "18.04", Architecture.X64, null)]
        [InlineData("ubuntu.18.04-arm", "ubuntu", "18.04", Architecture.Arm, null)]
        [InlineData("ubuntu.18.04-arm64", "ubuntu", "18.04", Architecture.Arm64, null)]
        [InlineData("ubuntu.18.04-armel", "ubuntu", "18.04", Architecture.Arm, null)]
        [InlineData("win-aot", "win", null, null, "aot")]
        [InlineData("win-x86-aot", "win", null, Architecture.X86,"aot")]
        [InlineData("win-x64-aot", "win", null, Architecture.X64, "aot")]
        [InlineData("win-arm-aot", "win", null, Architecture.Arm, "aot")]
        [InlineData("win-arm64-aot", "win", null, Architecture.Arm64, "aot")]
        [InlineData("win7-x86-aot", "win", "7", Architecture.X86, "aot")]
        [InlineData("win7-x64-aot", "win", "7", Architecture.X64, "aot")]
        [InlineData("win7-arm-aot", "win", "7", Architecture.Arm, "aot")]
        [InlineData("win7-arm64-aot", "win", "7", Architecture.Arm64, "aot")]
        public void ParseTest(string rid, string expectedOsName, string expectedVersion, Architecture? expectedArchitecture, string expectedQualifiers)
        {
            RuntimeIdentifiers.ParseRuntimeId(rid, out string osName, out string version, out Architecture? architecture, out string qualifiers);

            Assert.Equal(expectedOsName, osName);
            Assert.Equal(expectedVersion, version);
            Assert.Equal(expectedArchitecture, architecture);
            Assert.Equal(expectedQualifiers, qualifiers);
        }
    }
}
