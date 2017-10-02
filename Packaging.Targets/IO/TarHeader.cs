using System;
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
            get { return DateTimeOffset.FromUnixTimeSeconds((long)Convert.ToUInt32(this.GetString(this.mtime))); }
            set { this.mtime = value.ToUnixTimeSeconds().ToString("x8").ToCharArray(); }
        }

        public uint Checksum
        {
            get { return Convert.ToUInt32(this.GetString(this.chksum), 8); }
            set { this.chksum = value.ToString("x8").ToCharArray(); }
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

        public uint Version
        {
            get { return Convert.ToUInt32(this.GetString(this.version), 8); }
            set { this.version = value.ToString("x8").ToCharArray(); }
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

        public uint DevMajor
        {
            get { return Convert.ToUInt32(this.GetString(this.devmajor), 8); }
            set { this.devmajor = value.ToString("x8").ToCharArray(); }
        }

        public uint DevMinor
        {
            get { return Convert.ToUInt32(this.GetString(this.devminor), 8); }
            set { this.devminor = value.ToString("x8").ToCharArray(); }
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
