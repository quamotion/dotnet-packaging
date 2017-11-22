using System;
using System.Collections.Generic;
using System.IO;
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

        public void Write(Stream stream)
        {
            if (this.Pointers.Count == 0)
            {
                throw new InvalidOperationException();
            }

            byte[] whitespace = new byte[0x1000];

            // Write the header, and pad with whitepace to 0x200
            stream.WriteStruct(this.bom.Header);
            stream.Write(whitespace, 0, 0x200 - Marshal.SizeOf<BomHeader>());

            // Write the value of the VIndex variable
            stream.WriteStruct(
                new BomVIndex()
                {
                    indexToVTree = (uint)this.bom.Index.Pointers.IndexOf(this.Pointers[this.bom.VIndex.BomTree]),
                    unknown0 = 1,
                    unknown2 = 0,
                    unknown3 = 0,
                });

            // Not sure yet what this is
            byte[] data = new byte[] { 0x6e, 0x66, 0x6f, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00 };
            stream.Write(data, 0, data.Length);

            // Write the value of the HLindex variable
            var treeMagicArray = Encoding.ASCII.GetBytes("tree");
            Array.Reverse(treeMagicArray);
            var treeMagic = BitConverter.ToUInt32(treeMagicArray, 0);

            stream.WriteStruct(
                new BomTree()
                {
                    tree = treeMagic,
                    version = 1,
                    child = (uint)this.bom.Index.Pointers.IndexOf(this.Pointers[this.bom.HLIndex]),
                    blockSize = 0x1000,
                    pathCount = (uint)this.bom.HLIndex.Count
                });

            // Write the BomPaths tree

        }

        public void AssignPointers()
        {
            if (this.Pointers.Count > 0)
            {
                throw new InvalidOperationException();
            }

            this.AssignPointer(this.bom.Header, 0x200);
            this.AssignPointer(this.bom.Variables.Variables["VIndex"], (uint)Marshal.SizeOf<BomVIndex>());
            this.AssignPointer(new object(), 0xF); // Unknown
            this.AssignPointer(this.bom.Variables.Variables["HLIndex"], (uint)Marshal.SizeOf<BomTree>());
            this.AssignPointer(new object(), 0x5);
            this.AssignPointer(this.bom.Paths, 0x1000);
            this.AssignPointer(this.bom.Variables.Variables["Paths"], 0x15);
            this.AssignPointer(this.bom.VIndex.BomTree, (uint)Marshal.SizeOf<BomTree>());

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
