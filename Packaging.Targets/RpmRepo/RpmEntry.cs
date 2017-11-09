namespace Packaging.Targets.RpmRepo
{
    /// <summary>
    /// Represents a RPM entry, usually a dependency.
    /// </summary>
    public class RpmEntry
    {
        /// <summary>
        /// Gets or sets the name of the entry.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the flags for this entry. Used to compare versions.
        /// </summary>
        public string Flags
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the version epoch.
        /// </summary>
        public int? Epoch
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the version number.
        /// </summary>
        public string Version
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the release number.
        /// </summary>
        public string Release
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this is a pre-install script.
        /// </summary>
        public int? Pre
        {
            get;
            set;
        }
    }
}
