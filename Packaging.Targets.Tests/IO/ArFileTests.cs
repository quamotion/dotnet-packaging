using Packaging.Targets.IO;
using System.Collections.ObjectModel;
using System.IO;
using Xunit;

namespace Packaging.Targets.Tests.IO
{
    /// <summary>
    /// Tests the <see cref="ArFile"/> class.
    /// </summary>
    public class ArFileTests
    {
        /// <summary>
        /// Loops over all entries in an <see cref="ArFile"/>.
        /// </summary>
        [Fact]
        public void ReadTest()
        {
            using (Stream stream = File.OpenRead(@"Deb\libplist3_1.12-3.1_amd64.deb"))
            using (ArFile arFile = new ArFile(stream, leaveOpen: true))
            {
                Collection<string> filenames = new Collection<string>();
                Collection<ArHeader> headers = new Collection<ArHeader>();

                while (arFile.Read())
                {
                    filenames.Add(arFile.FileName);
                    headers.Add((ArHeader)arFile.FileHeader);

                    arFile.Skip();
                }

                Assert.Equal(
                    new string[]
                    {
                        "debian-binary",
                        "control.tar.gz",
                        "data.tar.xz"
                    },
                    filenames);
            }
        }
    }
}
