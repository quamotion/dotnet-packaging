namespace Packaging.Targets.RpmRepo
{
    /// <summary>
    /// Represents an entry in the repo metadata file.
    /// </summary>
    public class RepoData
    {
        /// <summary>
        /// Gets or sets the type of the file.
        /// </summary>
        public string Type
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the checksum value of the compressed file.
        /// </summary>
        public string Checksum
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the checksum size - for example, <c>sha256</c>.
        /// </summary>
        public string ChecksumType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the checksum of the uncompressed file.
        /// </summary>
        public string OpenChecksum
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the type of the checksum of the uncompressed file - for example, <c>sha256</c>.
        /// </summary>
        public string OpenChecksumType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the relative path to the file.
        /// </summary>
        public string Location
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the time at which the file was generated, as a Unix timestamp, in seconds.
        /// </summary>
        public int Timestamp
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the compressed size of the file.
        /// </summary>
        public int Size
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the uncompressed size of the file.
        /// </summary>
        public int OpenSize
        {
            get;
            set;
        }
    }
}
