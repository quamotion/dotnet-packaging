namespace Packaging.Targets.Pkg
{
    /// <summary>
    /// The root entry for a tree of files.
    /// </summary>
    internal struct BomTree
    {
        /// <summary>
        /// The magic for a tree entry. Should always be the ASCII representation of
        /// <c>tree</c>.
        /// </summary>
        public uint tree;

        /// <summary>
        /// The version of the tree entry format. Should always be 1.
        /// </summary>
        public uint version;

        /// <summary>
        /// The index for <see cref="BomPaths"/>.
        /// </summary>
        public uint child;

        /// <summary>
        /// The block size. Should alwasyb e 4096.
        /// </summary>
        public uint blockSize;

        /// <summary>
        /// The total number of pats in this tree, in al leaves combined.
        /// </summary>
        public uint pathCount;

        /// <summary>
        /// The value of this field is reserved.
        /// </summary>
        public uint reserved;
    }
}
