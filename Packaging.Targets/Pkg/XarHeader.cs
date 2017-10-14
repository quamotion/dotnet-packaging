//-----------------------------------------------------------------------
// <copyright file="XarHeader.cs" company="Quamotion">
//     Copyright (c) 2016 Quamotion. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Packaging.Targets.IO;
using System.Runtime.InteropServices;

namespace Packaging.Targets.Pkg
{
    /// <summary>
    /// Represents the header of a <see cref="XarFile"/>.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    internal struct XarHeader
    {
        /// <summary>
        /// The magic for a XAR file. Represents <c>xar!</c> in ASCII.
        /// </summary>
        public const uint Magic = 0x78617221;

        /// <summary>
        /// The current version of the Xar header.
        /// </summary>
        public const uint CurrentVersion = 1;

        /// <summary>
        /// The signature of the header. Should equal <see cref="Magic"/>.
        /// </summary>
        public uint Signature;

        /// <summary>
        /// The size of the header
        /// </summary>
        public ushort Size;

        /// <summary>
        /// The version of the header format. Should equal <see cref="CurrentVersion"/>.
        /// </summary>
        public ushort Version;

        /// <summary>
        /// The compressed length of the table of contents.
        /// </summary>
        public ulong TocLengthCompressed;

        /// <summary>
        /// The uncompressed length of the table of contents.
        /// </summary>
        public ulong TocLengthUncompressed;

        /// <summary>
        /// The algorithm used to calculate checksums.
        /// </summary>
        [MarshalAs(UnmanagedType.U4)]
        public XarChecksum ChecksumAlgorithm;
    }
}
