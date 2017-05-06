using System;

namespace Packaging.Targets.Rpm
{
    /// <summary>
    /// Provides methods for working with ELF files.
    /// </summary>
    internal static class ElfFile
    {
        /// <summary>
        /// Determines whether a file is an ELF file or not.
        /// </summary>
        /// <param name="header">
        /// An array containing at least the first 4 bytes of the file.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the file is an ELF file; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsElfFile(byte[] header)
        {
            return header.Length > 4 && header[0] == 0x7f && header[1] == 0x45 && header[2] == 0x4c && header[3] == 0x46;
        }

        /// <summary>
        /// Reads the <see cref="ElfHeader"/> of an ELF file.
        /// </summary>
        /// <param name="header">
        /// An array containing at least the first 0x14 bytes of the file.
        /// </param>
        /// <returns>
        /// A <see cref="ElfHeader"/> object represening the ELF file header.
        /// </returns>
        internal static ElfHeader ReadHeader(byte[] header)
        {
            if (!IsElfFile(header))
            {
                throw new InvalidOperationException();
            }

            ElfHeader value = default(ElfHeader);
            value.@class = (ElfClass)header[4];
            value.data = header[5];
            value.version = header[6];
            value.osAbi = header[7];
            value.abiVersion = header[8];
            value.type = (ElfType)BitConverter.ToInt16(header, 0x10);
            value.machine = (ElfMachine)BitConverter.ToInt16(header, 0x12);

            return value;
        }
    }
}
