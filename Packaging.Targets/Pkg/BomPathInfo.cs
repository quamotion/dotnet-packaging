namespace Packaging.Targets.Pkg
{
    /// <summary>
    /// Information about a BOM file entry.
    /// </summary>
    internal struct BomPathInfo
    {
        /// <summary>
        /// The ID of the BOM entry.
        /// </summary>
        public uint id;

        /// <summary>
        /// A pointer to a <see cref="BomPathInfo2"/> struct.
        /// </summary>
        public uint index;
    }
}
