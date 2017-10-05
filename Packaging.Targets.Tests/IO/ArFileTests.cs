using Packaging.Targets.IO;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
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
            using (Stream stream = File.OpenRead(@"Deb/libplist3_1.12-3.1_amd64.deb"))
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

        ArHeader CloneHeader(ArHeader hdr)
        {
            return new ArHeader
            {
                EndChar = hdr.EndChar,
                FileMode = hdr.FileMode,
                FileName = hdr.FileName,
                FileSize = hdr.FileSize,
                GroupId = hdr.GroupId,
                OwnerId = hdr.OwnerId,
                LastModified = hdr.LastModified
            };
        }
        
        void AssertCompareClonedHeader(ArHeader original, ArHeader clone)
        {
            var ms = new MemoryStream();
            ms.WriteStruct(clone);
            ms.Seek(0, SeekOrigin.Begin);
            clone = ms.ReadStruct<ArHeader>();
            foreach (var f in typeof(ArHeader).GetFields(BindingFlags.Instance | BindingFlags.NonPublic |
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
        public void WriteTest()
        {
            using (Stream original = File.OpenRead("Deb/libplist3_1.12-3.1_amd64.deb"))
            using (Stream expected = File.OpenRead("Deb/libplist3_1.12-3.1_amd64.deb"))
            using (Stream actual = new MemoryStream())
            using (Stream output = new ValidatingCompositeStream(null, actual, expected))
            {
                ArFileCreator.WriteMagic(output);
                var input = new ArFile(original, true);
                while (input.Read())
                {
                    var header = (ArHeader) input.FileHeader;
                    var clone = CloneHeader(header);
                    AssertCompareClonedHeader(header, clone);
                    using(var data = input.Open())
                        ArFileCreator.WriteEntry(output, clone, data);
                }
            }
        }
    }
}
