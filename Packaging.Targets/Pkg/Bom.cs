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
        private BomHeader header;
        private BomPointerList index;
        private BomPointerList freeList;

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
            this.header = this.stream.ReadStruct<BomHeader>();

            if (this.header.magic != BomHeader.Magic || this.header.version != 1)
            {
                throw new InvalidOperationException();
            }

            // Read the block table and the free pointer list
            this.stream.Position = this.header.indexOffset;
            this.index = new BomPointerList();
            this.index.NumberOfPointers = this.stream.ReadInt32BE();

            for (int i = 0; i < this.index.NumberOfPointers; i++)
            {
                var blockPointer = this.stream.ReadStruct<BomPointer>();
                this.index.Pointers.Add(blockPointer);
            }

            this.freeList = new BomPointerList();
            this.freeList.NumberOfPointers = this.stream.ReadInt32BE();

            for (int i = 0; i < this.freeList.NumberOfPointers; i++)
            {
                var blockPointer = this.stream.ReadStruct<BomPointer>();
                this.freeList.Pointers.Add(blockPointer);
            }

            // Read the variables
            this.stream.Position = this.header.varsOffset;
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
                this.Variables.Variables.Add(variable);
            }

            foreach (var variable in this.Variables.Variables)
            {
                var pointer = this.index.Pointers[(int)variable.index];
                this.stream.Seek((int)pointer.address, SeekOrigin.Begin);

                switch (variable.name)
                {
                    case "Paths":
                        this.ReadPaths(
                            this.stream.ReadStruct<BomTree>(),
                            this.Paths);
                        break;

                    case "HLIndex":
                        this.ReadPaths(
                            this.stream.ReadStruct<BomTree>(),
                            this.HLIndex);
                        break;

                    case "Size64":
                        this.ReadPaths(
                            this.stream.ReadStruct<BomTree>(),
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

                        var vPtr = this.index.Pointers[(int)vIndex.indexToVTree];
                        this.stream.Seek((int)vPtr.address, SeekOrigin.Begin);
                        var vTree = this.stream.ReadStruct<BomTree>();
                        this.ReadPaths(vTree, this.VIndex);
                        break;

                    default:
                        break;
                }
            }
        }

        public BomVariableList Variables
        { get; private set; }

        public BomInfo BomInfo
        { get; private set; }

        public Collection<BomFile> Paths
        { get; } = new Collection<BomFile>();

        public Collection<BomFile> HLIndex
        { get; } = new Collection<BomFile>();

        public Collection<BomFile> Size64
        { get; } = new Collection<BomFile>();

        public Collection<BomFile> VIndex
        { get; } = new Collection<BomFile>();

        private void ReadPaths(BomTree tree, Collection<BomFile> files)
        {
            var child = this.index.Pointers[(int)tree.child];

            this.ReadPaths(child, files);
        }

        private void ReadPaths(BomPointer pointer, Collection<BomFile> files)
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
                var filePointer = this.index.Pointers[(int)paths.Indices[i].index1];
                BomFile file = new BomFile();

                this.stream.Seek(filePointer.address, SeekOrigin.Begin);
                file.parent = (uint)this.stream.ReadInt32BE();

                int nameLength = (int)filePointer.length - 4; // 4 = sizeof(parent);
                byte[] nameBytes = new byte[nameLength];
                this.stream.Read(nameBytes, 0, nameBytes.Length);

                file.name = Encoding.UTF8.GetString(nameBytes, 0, nameBytes.Length - 1);
                files.Add(file);

                var infoPointer = this.index.Pointers[(int)paths.Indices[i].index0];
                this.stream.Seek(infoPointer.address, SeekOrigin.Begin);
                var info = this.stream.ReadStruct<BomPathInfo>();

                var info2Pointer = this.index.Pointers[(int)info.index];
                this.stream.Seek(info2Pointer.address, SeekOrigin.Begin);

                var info2 = this.stream.ReadStruct<BomPathInfo2>();
            }

            if (paths.Header.isLeaf == 0)
            {
                var childPtr = this.index.Pointers[(int)paths.Indices[0].index0];
                this.ReadPaths(childPtr, files);
            }

            if (paths.Header.forward != 0)
            {
                var siblingPtr = this.index.Pointers[(int)paths.Header.forward];
                this.ReadPaths(siblingPtr, files);
            }
        }
    }
}
