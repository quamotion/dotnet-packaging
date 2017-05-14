using Packaging.Targets.IO;
using System.Collections.ObjectModel;

namespace Packaging.Targets.Rpm
{
    /// <summary>
    /// Provides a very basic implementation of the <see cref="IFileAnalyzer"/> class. To get the details that are consistent
    /// with those found in real-world RPM packages, more complete parsing of ELF files is required to get the full list of
    /// dependencies, as well as the class (description).
    /// </summary>
    internal class FileAnalyzer : IFileAnalyzer
    {
        /// <inheritdoc/>
        public virtual Collection<PackageDependency> DetermineRequires(string filename, CpioHeader fileHeader, byte[] header)
        {
            // For now, report no dependencies at all. Could be enhanced if ELF parsing is available.
            var dependencies = new Collection<PackageDependency>();

            return dependencies;
        }

        /// <inheritdoc/>
        public virtual Collection<PackageDependency> DetermineProvides(string filename, CpioHeader fileHeader, byte[] header)
        {
            // For now, report no provides at all. Could be enhanced if ELF parsing is available.
            var dependencies = new Collection<PackageDependency>();

            return dependencies;
        }

        /// <inheritdoc/>
        public virtual RpmFileFlags DetermineFlags(string filename, CpioHeader fileHeader, byte[] header)
        {
            // The only custom flags which are supported for now are the RPMFILE_DOC flags for non-executable
            // files.
            if (fileHeader.FileMode.HasFlag(LinuxFileMode.S_IFDIR))
            {
                return RpmFileFlags.None;
            }
            else if (fileHeader.FileMode.HasFlag(LinuxFileMode.S_IFLNK))
            {
                return RpmFileFlags.None;
            }
            else if (!fileHeader.FileMode.HasFlag(LinuxFileMode.S_IXGRP)
                    && !fileHeader.FileMode.HasFlag(LinuxFileMode.S_IXOTH)
                    && !fileHeader.FileMode.HasFlag(LinuxFileMode.S_IXUSR))
            {
                return RpmFileFlags.RPMFILE_DOC;
            }
            else
            {
                return RpmFileFlags.None;
            }
        }

        /// <inheritdoc/>
        public virtual RpmFileColor DetermineColor(string filename, CpioHeader fileHeader, byte[] header)
        {
            // Only support ELF32 and ELF64 colors; otherwise default to BLACK.
            if (ElfFile.IsElfFile(header))
            {
                ElfHeader elfHeader = ElfFile.ReadHeader(header);

                if (elfHeader.@class == ElfClass.Elf32)
                {
                    return RpmFileColor.RPMFC_ELF32;
                }
                else
                {
                    return RpmFileColor.RPMFC_ELF64;
                }
            }
            else
            {
                return RpmFileColor.RPMFC_BLACK;
            }
        }

        /// <inheritdoc/>
        public virtual bool IsExecutable(byte[] header)
        {
            if (ElfFile.IsElfFile(header))
            {
                ElfHeader elfHeader = ElfFile.ReadHeader(header);
                return elfHeader.type.HasFlag(ElfType.Executable);
            }

            return false;
        }

        /// <inheritdoc/>
        public virtual string DetermineClass(string filename, CpioHeader fileHeader, byte[] header)
        {
            // Very simplistic implementation - non-executable files are considered to be tet files.
            if (fileHeader.FileMode.HasFlag(LinuxFileMode.S_IFDIR))
            {
                return "directory";
            }

            if (fileHeader.FileMode.HasFlag(LinuxFileMode.S_IFLNK))
            {
                return string.Empty;
            }

            if (filename.EndsWith(".svg"))
            {
                return "SVG Scalable Vector Graphics image";
            }
            else if (filename.EndsWith(".ttf"))
            {
                return "TrueType font data";
            }
            else if (filename.EndsWith(".woff"))
            {
                return string.Empty;
            }
            else if (filename.EndsWith(".woff2"))
            {
                return string.Empty;
            }
            else if (filename.EndsWith(".eot"))
            {
                return string.Empty;
            }

            if (!fileHeader.FileMode.HasFlag(LinuxFileMode.S_IXGRP)
                && !fileHeader.FileMode.HasFlag(LinuxFileMode.S_IXOTH)
                && !fileHeader.FileMode.HasFlag(LinuxFileMode.S_IXUSR))
            {
                for (int i = 0; i < header.Length; i++)
                {
                    if (header[i] > 127)
                    {
                        return "UTF-8 Unicode text";
                    }
                }

                return "ASCII text";
            }

            return string.Empty;
        }
    }
}
