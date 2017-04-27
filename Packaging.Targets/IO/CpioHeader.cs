using System;
using System.Runtime.InteropServices;

namespace Packaging.Targets.IO
{
    /// <summary>
    /// Represents the header of an individual entry in a CPIO file, in ASCII format.
    /// </summary>
    /// <seealso href="https://people.freebsd.org/~kientzle/libarchive/man/cpio.5.txt"/>
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    internal struct CpioHeader
    {
        /// <summary>
        /// The integer value octal 070707.  This value can be used to determine
        /// whether this archive is written with little-endian or big-endian integers,
        /// or ASCII.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 6)]
        public char[] Signature;

        /// <summary>
        /// The inode number from the disk.  These are used by
        /// programs that read cpio archives to determine when two entries
        /// refer to the same file.Programs that synthesize cpio archives
        /// should be careful to set these to distinct values for each entry.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 8)]
        public char[] Ino;

        /// <summary>
        /// The mode specifies both the regular permissions and the file type.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 8)]
        public char[] Mode;

        /// <summary>
        /// The numeric user id of the owner.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 8)]
        public char[] Uid;

        /// <summary>
        /// The numeric group id of the owner.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 8)]
        public char[] Gid;

        /// <summary>
        /// The number of links to this file. Directories always have a
        /// value of at least two here. Note that hardlinked files include
        /// file data with every copy in the archive.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 8)]
        public char[] Nlink;

        /// <summary>
        /// Modification time of the file, indicated as the number of seconds
        /// since the start of the epoch, 00:00:00 UTC January 1, 1970.  The
        /// four-byte integer is stored with the most-significant 16 bits
        /// first followed by the least-significant 16 bits.Each of the two
        /// 16 bit values are stored in machine-native byte order.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 8)]
        public char[] Mtime;

        /// <summary>
        /// The size of the file.  Note that this archive format is limited
        /// to four gigabyte file sizes.See mtime above for a description
        /// of the storage of four-byte integers.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 8)]
        public char[] Filesize;

        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 8)]
        public char[] DevMajor;

        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 8)]
        public char[] DevMinor;

        /// <summary>
        /// For block special and character special entries, this field contains
        /// the associated device number.For all other entry types,
        /// it should be set to zero by writers and ignored by readers.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 8)]
        public char[] RdevMajor;

        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 8)]
        public char[] RdevMinor;

        /// <summary>
        /// The number of bytes in the pathname that follows the header.
        /// This count includes the trailing NUL byte.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 8)]
        public char[] Namesize;

        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 8)]
        public char[] Check;

        /// <summary>
        /// Gets the value of the <see cref="Signature"/> field as a <see cref="string"/>.
        /// </summary>
        public string Magic
        {
            get { return new string(this.Signature); }
        }

        /// <summary>
        /// Gets the value of the <see cref="Namesize"/> field as a <see cref="uint"/>.
        /// </summary>
        public uint NameLength
        {
            get { return Convert.ToUInt32(new string(this.Namesize), 16); }
        }

        /// <summary>
        /// Gets the value of the <see cref="Filesize"/> field as a <see cref="uint"/>.
        /// </summary>
        public uint ContentLength
        {
            get { return Convert.ToUInt32(new string(this.Filesize), 16); }
        }
    }
}
