using System;

namespace Packaging.Targets.IO
{
    /// <summary>
    /// A common interface for the header of a file entry in an archive file.
    /// </summary>
    public interface IArchiveHeader
    {
        /// <summary>
        /// Gets the file mode.
        /// </summary>
        LinuxFileMode FileMode { get; }

        /// <summary>
        /// Gets the date at which the file was last modified.
        /// </summary>
        DateTimeOffset LastModified { get; }

        /// <summary>
        /// Gets the size of the file.
        /// </summary>
        uint FileSize { get; }
    }
}
