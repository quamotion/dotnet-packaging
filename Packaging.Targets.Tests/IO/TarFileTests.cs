using System;
using Packaging.Targets.IO;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
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

        TarHeader CloneHeader(TarHeader header)
        {

            var clone = new TarHeader
            {
                Checksum = header.Checksum,
                DevMajor = header.DevMajor,
                DevMinor = header.DevMinor,
                FileMode = header.FileMode,
                FileName = header.FileName,
                FileSize = header.FileSize,
                GroupId = header.GroupId,
                GroupName = header.GroupName,
                LastModified = header.LastModified,
                LinkName = header.LinkName,
                Magic = header.Magic,
                Prefix = header.Prefix,
                TypeFlag = header.TypeFlag,
                UserId = header.UserId,
                UserName = header.UserName,
                Version = header.Version
            };
            foreach (var f in typeof(TarHeader).GetFields(BindingFlags.Instance | BindingFlags.NonPublic |
                                                          BindingFlags.Public))
            {
                var attr = f.GetCustomAttribute<MarshalAsAttribute>();
                if (attr?.SizeConst > 0)
                {
                    var v = (byte[]) f.GetValue(clone);
                    if (v.Length > attr.SizeConst)
                        throw new Exception("Field " + f.Name + " size exceeded");
                }
            }
            return clone;
        }

        void AssertCompareClonedHeader(TarHeader original, TarHeader clone)
        {
            var ms = new MemoryStream();
            ms.WriteStruct(clone);
            ms.Seek(0, SeekOrigin.Begin);
            clone = ms.ReadStruct<TarHeader>();
            foreach (var f in typeof(TarHeader).GetFields(BindingFlags.Instance | BindingFlags.NonPublic |
                                                          BindingFlags.Public))
            {
                var orig = f.GetValue(original);
                var mod = f.GetValue(clone);
                if (mod is byte[] modchars)
                    Assert.True(modchars.SequenceEqual((byte[]) orig), $"Failed check for {f.Name}");
                else
                    Assert.True(orig.Equals(mod), $"Failed check for {f.Name}");
            }
        }
        
        [Fact]
        public void WriteTarFileTest()
        {
            using (Stream original = File.OpenRead(@"IO/test.tar"))
            using (Stream expected = File.OpenRead(@"IO/test.tar"))
            using (Stream actual = new MemoryStream())
            using (Stream output = new ValidatingCompositeStream(null, actual, expected))
            {
                var tar = new TarFile(original, true);
                while (tar.Read())
                {
                    Stream data = new MemoryStream();
                    if (tar.FileHeader.FileMode == LinuxFileMode.S_IFDIR)
                        tar.Skip();
                    else
                        data = tar.Open();
                    var clonedHeader = CloneHeader((TarHeader) tar.FileHeader);
                    AssertCompareClonedHeader((TarHeader) tar.FileHeader, clonedHeader);
                    using (data)
                        TarFileCreator.WriteEntry(output, CloneHeader(clonedHeader), data);
                }
                TarFileCreator.WriteTrailer(output);
            }
        }
    }
}
