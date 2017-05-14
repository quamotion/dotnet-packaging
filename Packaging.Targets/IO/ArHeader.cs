using System;
using System.Runtime.InteropServices;

namespace Packaging.Targets.IO
{
    /// <summary>
    /// The header of an entry in an <see cref="ArFile"/>.
    /// </summary>
    public struct ArHeader : IArchiveHeader
    {
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 16)]
        private char[] fileName;

        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 12)]
        private char[] lastModified;

        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 6)]
        private char[] ownerId;

        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 6)]
        private char[] groupId;

        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 8)]
        private char[] fileMode;

        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 10)]
        private char[] fileSize;

        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 2)]
        private char[] endChar;

        /// <summary>
        /// Gets or sets the name of the current file.
        /// </summary>
        public string FileName
        {
            get { return new string(this.fileName).Trim(); }
            set { this.fileName = value.ToCharArray(); }
        }

        /// <summary>
        /// Gets or sets the date at which the current file was last modified.
        /// </summary>
        public DateTimeOffset LastModified
        {
            get { return DateTimeOffset.FromUnixTimeSeconds((long)Convert.ToUInt32(new string(this.lastModified).Trim())); }
            set { this.lastModified = value.ToUnixTimeSeconds().ToString("x8").PadRight(12).ToCharArray(); }
        }

        /// <summary>
        /// Gets of sets the user ID of the owner of the file.
        /// </summary>
        public uint OwnerId
        {
            get { return Convert.ToUInt32(new string(this.ownerId).Trim()); }
            set { this.ownerId = value.ToString().PadRight(6).ToCharArray(); }
        }
        
        /// <summary>
        /// Gets or sets group ID of the owner of the file.
        /// </summary>
        public uint GroupId
        {
            get { return Convert.ToUInt32(new string(this.groupId).Trim()); }
            set { this.groupId = value.ToString().PadRight(6).ToCharArray(); }
        }

        /// <inheritdoc/>
        public LinuxFileMode FileMode
        {
            get { return (LinuxFileMode)Convert.ToUInt32(new string(this.fileMode).Trim(), 8); }
            set { this.fileMode = ((uint)value).ToString("x8").PadRight(8).ToCharArray(); }
        }

        /// <inheritdoc/>
        public uint FileSize
        {
            get { return Convert.ToUInt32(new string(this.fileSize).Trim()); }
            set { this.fileSize = value.ToString().PadRight(10).ToCharArray(); }
        }

        /// <summary>
        /// Gets or sets the value of the endchar field. This field is used as a checksum.
        /// </summary>
        public string EndChar
        {
            get { return new string(this.endChar); }
            set { this.endChar = value.ToCharArray(); }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return this.FileName;
        }
    }
}
