using Packaging.Targets.Rpm;
using System.IO;
using Xunit;

namespace Packaging.Targets.Tests.Rpm
{
    public class RpmPackageReaderTests
    {
        [Fact]
        public void ReadRpmPackageTest()
        {
            using (Stream stream = File.OpenRead(@"Rpm\libplist-2.0.1.151-1.1.x86_64.rpm"))
            {
                var package = RpmPackageReader.Read(stream);
            }
        }
    }
}
