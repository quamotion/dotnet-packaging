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

        public object PathInfoPointer { get; } = new object();
        public object PathInfo2Pointer { get; } = new object();
        public BomPathInfo PathInfo;
        public BomPathInfo2 PathInfo2;

        public uint SerializedSize
        {
            // 1 integer (parent) + null-terminated string (name)
            get { return 4 + (uint)this.name.Length + 1; }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return this.name;
        }
    }
}
