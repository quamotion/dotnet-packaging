using Packaging.Targets.Rpm;
using System;
using System.Collections.Generic;
using System.IO;

namespace Packaging.Targets.IO
{
    /// <summary>
    /// Supports generating CPIO files.
    /// </summary>
    public class CpioFileCreator
    {
        /// <summary>
        /// The <see cref="IFileAnalyzer"/> used to fetch file metadata.
        /// </summary>
        private IFileAnalyzer fileAnayzer;

        /// <summary>
        /// Initializes a new instance of the <see cref="CpioFileCreator"/> class.
        /// </summary>
        public CpioFileCreator()
        {
            this.fileAnayzer = new FileAnalyzer();
        }

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
        public void FromArchiveEntries(List<ArchiveEntry> archiveEntries, Stream targetStream)
        {
            using (CpioFile cpioFile = new CpioFile(targetStream, leaveOpen: true))
            {
                foreach (var entry in archiveEntries)
                {
                    if (entry.Mode.HasFlag(LinuxFileMode.S_IFDIR))
                    {
                        this.AddDirectory(entry, cpioFile);
                    }
                    else
                    {
                        this.AddFile(entry, cpioFile);
                    }
                }

                cpioFile.WriteTrailer();
            }
        }

        /// <summary>
        /// Adds a directory entry to the <see cref="CpioFile"/>.
        /// </summary>
        /// <param name="entry">
        /// The <see cref="ArchiveEntry"/> which represents the directory.
        /// </param>
        /// <param name="cpioFile">
        /// The <see cref="CpioFile"/> to which to add the directory entry.
        /// </param>
        public void AddDirectory(ArchiveEntry entry, CpioFile cpioFile)
        {
            // Write out an entry for the current directory
            CpioHeader directoryHeader = new CpioHeader()
            {
                Check = 0,
                DevMajor = 1,
                DevMinor = 0,
                FileSize = 0,
                Gid = 0,
                Ino = entry.Inode,
                FileMode = entry.Mode,
                LastModified = entry.Modified,
                Nlink = 1,
                RDevMajor = 0,
                RDevMinor = 0,
                Signature = "070701",
                Uid = 0,
                NameSize = 0
            };

            var targetPath = entry.TargetPath;
            if (!targetPath.StartsWith("."))
            {
                targetPath = "." + targetPath;
            }

            cpioFile.Write(directoryHeader, targetPath, new MemoryStream(Array.Empty<byte>()));
        }

        /// <summary>
        /// Adds a file entry to a <see cref="CpioFile"/>.
        /// </summary>
        /// <param name="entry">
        /// The file entry to add.
        /// </param>
        /// <param name="cpioFile">
        /// The <see cref="CpioFile"/> to which to add the entry.
        /// </param>
        public void AddFile(ArchiveEntry entry, CpioFile cpioFile)
        {
            var targetPath = entry.TargetPath;

            if (!targetPath.StartsWith("."))
            {
                targetPath = "." + targetPath;
            }

            using (Stream fileStream = File.OpenRead(entry.SourceFilename))
            {
                CpioHeader cpioHeader = new CpioHeader()
                {
                    Check = 0,
                    DevMajor = 1,
                    DevMinor = 0,
                    FileSize = entry.FileSize,
                    Gid = 0, // root
                    Uid = 0, // root
                    Ino = entry.Inode,
                    FileMode = entry.Mode,
                    LastModified = entry.Modified,
                    NameSize = (uint)entry.TargetPath.Length + 1,
                    Nlink = 1,
                    RDevMajor = 0,
                    RDevMinor = 0,
                    Signature = "070701",
                };

                cpioFile.Write(cpioHeader, targetPath, fileStream);
            }
        }
    }
}
