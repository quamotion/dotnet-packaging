namespace Packaging.Targets.Pkg
{
    /// <summary>
    /// A variable in a BOM file.
    /// </summary>
    internal struct BomVariable
    {
        /// <summary>
        /// The index of the variable.
        /// </summary>
        public uint index;

        /// <summary>
        /// The length of the variable name.
        /// </summary>
        public byte length;

        /// <summary>
        /// The name of the variable.
        /// </summary>
        public string name;

        /// <inheritdoc/>
        public override string ToString()
        {
            return this.name;
        }
    }
}
