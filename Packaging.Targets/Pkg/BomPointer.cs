namespace Packaging.Targets.Pkg
{
    /// <summary>
    /// A pointer to a section in the BOM file.
    /// </summary>
    internal struct BomPointer
    {
        /// <summary>
        /// The start address of the section.
        /// </summary>
        public uint address;

        /// <summary>
        /// The length of the section.
        /// </summary>
        public uint length;

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"Address: {this.address}, Length: {this.length}";
        }
    }
}
