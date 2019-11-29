using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Packaging.Targets.IO
{
    internal static class TarFileCreator
    {
        /// <summary>
        /// Generates a <see cref="CpioFile"/> based on a list of <see cref="ArchiveEntry"/>
        /// values.
        /// </summary>
        /// <param name="archiveEntries">
        /// The values based on which to generate the <see cref="CpioFile"/>.
        /// </param>
        /// <param name="targetStream">
        /// The <see cref="Stream"/> which will hold the <see cref="CpioFile"/>.
        /// </param>
        public static void FromArchiveEntries(List<ArchiveEntry> archiveEntries, Stream targetStream)
        {
            using (TarFile cpioFile = new TarFile(targetStream, leaveOpen: true))
            {
                foreach (var entry in archiveEntries)
                {
                    WriteEntry(targetStream, entry);
                }

                WriteTrailer(targetStream);
            }
        }

        public static void WriteTrailer(Stream stream)
        {
            // The stream should already be aligned; as it is aligned after every entry.
            // As a safety measure, for streams which can report on .Position, align the
            // stream again.
            if (stream.CanSeek)
            {
                Align(stream);
            }

            var trailer = new byte[1024];
            stream.Write(trailer, 0, trailer.Length);
        }

        public static void WriteEntry(Stream stream, TarHeader header, Stream data)
        {
            header.Checksum = header.ComputeChecksum();
            int written = stream.WriteStruct(header);
            Align(stream, written);
            data.CopyTo(stream);
            Align(stream, data.Length);
        }

        public static void WriteEntry(Stream stream, ArchiveEntry entry, Stream data = null)
        {
            var targetPath = entry.TargetPath;
            if (!targetPath.StartsWith("."))
            {
                targetPath = "." + targetPath;
            }

            // Handle long file names (> 99 characters). If this is the case, add a "././@LongLink" pseudo-entry
            // which contains the full name.
            if (targetPath.Length > 99)
            {
                // Must include a trailing \0
                var nameLength = Encoding.UTF8.GetByteCount(targetPath);
                byte[] entryName = new byte[nameLength + 1];

                Encoding.UTF8.GetBytes(targetPath, 0, targetPath.Length, entryName, 0);

                ArchiveEntry nameEntry = new ArchiveEntry()
                {
                    Mode = entry.Mode,
                    Modified = entry.Modified,
                    TargetPath = "././@LongLink",
                    Owner = entry.Owner,
                    Group = entry.Group
                };

                using (MemoryStream nameStream = new MemoryStream(entryName))
                {
                    WriteEntry(stream, nameEntry, nameStream);
                }

                targetPath = targetPath.Substring(0, 99);
            }

            var isDir = entry.Mode.HasFlag(LinuxFileMode.S_IFDIR);
            var isLink = !isDir && !string.IsNullOrWhiteSpace(entry.LinkTo);
            var isFile = !isDir && !isLink;

            TarTypeFlag type;

            if (entry.TargetPath == "././@LongLink")
            {
                type = TarTypeFlag.LongName;
            }
            else if (isFile)
            {
                type = TarTypeFlag.RegType;
            }
            else if (isDir)
            {
                type = TarTypeFlag.DirType;
            }
            else
            {
                type = TarTypeFlag.LnkType;
            }

            bool dispose = false;
            if (data == null)
            {
                if (isFile)
                {
                    dispose = true;
                    data = File.OpenRead(entry.SourceFilename);
                }
                else
                {
                    data = new MemoryStream();
                }
            }

            try
            {
                var hdr = new TarHeader()
                {
                    // No need to set the file type, the tar header has a special field for that.
                    FileMode = entry.Mode & LinuxFileMode.PermissionsMask,
                    DevMajor = null,
                    DevMinor = null,
                    FileName = targetPath,
                    FileSize = (uint)data.Length,
                    GroupId = 0,
                    UserId = 0,
                    GroupName = entry.Group,
                    LinkName = string.Empty,
                    Prefix = string.Empty,
                    TypeFlag = type,
                    UserName = entry.Owner,
                    Version = null,
                    LastModified = entry.Modified,
                    Magic = "ustar"
                };
                WriteEntry(stream, hdr, data);
            }
            finally
            {
                if (dispose)
                {
                    data.Dispose();
                }
            }
        }

        private static void Align(Stream stream)
        {
            Align(stream, stream.Position);
        }

        private static void Align(Stream stream, long position)
        {
            var spos = position % 512;
            if (spos == 0)
            {
                return;
            }

            var align = new byte[512 - spos];
            stream.Write(align, 0, align.Length);
        }
    }
}