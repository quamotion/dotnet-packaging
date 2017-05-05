using System;
using System.Runtime.InteropServices;

namespace Packaging.Targets.IO
{
    /// <summary>
    /// Represents the header of an individual entry in a CPIO file, in ASCII format.
    /// </summary>
    /// <seealso href="https://people.freebsd.org/~kientzle/libarchive/man/cpio.5.txt"/>
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct CpioHeader
    {
        /// <summary>
        /// The integer value octal 070701.  This value can be used to determine
        /// whether this archive is written with little-endian or big-endian integers,
        /// or ASCII.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 6)] // 0 through 5
        private char[] signature;

        /// <summary>
        /// The inode number from the disk.  These are used by
        /// programs that read cpio archives to determine when two entries
        /// refer to the same file. Programs that synthesize cpio archives
        /// should be careful to set these to distinct values for each entry.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 8)] // 6 through 13
        private char[] ino;

        /// <summary>
        /// The mode specifies both the regular permissions and the file type.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 8)] // 14 through 21
        private char[] mode;

        /// <summary>
        /// The numeric user id of the owner.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 8)] // 22 through 29
        private char[] uid;

        /// <summary>
        /// The numeric group id of the owner.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 8)] // 30 through 37
        private char[] gid;

        /// <summary>
        /// The number of links to this file. Directories always have a
        /// value of at least two here. Note that hardlinked files include
        /// file data with every copy in the archive.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 8)] // 38 through 45
        private char[] nlink;

        /// <summary>
        /// Modification time of the file, indicated as the number of seconds
        /// since the start of the epoch, 00:00:00 UTC January 1, 1970.  The
        /// four-byte integer is stored with the most-significant 16 bits
        /// first followed by the least-significant 16 bits.Each of the two
        /// 16 bit values are stored in machine-native byte order.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 8)] // 46 through 53
        private char[] mtime;

        /// <summary>
        /// The size of the file.  Note that this archive format is limited
        /// to four gigabyte file sizes.See mtime above for a description
        /// of the storage of four-byte integers.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 8)] // 54 through 61
        private char[] filesize;

        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 8)] // 62 through 69
        private char[] devMajor;

        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 8)] // 70 through 77
        private char[] devMinor;

        /// <summary>
        /// For block special and character special entries, this field contains
        /// the associated device number. For all other entry types,
        /// it should be set to zero by writers and ignored by readers.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 8)] // 78 through 85
        private char[] rdevMajor;

        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 8)] // 86 through 93
        private char[] rdevMinor;

        /// <summary>
        /// The number of bytes in the pathname that follows the header.
        /// This count includes the trailing NUL byte.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 8)] // 94 through 101
        private char[] namesize;

        /// <summary>
        /// This field is always set to zero by writers and ignored by readers.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 8)] // 102 through 109
        private char[] check;

        /// <summary>
        /// Gets an empty <see cref="CpioHeader"/> entry.
        /// </summary>
        public static CpioHeader Empty
        {
            get
            {
                return new CpioHeader()
                {
                    Check = 0,
                    DevMajor = 0,
                    DevMinor = 0,
                    FileSize = 0,
                    Gid = 0,
                    Ino = 0,
                    Mode = 0,
                    Mtime = DateTimeOffset.FromUnixTimeSeconds(0),
                    NameSize = 0,
                    Nlink = 1,
                    RDevMajor = 0,
                    RDevMinor = 0,
                    Signature = "070701",
                    Uid = 0
                };
            }
        }

        /// <summary>
        /// Gets or sets the value of the <see cref="signature"/> field as a <see cref="string"/>.
        /// </summary>
        public string Signature
        {
            get { return new string(this.signature); }
            set { this.signature = value.ToCharArray(); }
        }

        /// <summary>
        /// Gets or sets the value of the <see cref="ino"/> field as a <see cref="uint"/>.
        /// </summary>
        public uint Ino
        {
            get { return Convert.ToUInt32(new string(this.ino), 16); }
            set { this.ino = value.ToString("x8").ToCharArray(); }
        }

        /// <summary>
        /// Gets or sets the value of the <see cref="mode"/> field as a <see cref="uint"/>.
        /// </summary>
        public LinuxFileMode Mode
        {
            get { return (LinuxFileMode)Convert.ToUInt32(new string(this.mode), 16); }
            set { this.mode = ((uint)value).ToString("x8").ToCharArray(); }
        }

        /// <summary>
        /// Gets or sets the value of the <see cref="uid"/> field as a <see cref="uint"/>.
        /// </summary>
        public uint Uid
        {
            get { return Convert.ToUInt32(new string(this.uid), 16); }
            set { this.uid = value.ToString("x8").ToCharArray(); }
        }

        /// <summary>
        /// Gets or sets the value of the <see cref="gid"/> field as a <see cref="uint"/>.
        /// </summary>
        public uint Gid
        {
            get { return Convert.ToUInt32(new string(this.gid), 16); }
            set { this.gid = value.ToString("x8").ToCharArray(); }
        }

        /// <summary>
        /// Gets or sets the value of the <see cref="nlink"/> field as a <see cref="uint"/>.
        /// </summary>
        public uint Nlink
        {
            get { return Convert.ToUInt32(new string(this.nlink), 16); }
            set { this.nlink = value.ToString("x8").ToCharArray(); }
        }

        /// <summary>
        /// Gets or sets the value of the <see cref="mtime"/> field as a <see cref="uint"/>.
        /// </summary>
        public DateTimeOffset Mtime
        {
            get { return DateTimeOffset.FromUnixTimeSeconds((long)Convert.ToUInt32(new string(this.mtime), 16)); }
            set { this.mtime = value.ToUnixTimeSeconds().ToString("x8").ToCharArray(); }
        }

        /// <summary>
        /// Gets or sets the value of the <see cref="filesize"/> field as a <see cref="uint"/>.
        /// </summary>
        public uint FileSize
        {
            get { return Convert.ToUInt32(new string(this.filesize), 16); }
            set { this.filesize = value.ToString("x8").ToCharArray(); }
        }

        /// <summary>
        /// Gets or sets the value of the <see cref="devMajor"/> field as a <see cref="uint"/>.
        /// </summary>
        public uint DevMajor
        {
            get { return Convert.ToUInt32(new string(this.devMajor), 16); }
            set { this.devMajor = value.ToString("x8").ToCharArray(); }
        }

        /// <summary>
        /// Gets or sets the value of the <see cref="devMinor"/> field as a <see cref="uint"/>.
        /// </summary>
        public uint DevMinor
        {
            get { return Convert.ToUInt32(new string(this.devMinor), 16); }
            set { this.devMinor = value.ToString("x8").ToCharArray(); }
        }

        /// <summary>
        /// Gets or sets the value of the <see cref="rdevMajor"/> field as a <see cref="uint"/>.
        /// </summary>
        public uint RDevMajor
        {
            get { return Convert.ToUInt32(new string(this.rdevMajor), 16); }
            set { this.rdevMajor = value.ToString("x8").ToCharArray(); }
        }

        /// <summary>
        /// Gets or sets the value of the <see cref="rdevMinor"/> field as a <see cref="uint"/>.
        /// </summary>
        public uint RDevMinor
        {
            get { return Convert.ToUInt32(new string(this.rdevMinor), 16); }
            set { this.rdevMinor = value.ToString("x8").ToCharArray(); }
        }

        /// <summary>
        /// Gets or sets the value of the <see cref="namesize"/> field as a <see cref="uint"/>.
        /// </summary>
        public uint NameSize
        {
            get { return Convert.ToUInt32(new string(this.namesize), 16); }
            set { this.namesize = value.ToString("x8").ToCharArray(); }
        }

        /// <summary>
        /// Gets or sets the value of the <see cref="check"/> field as a <see cref="uint"/>.
        /// </summary>
        public uint Check
        {
            get { return Convert.ToUInt32(new string(this.check), 16); }
            set { this.check = value.ToString("x8").ToCharArray(); }
        }
    }
}
