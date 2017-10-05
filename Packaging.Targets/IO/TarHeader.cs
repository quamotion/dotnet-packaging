using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace Packaging.Targets.IO
{
    /// <summary>
    /// Represents the header for an individual entry in a <c>.tar</c> archive.
    /// </summary>
    internal struct TarHeader : IArchiveHeader
    {
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 100)]
        private char[] name;

        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 8)]
        private char[] mode;

        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 8)]
        private char[] uid;

        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 8)]
        private char[] gid;

        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 12)]
        private char[] size;

        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 12)]
        private char[] mtime;

        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 8)]
        private char[] chksum;

        private byte typeflag;

        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 100)]
        private char[] linkname;

        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 6)]
        private char[] magic;

        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 2)]
        private char[] version;

        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 32)]
        private char[] uname;

        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 32)]
        private char[] gname;

        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 8)]
        private char[] devmajor;

        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 8)]
        private char[] devminor;

        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 155)]
        private char[] prefix;

        
        
        /// <summary>
        /// Gets or sets the name of the current file.
        /// </summary>
        public string FileName
        {
            get { return this.GetString(this.name); }
            set { this.name = value.ToCharArray(); }
        }

        public LinuxFileMode FileMode
        {
            get { return (LinuxFileMode)Convert.ToUInt32(this.GetString(this.mode), 8); }
            set { this.mode = ((uint)value).ToString("x8").ToCharArray(); }
        }

        public uint UserId
        {
            get { return Convert.ToUInt32(this.GetString(this.uid), 8); }
            set { this.uid = value.ToString().ToCharArray(); }
        }

        public uint GroupId
        {
            get { return Convert.ToUInt32(this.GetString(this.gid), 8); }
            set { this.gid = value.ToString("x8").ToCharArray(); }
        }

        public uint FileSize
        {
            get { return Convert.ToUInt32(this.GetString(this.size), 8); }
            set { this.size = value.ToString("x8").ToCharArray(); }
        }

        public DateTimeOffset LastModified
        {
            get { return DateTimeOffset.FromUnixTimeSeconds((long)Convert.ToUInt64(this.GetString(this.mtime), 8)); }
            set { this.mtime = value.ToUnixTimeSeconds().ToString("x8").ToCharArray(); }
        }

        public uint Checksum
        {
            get { return Convert.ToUInt32(this.GetString(this.chksum), 8); }
            set { this.chksum = value.ToString("x8").ToCharArray(); }
        }

        public unsafe uint ComputeChecksum()
        {
            var other = this;
            other.chksum = new string(' ', 8).ToCharArray();
            var data = new byte[Marshal.SizeOf<TarHeader>()];
            fixed (byte* ptr = data)
                Marshal.StructureToPtr(other, new IntPtr(ptr), true);
            uint sum = 0;
            foreach (var b in data)
                sum += b;
            return sum;
        }

        public TarTypeFlag TypeFlag
        {
            get { return (TarTypeFlag)this.typeflag; }
            set { this.typeflag = (byte)value; }
        }

        public string LinkName
        {
            get { return this.GetString(this.linkname); }
            set { this.linkname = value.ToCharArray(); }
        }

        public string Magic
        {
            get { return this.GetString(this.magic).Trim(); }
            set { this.magic = value.PadRight(6).ToCharArray(); }
        }

        public uint? Version
        {
            get
            {
                if (uint.TryParse(version.ToString(), out uint rv))
                    return rv;
                return null;
            }
            set { this.version = (value == null ? " " : value.Value.ToString("x8")).ToCharArray(); }
        }

        public string UserName
        {
            get { return this.GetString(this.uname); }
            set { this.uname = value.ToCharArray(); }
        }

        public string GroupName
        {
            get { return this.GetString(this.gname); }
            set { this.gname = value.ToCharArray(); }
        }

        public uint? DevMajor
        {
            get => devmajor[0] == (char) 0 ? (uint?) null : Convert.ToUInt32(this.GetString(this.devmajor), 8);
            set => devmajor = (value == null ? new string((char) 0, 8) : value.Value.ToString("x8")).ToCharArray();
        }

        public uint? DevMinor
        {
            get => devminor[0] == (char) 0 ? (uint?) null : Convert.ToUInt32(this.GetString(this.devminor), 8);
            set => devminor = (value == null ? new string((char) 0, 8) : value.Value.ToString("x8")).ToCharArray();
        }

        public string Prefix
        {
            get { return this.GetString(this.prefix); }
            set { this.prefix = value.ToCharArray(); }
        }

        private string GetString(char[] buffer)
        {
            int count = 0;

            while (count < buffer.Length && buffer[count] != '\0')
            {
                count++;
            }

            return new string(buffer, 0, count);
        }
    }
}
