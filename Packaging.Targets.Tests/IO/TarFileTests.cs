using Packaging.Targets.IO;
using System.Collections.ObjectModel;
using System.IO;
using Xunit;

namespace Packaging.Targets.Tests.IO
{
    /// <summary>
    /// Tests the <see cref="TarFile"/> class.
    /// </summary>
    public class TarFileTests
    {
        /// <summary>
        /// Reads the contents of a <c>.tar</c> file.
        /// </summary>
        [Fact]
        public void ReadTarFileTest()
        {
            using (Stream stream = File.OpenRead("Deb/libplist3_1.12-3.1_amd64.deb"))
            using (ArFile arFile = new ArFile(stream, leaveOpen: true))
            {
                // Skip the debian version
                arFile.Read();

                // This is the tar file
                arFile.Read();

                Collection<string> filenames = new Collection<string>();
                Collection<string> contents = new Collection<string>();
                Collection<TarHeader> headers = new Collection<TarHeader>();

                using (Stream entryStream = arFile.Open())
                using (GZipDecompressor decompressedStream = new GZipDecompressor(entryStream, leaveOpen: true))
                using (TarFile tarFile = new TarFile(decompressedStream, leaveOpen: true))
                {
                    while (tarFile.Read())
                    {
                        filenames.Add(tarFile.FileName);
                        headers.Add((TarHeader)tarFile.FileHeader);

                        using (Stream data = tarFile.Open())
                        using (StreamReader reader = new StreamReader(data))
                        {
                            contents.Add(reader.ReadToEnd());
                        }
                    }
                }
            }
        }
    }
}
