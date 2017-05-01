using Packaging.Targets.Rpm;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using Xunit;

namespace Packaging.Targets.Tests.Rpm
{
    /// <summary>
    /// Tests the <see cref="RpmPackageWriter"/> class.
    /// </summary>
    public class RpmPackageWriterTests
    {
        /// <summary>
        /// Reads a <see cref="RpmPackage"/> from a stream, and then serializes that package back to another stream, and makes
        /// sure both streams are equivalent.
        /// </summary>
        [Fact]
        public void WriteRpmPackageTest()
        {
            using (Stream stream = File.OpenRead(@"Rpm\libplist-2.0.1.151-1.1.x86_64.rpm"))
            using (Stream expected = File.OpenRead(@"Rpm\libplist-2.0.1.151-1.1.x86_64.rpm"))
            using (Stream actual = new MemoryStream())
            using (Stream output = new ValidatingCompositeStream(null, actual, expected))
            {
                var package = RpmPackageReader.Read(stream);
                RpmPackageWriter.Write(output, package);
            }
        }
    }
}
