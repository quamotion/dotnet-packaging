using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Packaging.Targets.Pkg
{
    /// <summary>
    /// Contains information about paths (entries) in the BOM.
    /// </summary>
    internal class BomPaths
    {
        /// <summary>
        /// Gets or sets the header for the paths section.
        /// </summary>
        public BomPathsHeader Header
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets all indices of the entries in the paths section.
        /// </summary>
        public Collection<BomPathIndices> Indices
        { get; } = new Collection<BomPathIndices>();
    }
}
