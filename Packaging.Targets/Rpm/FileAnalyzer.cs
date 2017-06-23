using Packaging.Targets.IO;
using System;
using System.Collections.ObjectModel;
using System.IO;

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
        public virtual Collection<PackageDependency> DetermineRequires(ArchiveEntry entry)
        {
            // For now, report no dependencies at all. Could be enhanced if ELF parsing is available.
            var dependencies = new Collection<PackageDependency>();

            return dependencies;
        }

        /// <inheritdoc/>
        public virtual Collection<PackageDependency> DetermineProvides(ArchiveEntry entry)
        {
            // For now, report no provides at all. Could be enhanced if ELF parsing is available.
            var dependencies = new Collection<PackageDependency>();

            return dependencies;
        }

        /// <inheritdoc/>
        public virtual RpmFileFlags DetermineFlags(ArchiveEntry entry)
        {
            // The only custom flags which are supported for now are the RPMFILE_DOC flags for non-executable
            // files.
            if (entry.Mode.HasFlag(LinuxFileMode.S_IFDIR))
            {
                return RpmFileFlags.None;
            }
            else if (entry.Mode.HasFlag(LinuxFileMode.S_IFLNK))
            {
                return RpmFileFlags.None;
            }
            else if (!entry.Mode.HasFlag(LinuxFileMode.S_IXGRP)
                    && !entry.Mode.HasFlag(LinuxFileMode.S_IXOTH)
                    && !entry.Mode.HasFlag(LinuxFileMode.S_IXUSR))
            {
                return RpmFileFlags.RPMFILE_DOC;
            }
            else
            {
                return RpmFileFlags.None;
            }
        }

        /// <inheritdoc/>
        public virtual RpmFileColor DetermineColor(ArchiveEntry entry)
        {
            // Only support ELF32 and ELF64 
            switch (entry.Type)
            {
                case ArchiveEntryType.Executable32:
                    return RpmFileColor.RPMFC_ELF32;

                case ArchiveEntryType.Executable64:
                    return RpmFileColor.RPMFC_ELF64;

                default:
                    return RpmFileColor.RPMFC_BLACK;
            }
        }

        /// <inheritdoc/>
        public virtual bool IsExecutable(ArchiveEntry entry)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public virtual string DetermineClass(ArchiveEntry entry)
        {
            // Very simplistic implementation - non-executable files are considered to be tet files.
            if (entry.Mode.HasFlag(LinuxFileMode.S_IFDIR))
            {
                return "directory";
            }

            if (entry.Mode.HasFlag(LinuxFileMode.S_IFLNK))
            {
                return string.Empty;
            }

            if (entry.TargetPath.EndsWith(".svg"))
            {
                return "SVG Scalable Vector Graphics image";
            }
            else if (entry.TargetPath.EndsWith(".ttf"))
            {
                return "TrueType font data";
            }
            else if (entry.TargetPath.EndsWith(".woff"))
            {
                return string.Empty;
            }
            else if (entry.TargetPath.EndsWith(".woff2"))
            {
                return string.Empty;
            }
            else if (entry.TargetPath.EndsWith(".eot"))
            {
                return string.Empty;
            }

            if (!entry.Mode.HasFlag(LinuxFileMode.S_IXGRP)
                && !entry.Mode.HasFlag(LinuxFileMode.S_IXOTH)
                && !entry.Mode.HasFlag(LinuxFileMode.S_IXUSR))
            {
                if (entry.IsAscii)
                {
                    return "ASCII text";
                }
                else
                {
                    return "UTF-8 Unicode text";
                }
            }

            return string.Empty;
        }
    }
}
