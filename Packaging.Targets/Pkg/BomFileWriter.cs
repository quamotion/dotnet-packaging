using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Packaging.Targets.Pkg
{
    internal class BomFileWriter
    {
        private readonly Bom bom;
        private uint offset = 0;

        public BomFileWriter(Bom bom)
        {
            this.bom = bom;
        }

        public Dictionary<object, BomPointer> Pointers
        { get; } = new Dictionary<object, BomPointer>();

        public void AssignPointers()
        {
            this.AssignPointer(this.bom.Header, 0x200);
            this.AssignPointer(this.bom.Variables.Variables["VIndex"], (uint)Marshal.SizeOf<BomVIndex>());
            this.AssignPointer(new object(), 0xF); // Unknown
            this.AssignPointer(this.bom.Variables.Variables["HLIndex"], (uint)Marshal.SizeOf<BomTree>());
            this.AssignPointer(new object(), 0x5);
            this.AssignPointer(this.bom.Paths, 0x1000);
            this.AssignPointer(this.bom.Variables.Variables["Paths"], 0x15);
            this.AssignPointer(this.bom.VIndexPointer, (uint)Marshal.SizeOf<BomTree>());

            // Top-level directory
            this.AssignPointer(this.bom.Paths[0], this.bom.Paths[0].SerializedSize);
            this.AssignPointer(this.bom.Paths[0].PathInfoPointer, (uint)Marshal.SizeOf<BomPathInfo>());
            this.AssignPointer(new object(), 0x3); // Unknown

            this.AssignPointer(this.bom.HLIndex, 0x1000);
            this.AssignPointer(this.bom.Variables.Variables["Size64"], (uint)Marshal.SizeOf<BomTree>());

            this.AssignPointer(this.bom.Paths[1], this.bom.Paths[1].SerializedSize);
            this.AssignPointer(this.bom.Paths[1].PathInfoPointer, (uint)Marshal.SizeOf<BomPathInfo>());
            this.AssignPointer(new object(), 0x3); // Unknown
            this.AssignPointer(this.bom.VIndex, 0x80);

            // Determine the lenght of the variables
            // # of variables (integer) + for each avariable length of name + 0-terminated name
            var variablesLength =
                4 + (uint)this.bom.Variables.Variables.Sum(v => 4 + (uint)v.Key.Length + 1);
            this.AssignPointer(this.bom.Variables, variablesLength);

            this.AssignPointer(this.bom.Size64, 0x1000);
            this.AssignPointer(this.bom.Paths[0].PathInfo2Pointer, (uint)Marshal.SizeOf<BomPathInfo2>());
            this.AssignPointer(this.bom.Paths[1].PathInfo2Pointer, (uint)Marshal.SizeOf<BomPathInfo2>());

            this.AssignPointer(this.bom.Paths[2].PathInfo2Pointer, (uint)Marshal.SizeOf<BomPathInfo2>() + 4 /* Why? */);
            this.AssignPointer(this.bom.Paths[2], this.bom.Paths[2].SerializedSize);
            this.AssignPointer(this.bom.Paths[2].PathInfoPointer, (uint)Marshal.SizeOf<BomPathInfo>());

            this.AssignPointer(this.bom.BomInfo, this.bom.BomInfo.SerializedSize);
            this.AssignPointer(this.bom.Index, this.bom.Index.SerializedSize + this.bom.FreeList.SerializedSize + 0x10 /* Why? */);
        }

        private void AssignPointer(object value, uint size)
        {
            BomPointer pointer = new BomPointer
            {
                address = this.offset,
                length = size
            };

            this.Pointers.Add(value, pointer);

            this.offset += size;
        }
    }
}
