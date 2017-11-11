using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

namespace Packaging.Targets.Pkg
{
    /// <summary>
    /// Contains additional information about this BOM file.
    /// </summary>
    internal class BomInfo
    {
        /// <summary>
        /// Gets or sets the header for the BOM information section.
        /// </summary>
        public BomInfoHeader Header
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the entries in the BOM information section.
        /// </summary>
        public Collection<BomInfoEntry> Entries
        { get; } = new Collection<BomInfoEntry>();

        public uint SerializedSize
        {
            get
            {
                return (uint)Marshal.SizeOf<BomInfoHeader>() + (uint)this.Entries.Count * (uint)Marshal.SizeOf<BomInfoEntry>();
            }
        }
    }
}
