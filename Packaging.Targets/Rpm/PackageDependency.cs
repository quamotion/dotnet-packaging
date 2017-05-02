using System;
using System.Collections.Generic;
using System.Text;

namespace Packaging.Targets.Rpm
{
    /// <summary>
    /// Represents a dependency on another package, or, a dependency on a specific RPM feature.
    /// </summary>
    internal class PackageDependency
    {
        /// <summary>
        /// Gets or sets a value indicating the dependency constraints.
        /// </summary>
        public RpmSense Flags
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the package which is a dependency.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the version of the package on which a dependency is taken.
        /// </summary>
        public string Version
        {
            get;
            set;
        }
    }
}
