using System;

namespace Packaging.Targets.Deb
{
    /// <summary>
    /// Represents a Debian installer package.
    /// </summary>
    internal class DebPackage
    {
        /// <summary>
        /// Gets or sets the Debian installer file format used.
        /// </summary>
        public Version PackageFormatVersion
        {
            get;
            set;
        }
    }
}
