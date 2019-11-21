using Xunit;

namespace Packaging.Targets.Tests.Rpm
{
    /// <summary>
    /// Tests the <see cref="RpmTask"/> class.
    /// </summary>
    public class RpmTaskTests
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
        [InlineData(null, "any")]
        [InlineData("ubuntu.18.04", "any")]
        [InlineData("ubuntu.18.04-x86", "i386")]
        [InlineData("ubuntu.18.04-x64", "x86_64")]
        [InlineData("ubuntu.18.04-arm", "armhfp")]
        [InlineData("ubuntu.18.04-arm64", "aarch64")]
        [InlineData("linux", "any")]
        [InlineData("linux-x86", "i386")]
        [InlineData("linux-x64", "x86_64")]
        [InlineData("linux-arm", "armhfp")]
        [InlineData("linux-arm64", "aarch64")]
        [Theory]
        public void GetPackageArchitectureTest(string runtimeIdentifier, string packageAchitecture)
        {
            Assert.Equal(packageAchitecture, RpmTask.GetPackageArchitecture(runtimeIdentifier));
        }
    }
}
