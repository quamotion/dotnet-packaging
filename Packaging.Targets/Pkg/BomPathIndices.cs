namespace Packaging.Targets.Pkg
{
    /// <summary>
    /// For an entry in a BOM, the pointers to additional information about that entry.
    /// </summary>
    internal struct BomPathIndices
    {
        /// <summary>
        /// For leaf entries, a link to <see cref="BomPathInfo"/>; for branch entries,
        /// a link to <see cref="BomPaths"/>.
        /// </summary>
        public uint index0;

        /// <summary>
        /// A pointer to a <see cref="BomFile"/> struct.
        /// </summary>
        public uint index1;
    }
}
