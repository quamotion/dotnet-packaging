using System.Collections.ObjectModel;

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
    }
}
