namespace Packaging.Targets.Pkg
{
    /// <summary>
    /// The header for a <see cref="BomInfo"/> entry.
    /// </summary>
    internal struct BomInfoHeader
    {
        /// <summary>
        /// The version of the BOM file format used. Should always be 1.
        /// </summary>
        public uint version;

        /// <summary>
        /// The number of paths (entries) in the BOM file.
        /// </summary>
        public uint numberOfPaths;

        /// <summary>
        /// The number of info elements.
        /// </summary>
        public uint numberOfInfoEntries;
    }
}
