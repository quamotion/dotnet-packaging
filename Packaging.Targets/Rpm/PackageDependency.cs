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
        /// Initializes a new instance of the <see cref="PackageDependency"/> class.
        /// </summary>
        public PackageDependency()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PackageDependency"/> class.
        /// </summary>
        /// <param name="name">
        /// The name of the dependency.
        /// </param>
        /// <param name="flags">
        /// The dependency flags.
        /// </param>
        /// <param name="version">
        /// The dependency version.
        /// </param>
        public PackageDependency(string name, RpmSense flags, string version)
        {
            this.Name = name;
            this.Flags = flags;
            this.Version = version;
        }

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

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            var other = obj as PackageDependency;

            if (other == null)
            {
                return false;
            }

            return string.Equals(this.Name, other.Name, StringComparison.Ordinal)
                && this.Flags == other.Flags
                && string.Equals(this.Version, other.Version, StringComparison.Ordinal);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {

            return 13 * this.Name.GetHashCode() + 7 * (int)this.Flags + 7 * this.Version.GetHashCode();
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{this.Name} {this.Flags} {this.Version}";
        }
    }
}
