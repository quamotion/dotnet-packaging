namespace Packaging.Targets.Pkg
{
    /// <summary>
    /// The header for a path entry.
    /// </summary>
    internal struct BomPathsHeader
    {
        /// <summary>
        /// A boolean value indicating whether this entry is a leaf node or not.
        /// </summary>
        public ushort isLeaf;

        /// <summary>
        /// The number of child entries.
        /// </summary>
        public ushort count;

        /// <summary>
        /// A boolean value indicating whether this entry is a forward entry or not.
        /// </summary>
        public uint forward;

        /// <summary>
        /// A boolean value indicating whether this entry is a backward entry or not.
        /// </summary>
        public uint backward;
    }
}
