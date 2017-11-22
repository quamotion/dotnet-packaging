using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace Packaging.Targets.Pkg
{
    /// <summary>
    /// The Mac OS X Installer uses a file system "bill of materials" to determine which files to install,
    /// remove, or upgrade. A bill of materials, bom, contains all the files within a directory, along with
    /// some information about each file.File information includes: the file's UNIX permissions, its owner and
    /// group, its size, its time of last modification, and so on.Also included are a checksum of each file
    /// and information about hard links.
    /// </summary>
    internal class Bom
    {
        private Stream stream;

        /// <summary>
        /// Initializes a new instance of the <see cref="Bom"/> class.
        /// </summary>
        /// <param name="stream">
        /// A <see cref="Stream"/> which represents the bill of materials.
        /// </param>
        public Bom(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            this.stream = stream;

            // Read the header
            this.Header = this.stream.ReadStruct<BomHeader>();

            if (this.Header.magic != BomHeader.Magic || this.Header.version != 1)
            {
                throw new InvalidOperationException();
            }

            // Read the block table and the free pointer list
            this.stream.Position = this.Header.indexOffset;
            this.Index = new BomPointerList();
            this.Index.NumberOfPointers = this.stream.ReadInt32BE();

            for (int i = 0; i < this.Index.NumberOfPointers; i++)
            {
                var blockPointer = this.stream.ReadStruct<BomPointer>();
                this.Index.Pointers.Add(blockPointer);
            }

            this.FreeList = new BomPointerList();
            this.FreeList.NumberOfPointers = this.stream.ReadInt32BE();

            for (int i = 0; i < this.FreeList.NumberOfPointers; i++)
            {
                var blockPointer = this.stream.ReadStruct<BomPointer>();
                this.FreeList.Pointers.Add(blockPointer);
            }

            // Read the variables
            this.stream.Position = this.Header.varsOffset;
            this.Variables = new BomVariableList();
            this.Variables.NumberOfVariables = (uint)this.stream.ReadInt32BE();

            for (int i = 0; i < this.Variables.NumberOfVariables; i++)
            {
                BomVariable variable = default(BomVariable);
                variable.index = (uint)this.stream.ReadInt32BE();
                variable.length = (byte)this.stream.ReadByte();

                byte[] nameBytes = new byte[variable.length];
                this.stream.Read(nameBytes, 0, variable.length);

                variable.name = Encoding.UTF8.GetString(nameBytes);
                this.Variables.Variables.Add(variable.name, variable);
            }

            foreach (var variable in this.Variables.Variables.Values)
            {
                var pointer = this.Index.Pointers[(int)variable.index];
                this.stream.Seek((int)pointer.address, SeekOrigin.Begin);

                switch (variable.name)
                {
                    case "Paths":
                        this.Paths.BomTree = this.stream.ReadStruct<BomTree>();
                        this.ReadPaths(
                            this.Paths);
                        break;

                    case "HLIndex":
                        this.HLIndex.BomTree = this.stream.ReadStruct<BomTree>();
                        this.ReadPaths(
                            this.HLIndex);
                        break;

                    case "Size64":
                        this.Size64.BomTree = this.stream.ReadStruct<BomTree>();
                        this.ReadPaths(
                            this.Size64);
                        break;

                    case "BomInfo":
                        this.BomInfo = new BomInfo();
                        this.BomInfo.Header = this.stream.ReadStruct<BomInfoHeader>();

                        for (int i = 0; i < this.BomInfo.Header.numberOfInfoEntries; i++)
                        {
                            this.BomInfo.Entries.Add(this.stream.ReadStruct<BomInfoEntry>());
                        }

                        break;

                    case "VIndex":
                        BomVIndex vIndex = this.stream.ReadStruct<BomVIndex>();

                        var vPtr = this.Index.Pointers[(int)vIndex.indexToVTree];
                        this.stream.Seek((int)vPtr.address, SeekOrigin.Begin);
                        this.VIndex.BomTree = this.stream.ReadStruct<BomTree>();
                        this.ReadPaths(this.VIndex);
                        break;

                    default:
                        break;
                }
            }
        }

        public BomPointerList Index
        { get; private set; }

        public BomPointerList FreeList
        { get; private set; }

        public BomHeader Header
        { get; private set; }

        public BomVariableList Variables
        { get; private set; }

        public BomInfo BomInfo
        { get; private set; }

        public BomFileTree Paths
        { get; } = new BomFileTree();

        public BomFileTree HLIndex
        { get; } = new BomFileTree();

        public BomFileTree Size64
        { get; } = new BomFileTree();

        public BomFileTree VIndex
        { get; } = new BomFileTree();

        private void ReadPaths(BomFileTree files)
        {
            var child = this.Index.Pointers[(int)files.BomTree.child];

            this.ReadPaths(child, files);
        }

        private void ReadPaths(BomPointer pointer, BomFileTree files)
        {
            this.stream.Seek(pointer.address, SeekOrigin.Begin);
            BomPaths paths = new BomPaths();
            paths.Header = this.stream.ReadStruct<BomPathsHeader>();

            for (int i = 0; i < paths.Header.count; i++)
            {
                var indices = this.stream.ReadStruct<BomPathIndices>();
                paths.Indices.Add(indices);
            }

            for (int i = 0; i < paths.Header.count; i++)
            {
                var filePointer = this.Index.Pointers[(int)paths.Indices[i].index1];
                BomFile file = new BomFile();

                this.stream.Seek(filePointer.address, SeekOrigin.Begin);
                file.parent = (uint)this.stream.ReadInt32BE();

                int nameLength = (int)filePointer.length - 4; // 4 = sizeof(parent);
                byte[] nameBytes = new byte[nameLength];
                this.stream.Read(nameBytes, 0, nameBytes.Length);

                file.name = Encoding.UTF8.GetString(nameBytes, 0, nameBytes.Length - 1);
                files.Add(file);

                var infoPointer = this.Index.Pointers[(int)paths.Indices[i].index0];
                this.stream.Seek(infoPointer.address, SeekOrigin.Begin);
                var info = this.stream.ReadStruct<BomPathInfo>();
                file.PathInfo = info;

                var info2Pointer = this.Index.Pointers[(int)info.index];
                this.stream.Seek(info2Pointer.address, SeekOrigin.Begin);

                var info2 = this.stream.ReadStruct<BomPathInfo2>();
                file.PathInfo2 = info2;
            }

            if (paths.Header.isLeaf == 0)
            {
                var childPtr = this.Index.Pointers[(int)paths.Indices[0].index0];
                this.ReadPaths(childPtr, files);
            }

            if (paths.Header.forward != 0)
            {
                var siblingPtr = this.Index.Pointers[(int)paths.Header.forward];
                this.ReadPaths(siblingPtr, files);
            }
        }
    }
}
