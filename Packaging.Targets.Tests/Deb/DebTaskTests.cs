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
    }
}
