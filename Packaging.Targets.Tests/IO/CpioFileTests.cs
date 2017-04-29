using Packaging.Targets.IO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Xunit;

namespace Packaging.Targets.Tests.IO
{
    /// <summary>
    /// Tests the <see cref="CpioFile"/> class.
    /// </summary>
    public class CpioFileTests
    {
        /// <summary>
        /// Tests the opening an reading of entries from a <see cref="CpioFile"/>
        /// object.
        /// </summary>
        [Fact]
        public void OpenCpioFileTests()
        {
            Collection<string> entryNames = new Collection<string>();
            Collection<CpioHeader> entryHeaders = new Collection<CpioHeader>();
            Collection<string> entryHashes = new Collection<string>();

            SHA256 sha = SHA256.Create();

            using (Stream stream = File.OpenRead(@"IO\test.cpio"))
            using (CpioFile file = new CpioFile(stream, true))
            {
                while (file.Read())
                {
                    entryNames.Add(file.EntryName);
                    entryHeaders.Add(file.EntryHeader);

                    using (Stream entry = file.Open())
                    {
                        entryHashes.Add(BitConverter.ToString(sha.ComputeHash(entry)).Replace("-", string.Empty).ToLower());
                    }
                }
            }

            Assert.Equal(3, entryHashes.Count);
            Assert.Equal(3, entryHeaders.Count);
            Assert.Equal(3, entryHashes.Count);

            Assert.Equal(".", entryNames[0]);
            Assert.Equal(0u, entryHeaders[0].Check);
            Assert.Equal(0xfcu, entryHeaders[0].DevMajor);
            Assert.Equal(0u, entryHeaders[0].DevMinor);
            Assert.Equal(0u, entryHeaders[0].FileSize);
            Assert.Equal(0u, entryHeaders[0].Gid);
            Assert.Equal(0x24fafu, entryHeaders[0].Ino);
            Assert.Equal(0x41edu, entryHeaders[0].Mode);
            Assert.Equal(0x471c8630u, entryHeaders[0].Mtime);
            Assert.Equal(0x02u, entryHeaders[0].NameSize);
            Assert.Equal(0x8u, entryHeaders[0].Nlink);
            Assert.Equal(0u, entryHeaders[0].RDevMajor);
            Assert.Equal(0u, entryHeaders[0].RDevMinor);
            Assert.Equal("070701", entryHeaders[0].Signature);
            Assert.Equal(0u, entryHeaders[0].Uid);

            Assert.Equal("e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855", entryHashes[0]);
            Assert.Equal("e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855", entryHashes[1]);
            Assert.Equal("811ee67ae433927d702c675c53dfe53811b1479aa9eedd91666b548a4797a7bc", entryHashes[2]);
        }

        /// <summary>
        /// Tests the saving of data to a <see cref="CpioFile"/> object, by copying one CPIO
        /// stream to another.
        /// </summary>
        [Fact]
        public void WriteCpioFileTests()
        {
            using (Stream stream = File.OpenRead(@"IO\test.cpio"))
            using (CpioFile source = new CpioFile(stream, true))
            using (MemoryStream output = new MemoryStream())
            using (Stream expectedOutput = File.OpenRead(@"IO\test.cpio"))
            using (Stream validatingOutput = new ValidatingCompositeStream(null, output, expectedOutput))
            using (CpioFile target = new CpioFile(validatingOutput, true))
            {
                int index = 0;

                while (source.Read())
                {
                    using (Stream file = source.Open())
                    {
                        target.Write(source.EntryHeader, source.EntryName, file);
                        index++;
                    }
                }

                target.WriteTrailer();
            }
        }
    }
}
