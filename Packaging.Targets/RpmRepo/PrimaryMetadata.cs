using System.Collections.ObjectModel;

namespace Packaging.Targets.RpmRepo
{
    /// <summary>
    /// Represents a file which contains metadata.
    /// </summary>
    public class PrimaryMetadata
    {
        /// <summary>
        /// Gets the package metadata information.
        /// </summary>
        public Collection<PackageMetadata> Packages
        { get; } = new Collection<PackageMetadata>();
    }
}
