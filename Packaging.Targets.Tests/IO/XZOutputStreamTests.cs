using Packaging.Targets.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace Packaging.Targets.Tests.IO
{
    public class XZOutputStreamTests
    {
        [Fact]
        public void CompressFileTests()
        {
            using (FileStream stream = File.Open(@"XZOutputStreamTests_CompressFileTests.cpio.xz", FileMode.Create, FileAccess.Write, FileShare.None))
            using (FileStream input = File.OpenRead("IO/test.cpio"))
            using (XZOutputStream compressedStream = new XZOutputStream(stream))
            {
                input.CopyTo(compressedStream);
            }
        }

        [Fact]
        public void CompressDecompressTextTest()
        {
            using (FileStream stream = File.Open(@"XZOutputStreamTests_CompressTextTest.txt.xz", FileMode.Create, FileAccess.Write, FileShare.None))
            using (XZOutputStream compressedStream = new XZOutputStream(stream))
            {
                byte[] data = Encoding.UTF8.GetBytes("Hello, World! I am your XZ compressed file stream. Did we decompress correctly?");

                compressedStream.Write(data, 0, data.Length);
            }

            using (FileStream stream = File.Open(@"XZOutputStreamTests_CompressTextTest.txt.xz", FileMode.Open, FileAccess.Read, FileShare.None))
            using (XZOutputStream decompressedStream = new XZOutputStream(stream))
            {
                byte[] data = new byte[1024];
                int read = decompressedStream.Read(data, 0, data.Length);
                string text = Encoding.UTF8.GetString(data, 0, read);

                Assert.Equal("Hello, World! I am your XZ compressed file stream. Did we decompress correctly?", text);
            }
        }
    }
}
