namespace Packaging.Targets.RpmRepo
{
    /// <summary>
    /// Represents a change log entry for a package.
    /// </summary>
    public class ChangeLogEntry
    {
        /// <summary>
        /// Gets or sets the name of the author who created the change log entry.
        /// </summary>
        public string Author
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the date (as a Unix timestamp, in seconds) at which the change log entry was created.
        /// </summary>
        public long Date
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the change log message.
        /// </summary>
        public string Text
        {
            get;
            set;
        }
    }
}
