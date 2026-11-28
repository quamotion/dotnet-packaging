using System;
using System.Collections.Generic;
using System.Formats.Tar;
using System.IO;

namespace Packaging.Targets.IO
{
    internal static class TarFileCreator
    {
        /// <summary>
        /// Generates a <c>.tar</c> archive based on a list of <see cref="ArchiveEntry"/>
        /// values.
        /// </summary>
        /// <param name="archiveEntries">
        /// The values based on which to generate the archive.
        /// </param>
        /// <param name="targetStream">
        /// The <see cref="Stream"/> which will hold the archive.
        /// </param>
        public static void FromArchiveEntries(List<ArchiveEntry> archiveEntries, Stream targetStream)
        {
            using (var writer = CreateWriter(targetStream))
            {
                foreach (var entry in archiveEntries)
                {
                    WriteEntry(writer, entry);
                }
            }
        }

        /// <summary>
        /// Creates a <see cref="TarWriter"/> which writes GNU-format tar archives. The GNU format
        /// is used because it transparently supports long file names (using <c>././@LongLink</c> entries),
        /// which are understood by <c>dpkg</c>, <c>rpm</c> and GNU <c>tar</c>.
        /// </summary>
        /// <param name="targetStream">
        /// The <see cref="Stream"/> which will hold the archive. The stream is left open when the writer is disposed of.
        /// </param>
        /// <returns>
        /// A new <see cref="TarWriter"/>. The tar trailer is written when the writer is disposed of.
        /// </returns>
        public static TarWriter CreateWriter(Stream targetStream)
        {
            return new TarWriter(targetStream, TarEntryFormat.Gnu, leaveOpen: true);
        }

        public static void WriteEntry(TarWriter writer, ArchiveEntry entry, Stream data = null)
        {
            var targetPath = entry.TargetPath;
            if (!targetPath.StartsWith(".", StringComparison.Ordinal))
            {
                targetPath = "." + targetPath;
            }

            var isDir = entry.Mode.HasFlag(LinuxFileMode.S_IFDIR);
            var isLink = !isDir && !string.IsNullOrWhiteSpace(entry.LinkTo);
            var isFile = !isDir && !isLink;

            TarEntryType type = isDir ? TarEntryType.Directory : isLink ? TarEntryType.SymbolicLink : TarEntryType.RegularFile;

            // The tar header has a dedicated field for the file type, so only the permission bits are stored in the mode.
            var tarEntry = new GnuTarEntry(type, targetPath)
            {
                Mode = (UnixFileMode)(entry.Mode & LinuxFileMode.PermissionsMask),
                ModificationTime = entry.Modified,
                Uid = 0,
                Gid = 0,
                UserName = entry.Owner ?? string.Empty,
                GroupName = entry.Group ?? string.Empty,
            };

            if (isLink)
            {
                tarEntry.LinkName = entry.LinkTo;
            }

            bool dispose = false;
            if (isFile)
            {
                if (data == null)
                {
                    dispose = true;
                    data = File.OpenRead(entry.SourceFilename);
                }

                tarEntry.DataStream = data;
            }

            try
            {
                writer.WriteEntry(tarEntry);
            }
            finally
            {
                if (dispose)
                {
                    data.Dispose();
                }
            }
        }
    }
}
