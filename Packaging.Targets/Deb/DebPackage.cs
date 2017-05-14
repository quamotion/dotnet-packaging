using System;
using System.Collections.Generic;

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

        /// <summary>
        /// Gets or sets the value of the control file.
        /// </summary>
        public Dictionary<string, string> ControlFile
        {
            get;
            set;
        }
    }
}
