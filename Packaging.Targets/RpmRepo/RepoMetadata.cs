using System.Collections.ObjectModel;

namespace Packaging.Targets.RpmRepo
{
    /// <summary>
    /// Represents a file which contains repository metadata information.
    /// </summary>
    public class RepoMetadata
    {
        /// <summary>
        /// Gets or sets the revision of the repository metadata file.
        /// </summary>
        public int Revision
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a list of all files which contain repository metadata.
        /// </summary>
        public Collection<RepoData> Data
        { get; } = new Collection<RepoData>();
    }
}
