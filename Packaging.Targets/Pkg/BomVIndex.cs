using System.Runtime.InteropServices;

namespace Packaging.Targets.Pkg
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct BomVIndex
    {
        /// <summary>
        /// The value of this field is always 1.
        /// </summary>
        public uint unknown0;
        public uint indexToVTree;

        /// <summary>
        /// The value of this field is always 2.
        /// </summary>
        public uint unknown2;

        /// <summary>
        /// The value of this field is always 0.
        /// </summary>
        public byte unknown3;
    }
}
