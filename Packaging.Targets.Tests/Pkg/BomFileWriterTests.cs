using Packaging.Targets.Pkg;
using System.IO;
using Xunit;

namespace Packaging.Targets.Tests.Pkg
{
    public class BomFileWriterTests
    {
        [Fact]
        public void AssignPointersTest()
        {
            using (Stream stream = File.OpenRead("Pkg/Bom"))
            {
                Bom bom = new Bom(stream);

                BomFileWriter writer = new BomFileWriter(bom);
                writer.AssignPointers();

                // See BomFiles.md
                Assert.Equal(new BomPointer(0x0000, 0x0200), writer.Pointers[bom.Header]);
                Assert.Equal(new BomPointer(0x0200, 0x000D), writer.Pointers[bom.Variables.Variables["VIndex"]]);
                Assert.Equal(new BomPointer(0x021C, 0x0015), writer.Pointers[bom.Variables.Variables["HLIndex"]]);
                Assert.Equal(new BomPointer(0x0236, 0x1000), writer.Pointers[bom.Paths]);
                Assert.Equal(new BomPointer(0x1236, 0x0015), writer.Pointers[bom.Variables.Variables["Paths"]]);
                Assert.Equal(new BomPointer(0x124B, 0x0015), writer.Pointers[bom.VIndexPointer]);
                Assert.Equal(new BomPointer(0x1260, 0x0006), writer.Pointers[bom.Paths[0]]);
                Assert.Equal(new BomPointer(0x1266, 0x0008), writer.Pointers[bom.Paths[0].PathInfoPointer]);
                Assert.Equal(new BomPointer(0x1271, 0x1000), writer.Pointers[bom.HLIndex]);
                Assert.Equal(new BomPointer(0x2271, 0x0015), writer.Pointers[bom.Variables.Variables["Size64"]]);
                Assert.Equal(new BomPointer(0x2286, 0x0011), writer.Pointers[bom.Paths[1]]);
                Assert.Equal(new BomPointer(0x2297, 0x0008), writer.Pointers[bom.Paths[1].PathInfoPointer]);
                Assert.Equal(new BomPointer(0x22A2, 0x0080), writer.Pointers[bom.VIndex]);
                Assert.Equal(new BomPointer(0x2322, 0x003C), writer.Pointers[bom.Variables]);
                Assert.Equal(new BomPointer(0x235E, 0x1000), writer.Pointers[bom.Size64]);
                Assert.Equal(new BomPointer(0x335E, 0x001F), writer.Pointers[bom.Paths[0].PathInfo2Pointer]);
                Assert.Equal(new BomPointer(0x337D, 0x001F), writer.Pointers[bom.Paths[1].PathInfo2Pointer]);
                Assert.Equal(new BomPointer(0x339C, 0x0023), writer.Pointers[bom.Paths[2].PathInfo2Pointer]);
                Assert.Equal(new BomPointer(0x33BF, 0x000E), writer.Pointers[bom.Paths[2]]);
                Assert.Equal(new BomPointer(0x33CD, 0x0008), writer.Pointers[bom.Paths[2].PathInfoPointer]);
                Assert.Equal(new BomPointer(0x33D5, 0x001C), writer.Pointers[bom.BomInfo]);
                Assert.Equal(new BomPointer(0x33F1, 0x5590), writer.Pointers[bom.Index]);
            }
        }
    }
}
