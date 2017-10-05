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
            using (Stream stream = File.OpenRead(@"Rpm/libplist-2.0.1.151-1.1.x86_64.rpm"))
            {
                var package = RpmPackageReader.Read(stream);
                var names = RpmPayloadReader.Read(package);
            }
        }

        [Fact]
        public void ReadRpmPackageTest2()
        {
            using (Stream stream = File.OpenRead(@"Rpm/tomcat-8.0.44-1.fc27.noarch.rpm"))
            {
                var package = RpmPackageReader.Read(stream);
                var records = package.Header.Records;
                var names = RpmPayloadReader.Read(package);
            }
        }

        [Fact]
        public void ReadRpmPackageTest3()
        {
            using (Stream stream = File.OpenRead(@"Rpm/usbmuxd-1.1.0.95-11.11.x86_64.rpm"))
            {
                var package = RpmPackageReader.Read(stream);
                var records = package.Header.Records;
                var names = RpmPayloadReader.Read(package);
            }
        }
    }
}
