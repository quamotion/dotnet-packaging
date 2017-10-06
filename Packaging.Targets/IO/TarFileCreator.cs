using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

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
            Align(stream);
            var trailer = new byte[1024];
            stream.Write(trailer, 0, trailer.Length);
        }

        static void Align(Stream stream)
        {
            var spos = stream.Position % 512;
            if(spos == 0)
                return;
            var align = new byte[512 -spos];
            stream.Write(align, 0, align.Length);
        }

        public static void WriteEntry(Stream stream, TarHeader header, Stream data)
        {
            header.Checksum = header.ComputeChecksum();
            stream.WriteStruct(header);
            Align(stream);
            data.CopyTo(stream);
            Align(stream);
        }
        
        public static void WriteEntry(Stream stream, ArchiveEntry entry, Stream data = null)
        {
            var targetPath = entry.TargetPath;
            if (!targetPath.StartsWith("."))
            {
                targetPath = "." + targetPath;
            }
            var isDir = entry.Mode.HasFlag(LinuxFileMode.S_IFDIR);
            var isLink = !isDir && !string.IsNullOrWhiteSpace(entry.LinkTo);
            var isFile = !isDir && !isLink;
            var type = isFile
                ? TarTypeFlag.RegType
                : isDir
                    ? TarTypeFlag.DirType
                    : TarTypeFlag.LnkType;

            
            bool dispose = false;
            if (data == null)
            {
                if (isFile)
                {
                    dispose = true;
                    data = File.OpenRead(entry.SourceFilename);
                }
                else
                    data = new MemoryStream();
            }
            try
            {
                var hdr = new TarHeader()
                {
                    FileMode = entry.Mode,
                    DevMajor = null,
                    DevMinor = null,
                    FileName = targetPath,
                    FileSize = (uint) data.Length,
                    GroupId = 0,
                    UserId = 0,
                    GroupName = entry.Group,
                    LinkName = "",
                    Prefix = "",
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
                if(dispose)
                    data.Dispose();
            }
        }

    }
}