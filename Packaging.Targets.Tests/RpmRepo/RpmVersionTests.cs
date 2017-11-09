using Packaging.Targets.RpmRepo;
using Xunit;

namespace Packaging.Targets.Tests.RpmRepo
{
    /// <summary>
    /// Tests the <see cref="RpmVersion"/> class.
    /// </summary>
    public class RpmVersionTests
    {
        /// <summary>
        /// Parses a RPM version number without an epoch.
        /// </summary>
        [Fact]
        public void ParseNoEpochTest()
        {
            var version = RpmVersion.Parse("1.2-1.el6");
            Assert.Equal(0, version.Epoch);
            Assert.Equal("1.2", version.Version);
            Assert.Equal("1.el6", version.Release);
        }

        /// <summary>
        /// Parses a simple RPM version number.
        /// </summary>
        [Fact]
        public void ParseVersionOnlyTest()
        {
            var version = RpmVersion.Parse("1.2");
            Assert.Equal(0, version.Epoch);
            Assert.Equal("1.2", version.Version);
            Assert.Null(version.Release);
        }
    }
}
