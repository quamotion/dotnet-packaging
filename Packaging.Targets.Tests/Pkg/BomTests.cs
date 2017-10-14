using Packaging.Targets.Pkg;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace Packaging.Targets.Tests.Pkg
{
    public class BomTests
    {
        [Fact]
        public void OpenBomTest()
        {
            using (Stream stream = File.OpenRead("Pkg/Bom"))
            {
                Bom bom = new Bom(stream);

                Assert.NotNull(bom.Variables);

                Assert.NotNull(bom.BomInfo);
                Assert.Equal(1, bom.BomInfo.Entries.Count);

                Assert.NotNull(bom.HLIndex);
                Assert.Equal(0, bom.HLIndex.Count);

                Assert.NotNull(bom.Paths);
                Assert.Equal(3, bom.Paths.Count);
                Assert.Equal(".", bom.Paths[0].name);
                Assert.Equal(0u, bom.Paths[0].parent);
                Assert.Equal("Applications", bom.Paths[1].name);
                Assert.Equal(1u, bom.Paths[1].parent);
                Assert.Equal("hello.txt", bom.Paths[2].name);
                Assert.Equal(2u, bom.Paths[2].parent);

                Assert.NotNull(bom.Size64);
                Assert.Equal(0, bom.Size64.Count);

                Assert.NotNull(bom.VIndex);
                Assert.Equal(0, bom.VIndex.Count);
            }
        }
    }
}
