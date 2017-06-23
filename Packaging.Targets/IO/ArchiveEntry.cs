using System;

namespace Packaging.Targets.IO
{
    /// <summary>
    /// Represents an entry in a Unix archive (usually a CPIO or tar archive).
    /// </summary>
    public class ArchiveEntry
    {
        /// <summary>
        /// Gets or sets the full path of the file or directory on the target file system.
        /// </summary>
        public string TargetPath
        { get; set; }

        /// <summary>
        /// Gets or sets the name of the owner of the file or directory.
        /// </summary>
        public string Owner
        { get; set; }

        /// <summary>
        /// Gets or sets the name of the group of the file or directory.
        /// </summary>
        public string Group
        { get; set; }

        /// <summary>
        /// Gets or sets the file mode of the file.
        /// </summary>
        public LinuxFileMode Mode
        { get; set; }

        /// <summary>
        /// Gets or sets the full path to the file on the source operating system.
        /// </summary>
        public string SourceFilename
        { get; set; }

        /// <summary>
        /// Gets or sets the total file size, in bytes.
        /// </summary>
        public uint FileSize
        { get; set; }

        /// <summary>
        /// Gets or sets the date at which the file was last modified.
        /// </summary>
        public DateTimeOffset Modified
        { get; set; }

        /// <summary>
        /// Gets or sets the SHA256 of the file contents.
        /// </summary>
        public byte[] Sha256
        { get; set; }

        /// <summary>
        /// Gets or sets the file type (executable or not).
        /// </summary>
        public ArchiveEntryType Type
        { get; set; }

        /// <summary>
        /// Gets or sets, if the file is a link, the link target.
        /// </summary>
        public string LinkTo
        { get; set; }

        /// <summary>
        /// Gets or sets the inode number for the file.
        /// </summary>
        public uint Inode
        { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the file only contains ASCII characters.
        /// </summary>
        public bool IsAscii
        { get; set; }
    }
}
