using System;

namespace Packaging.Targets.IO
{
    /// <summary>
    /// Represents the Unix-style file permission flags. Taken from <c>&lt;sys/stat.h&gt;</c>.
    /// </summary>
    /// <seealso href="http://minnie.tuhs.org/cgi-bin/utree.pl?file=4.4BSD/usr/include/sys/stat.h"/>
    [Flags]
    public enum LinuxFileMode : ushort
    {
        /// <summary>
        /// Set user ID on execution
        /// </summary>
        S_ISUID = 0x0800,

        /// <summary>
        /// Set group ID on execution
        /// </summary>
        S_ISGID = 0x0400,

        /// <summary>
        /// Save swapped text after use (sticky).
        /// </summary>
        S_ISVTX = 0x0200,

        /// <summary>
        /// Read by owner
        /// </summary>
        S_IRUSR = 0x0100,

        /// <summary>
        /// Write by owner
        /// </summary>
        S_IWUSR = 0x0080,

        /// <summary>
        /// Execute by owner
        /// </summary>
        S_IXUSR = 0x0040,

        /// <summary>
        ///  Read by group
        /// </summary>
        S_IRGRP = 0x0020,

        /// <summary>
        /// Write by group
        /// </summary>
        S_IWGRP = 0x0010,

        /// <summary>
        /// Execute by group
        /// </summary>
        S_IXGRP = 0x0008,

        /// <summary>
        /// Read by other
        /// </summary>
        S_IROTH = 0x0004,

        /// <summary>
        /// Write by other
        /// </summary>
        S_IWOTH = 0x0002,

        /// <summary>
        /// Execute by other
        /// </summary>
        S_IXOTH = 0x0001,

        /// <summary>
        /// The file is a named pipe (fifo).
        /// </summary>
        S_IFIFO = 0x1000, // 010000 in octal

        /// <summary>
        /// The file is a character special device.
        /// </summary>
        S_IFCHR = 0x2000, // 0020000 in octal

        /// <summary>
        /// The file is a directory.
        /// </summary>
        S_IFDIR = 0x4000, // 0040000 in octal

        /// <summary>
        /// The file is a block special device.
        /// </summary>
        S_IFBLK = 0x6000, // 0060000 in octal

        /// <summary>
        /// The file is a regular file.
        /// </summary>
        S_IFREG = 0x8000, // 0100000 in octal

        /// <summary>
        /// The file is a symbolic link.
        /// </summary>
        S_IFLNK = 0xA000, // 0120000 in octal

        /// <summary>
        /// The file is a Unix socket.
        /// </summary>
        S_IFSOCK = 0xC000 // 0140000 in octal

        /// <summary>
        /// A flag to get all permissions applied to this file.
        /// </summary>
        PermissionsMask = 0x0FFF,

        /// <summary>
        /// A flag to get the file type.
        /// </summary>
        FileTypeMask = 0xF000,
    }
}
