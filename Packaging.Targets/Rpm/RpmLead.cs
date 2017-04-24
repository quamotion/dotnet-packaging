using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Packaging.Targets.Rpm
{
    /// <summary>
    /// The lead section is used to identify the package file.
    /// </summary>
    /// <seealso href="http://refspecs.linuxbase.org/LSB_4.1.0/LSB-Core-generic/LSB-Core-generic/pkgformat.html"/>
    internal struct RpmLead
    {
        /// <summary>
        /// Value identifying this file as an RPM format file. This value shall be "\355\253\356\333".
        /// </summary>
        public uint Magic;

        /// <summary>
        /// Value indicating the major version number of the file format version. This value shall be 3.
        /// </summary>
        public byte Major;

        /// <summary>
        /// Value indicating the minor revision number of file format version. This value shall be 0.
        /// </summary>
        public byte Minor;

        /// <summary>
        /// Value indicating whether this is a source or binary package. This value shall be 0 to indicate a binary package.
        /// </summary>
        public ushort Type;

        /// <summary>
        /// Value indicating the architecture for which this package is valid. This value is specified in the relevant architecture
        /// specific part of ISO/IEC 23360.
        /// </summary>
        public ushort ArchNum;

        /// <summary>
        /// A NUL terminated string that provides the package name. This name shall conform with the Package Naming section of this specification.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 66)]
        public byte[] Name;

        /// <summary>
        /// Value indicating the Operating System for which this package is valid. This value shall be 1.
        /// </summary>
        public ushort OsNum;

        /// <summary>
        /// Value indicating the type of the signature used in the Signature part of the file. This value shall be 5.
        /// </summary>
        public ushort SignatureType;

        /// <summary>
        /// Reserved space. The value is undefined.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] Resvered;
    }
}
