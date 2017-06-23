using Packaging.Targets.IO;
using Packaging.Targets.Rpm;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Packaging.Targets
{
    /// <summary>
    /// Creates a list of <see cref="ArchiveEntry"/> objects based on the publish directory of a .NET Core application.
    /// </summary>
    internal class ArchiveBuilder
    {
        private IFileAnalyzer fileAnayzer;
        private uint inode = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArchiveBuilder"/> class.
        /// </summary>
        public ArchiveBuilder()
        {
            fileAnayzer = new FileAnalyzer();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArchiveBuilder"/> class.
        /// </summary>
        /// <param name="analyzer">
        /// A <see cref="IFileAnalyzer"/> which can extract item metadata.
        /// </param>
        public ArchiveBuilder(IFileAnalyzer analyzer)
        {
            if (analyzer == null)
            {
                throw new ArgumentNullException(nameof(analyzer));
            }

            this.fileAnayzer = analyzer;
        }

        /// <summary>
        /// Extracts the <see cref="ArchiveEntry"/> objects from a CPIO file.
        /// </summary>
        /// <param name="file">
        /// The CPIO file from which to extract the entries.
        /// </param>
        /// <returns>
        /// A list of <see cref="ArchiveEntry"/> objects representing the data in the CPIO file.
        /// </returns>
        public Collection<ArchiveEntry> FromCpio(CpioFile file)
        {
            Collection<ArchiveEntry> value = new Collection<ArchiveEntry>();
            byte[] buffer = new byte[1024];
            byte[] fileHeader = null;

            while (file.Read())
            {
                fileHeader = null;

                ArchiveEntry entry = new ArchiveEntry()
                {
                    FileSize = file.EntryHeader.FileSize,
                    Group = "root",
                    Owner = "root",
                    Inode = file.EntryHeader.Ino,
                    Mode = file.EntryHeader.Mode,
                    Modified = file.EntryHeader.Mtime,
                    TargetPath = file.EntryName,
                    Type = ArchiveEntryType.None,
                    LinkTo = string.Empty,
                    Sha256 = Array.Empty<byte>(),
                    SourceFilename = null,
                    IsAscii = true
                };

                if (entry.Mode.HasFlag(LinuxFileMode.S_IFREG) && !entry.Mode.HasFlag(LinuxFileMode.S_IFLNK))
                {
                    using (var fileStream = file.Open())
                    using (var hasher = IncrementalHash.CreateHash(HashAlgorithmName.SHA256))
                    {
                        int read;

                        while (true)
                        {
                            read = fileStream.Read(buffer, 0, buffer.Length);

                            if (fileHeader == null)
                            {
                                fileHeader = new byte[read];
                                Buffer.BlockCopy(buffer, 0, fileHeader, 0, read);
                            }

                            hasher.AppendData(buffer, 0, read);
                            entry.IsAscii = entry.IsAscii && fileHeader.All(c => c < 128);

                            if (read < buffer.Length)
                            {
                                break;
                            }
                        }

                        entry.Sha256 = hasher.GetHashAndReset();
                    }

                    entry.Type = GetArchiveEntryType(fileHeader);
                }
                else if (entry.Mode.HasFlag(LinuxFileMode.S_IFLNK))
                {
                    using (var fileStrema = file.Open())
                    using (var reader = new StreamReader(fileStrema, Encoding.UTF8))
                    {
                        entry.LinkTo = reader.ReadToEnd();
                    }
                }
                else
                {
                    file.Skip();
                }

                if (entry.Mode.HasFlag(LinuxFileMode.S_IFDIR))
                {
                    entry.FileSize = 0x1000;
                }

                if (entry.TargetPath.StartsWith("."))
                {
                    entry.TargetPath = entry.TargetPath.Substring(1);
                }

                value.Add(entry);
            }

            return value;
        }

        /// <summary>
        /// Extracts the <see cref="ArchiveEntry"/> objects from a directory.
        /// </summary>
        /// <param name="file">
        /// The directory from which to extract the entries.
        /// </param>
        /// <returns>
        /// A list of <see cref="ArchiveEntry"/> objects representing the data in the directory.
        /// </returns>
        public Collection<ArchiveEntry> FromDirectory(string directory, string prefix)
        {
            Collection<ArchiveEntry> value = new Collection<ArchiveEntry>();
            AddDirectory(directory, prefix, value);
            return value;
        }

        protected void AddDirectory(string directory, string prefix, Collection<ArchiveEntry> value)
        {
            // Write out an entry for the current directory
            // (actually, this is _not_ done)
            ArchiveEntry directoryEntry = new ArchiveEntry()
            {
                FileSize = 0x00001000,
                Sha256 = Array.Empty<byte>(),
                Mode = LinuxFileMode.S_IXOTH | LinuxFileMode.S_IROTH | LinuxFileMode.S_IXGRP | LinuxFileMode.S_IRGRP | LinuxFileMode.S_IXUSR | LinuxFileMode.S_IWUSR | LinuxFileMode.S_IRUSR | LinuxFileMode.S_IFDIR,
                Modified = Directory.GetLastWriteTime(directory),
                Group = "root",
                Owner = "root",
                Inode = this.inode++
            };

            // The other in which the files appear in the cpio archive is important; if this is not respected xzdio
            // will report errors like:
            // error: unpacking of archive failed on file ./usr/share/quamotion/mscorlib.dll: cpio: Archive file not in header
            var entries = Directory.GetFileSystemEntries(directory).OrderBy(e => Directory.Exists(e) ? e + "/" : e, StringComparer.Ordinal).ToArray();

            foreach (var entry in entries)
            {
                if (File.Exists(entry))
                {
                    AddFile(entry, prefix, value);
                }
                else
                {
                    AddDirectory(entry, prefix + "/" + Path.GetFileName(entry), value);
                }
            }
        }

        protected void AddFile(string entry, string prefix, Collection<ArchiveEntry> value)
        {
            var fileName = Path.GetFileName(entry);

            byte[] fileHeader = null;
            byte[] hash = null;
            byte[] buffer = new byte[1024];
            bool isAscii = true;

            using (Stream fileStream = File.OpenRead(entry))
            {
                if (fileName.StartsWith(".") || fileStream.Length == 0)
                {
                    // Skip hidden and empty files - this would case rmplint errors.
                    return;
                }

                using (var hasher = IncrementalHash.CreateHash(HashAlgorithmName.SHA256))
                {
                    int read;

                    while (true)
                    {
                        read = fileStream.Read(buffer, 0, buffer.Length);

                        if (fileHeader == null)
                        {
                            fileHeader = new byte[read];
                            Buffer.BlockCopy(buffer, 0, fileHeader, 0, read);
                        }

                        hasher.AppendData(buffer, 0, read);
                        isAscii = isAscii && buffer.All(c => c < 128);

                        if (read < buffer.Length)
                        {
                            break;
                        }
                    }

                    hash = hasher.GetHashAndReset();
                }

                // Only support ELF32 and ELF64 colors; otherwise default to BLACK.
                ArchiveEntryType entryType = GetArchiveEntryType(fileHeader);

                var mode = LinuxFileMode.S_IROTH | LinuxFileMode.S_IRGRP | LinuxFileMode.S_IRUSR | LinuxFileMode.S_IFREG;

                if (entryType == ArchiveEntryType.Executable32 || entryType == ArchiveEntryType.Executable64)
                {
                    mode |= LinuxFileMode.S_IXOTH | LinuxFileMode.S_IXGRP | LinuxFileMode.S_IWUSR | LinuxFileMode.S_IXUSR;
                }

                string name = prefix + "/" + fileName;
                string linkTo = string.Empty;

                if (mode.HasFlag(LinuxFileMode.S_IFLNK))
                {
                    // Find the link text
                    int stringEnd = 0;

                    while (stringEnd < fileHeader.Length - 1 && fileHeader[stringEnd] != 0)
                    {
                        stringEnd++;
                    }

                    linkTo = Encoding.UTF8.GetString(fileHeader, 0, stringEnd + 1);
                    hash = new byte[] { };
                }

                ArchiveEntry archiveEntry = new ArchiveEntry()
                {
                    FileSize = (uint)fileStream.Length,
                    Group = "root",
                    Owner = "root",
                    Modified = File.GetLastAccessTimeUtc(entry),
                    SourceFilename = entry,
                    TargetPath = name,
                    Sha256 = hash,
                    Type = entryType,
                    LinkTo = linkTo,
                    Inode = this.inode++,
                    IsAscii = isAscii
                };

                value.Add(archiveEntry);
            }
        }

        private ArchiveEntryType GetArchiveEntryType(byte[] fileHeader)
        {
            if (ElfFile.IsElfFile(fileHeader))
            {
                ElfHeader elfHeader = ElfFile.ReadHeader(fileHeader);

                if (elfHeader.@class == ElfClass.Elf32)
                {
                    return ArchiveEntryType.Executable32;
                }
                else
                {
                    return ArchiveEntryType.Executable64;
                }
            }

            return ArchiveEntryType.None;
        }
    }
}
