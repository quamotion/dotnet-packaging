using Packaging.Targets.IO;
using System;
using System.Runtime.InteropServices;

namespace Packaging.Targets.Pkg
{
    /// <summary>
    /// Additional information about an entry in a BOM file.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct BomPathInfo2
    {
        /// <summary>
        /// The type of file.
        /// </summary>
        public BomFileType type;

        /// <summary>
        /// The value of this field is always 1.
        /// </summary>
        public byte unknown0;

        /// <summary>
        /// The architecture (?) of the entry.
        /// </summary>
        public ushort architecture;

        /// <summary>
        /// The file mode of the entry.
        /// </summary>
        public LinuxFileMode mode;

        /// <summary>
        /// The ID of the user which owns the entry.
        /// </summary>
        public uint user;

        /// <summary>
        /// The ID of the group which owns the entry.
        /// </summary>
        public uint group;

        /// <summary>
        /// The time at which the entry was last modified, as a Unix timestamp.
        /// </summary>
        public uint modtime;

        /// <summary>
        /// The size of the entry.
        /// </summary>
        public uint size;

        /// <summary>
        /// The value of this field is always 1.
        /// </summary>
        public byte unknown1;

        /// <summary>
        /// A checksum/device type value.
        /// </summary>
        public uint checksum;

        /// <summary>
        /// The length of the link name.
        /// </summary>
        uint linkNameLength;

        /// <summary>
        /// Gets or sets the time at which the file was last modified.
        /// </summary>
        public DateTimeOffset LastModified
        {
            get => DateTimeOffset.FromUnixTimeSeconds(this.modtime);
            set => this.modtime = (uint)value.ToUnixTimeSeconds();
        }
    }
}
