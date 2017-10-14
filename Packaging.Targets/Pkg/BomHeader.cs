namespace Packaging.Targets.Pkg
{
    /// <summary>
    /// The header of a BOM file.
    /// </summary>
    internal struct BomHeader
    {
        /// <summary>
        /// The magic used to identify a BOM file. Is equal to <c>BOMStore</c>.
        /// </summary>
        public const ulong Magic = 0x424f4d53746f7265; // BOMStore in ASCII

        /// <summary>
        /// The magic of the BOM file.
        /// </summary>
        public ulong magic;

        /// <summary>
        /// The version of the BOM file format used. Should always be 1.
        /// </summary>
        public uint version;

        /// <summary>
        /// The number of non-empty entries in the <see cref="Bom.blockTable"/>.
        /// </summary>
        public uint numberOfBlocks;

        /// <summary>
        /// The absolute offset to the index table.
        /// </summary>
        public uint indexOffset;

        /// <summary>
        /// The length of the index table. Because the index table is the last entry in the BOM file,
        /// <see cref="indexOffset"/> + <see cref="indexLength"/> will match the entire file length.
        /// </summary>
        public uint indexLength;

        /// <summary>
        /// The absolute offset to the variable table.
        /// </summary>
        public uint varsOffset;

        /// <summary>
        /// The length of the variable table.
        /// </summary>
        public uint varsLength;
    }
}
