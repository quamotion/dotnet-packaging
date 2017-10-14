namespace Packaging.Targets.Pkg
{
    /// <summary>
    /// Represents a file in a bill of materials.
    /// </summary>
    internal class BomFile
    {
        /// <summary>
        /// The parent of this file (e.g. the parent directory).
        /// </summary>
        public uint parent;

        /// <summary>
        /// The name of this file.
        /// </summary>
        public string name;

        /// <inheritdoc/>
        public override string ToString()
        {
            return this.name;
        }
    }
}
