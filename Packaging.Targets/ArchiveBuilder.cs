using Microsoft.Build.Framework;
using Packaging.Targets.IO;
using Packaging.Targets.Rpm;
using System;
using System.Collections.Generic;
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
            this.fileAnayzer = new FileAnalyzer();
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
        public List<ArchiveEntry> FromCpio(CpioFile file)
        {
            List<ArchiveEntry> value = new List<ArchiveEntry>();
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
                    Mode = file.EntryHeader.FileMode,
                    Modified = file.EntryHeader.LastModified,
                    TargetPath = file.FileName,
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

                    entry.Type = this.GetArchiveEntryType(fileHeader);
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

        public Collection<ArchiveEntry> FromLinuxFolders(ITaskItem[] metadata)
        {
            Collection<ArchiveEntry> value = new Collection<ArchiveEntry>();

            // This can be null if the user did not define any folders.
            // In that case: nothing to do.
            if (metadata != null)
            {
                foreach (var folder in metadata)
                {
                    var path = folder.ItemSpec.Replace("\\", "/");

                    // Write out an entry for the current directory
                    // (actually, this is _not_ done)
                    ArchiveEntry directoryEntry = new ArchiveEntry()
                    {
                        FileSize = 0x00001000,
                        Sha256 = Array.Empty<byte>(),
                        Mode = LinuxFileMode.S_IXOTH | LinuxFileMode.S_IROTH | LinuxFileMode.S_IXGRP | LinuxFileMode.S_IRGRP | LinuxFileMode.S_IXUSR | LinuxFileMode.S_IWUSR | LinuxFileMode.S_IRUSR | LinuxFileMode.S_IFDIR,
                        Modified = DateTime.Now,
                        Group = folder.GetGroup(),
                        Owner = folder.GetOwner(),
                        Inode = this.inode++,
                        TargetPath = path,
                        LinkTo = string.Empty,
                        RemoveOnUninstall = folder.GetRemoveOnUninstall()
                    };

                    value.Add(directoryEntry);
                }
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
        public List<ArchiveEntry> FromDirectory(string directory, string prefix, ITaskItem[] metadata)
        {
            List<ArchiveEntry> value = new List<ArchiveEntry>();
            this.AddDirectory(directory, string.Empty, prefix, value, metadata);
            return value;
        }

        protected void AddDirectory(string directory, string relativePath, string prefix, List<ArchiveEntry> value, ITaskItem[] metadata)
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
                    this.AddFile(entry, relativePath + Path.GetFileName(entry), prefix, value, metadata);
                }
                else
                {
                    this.AddDirectory(entry, relativePath + Path.GetFileName(entry) + "/", prefix + "/" + Path.GetFileName(entry), value, metadata);
                }
            }
        }

        protected void AddFile(string entry, string relativePath, string prefix, List<ArchiveEntry> value, ITaskItem[] metadata)
        {
            var fileName = Path.GetFileName(entry);

            byte[] fileHeader = null;
            byte[] hash = null;
            byte[] md5hash = null;
            byte[] buffer = new byte[1024];
            bool isAscii = true;

            var fileMetadata = metadata.SingleOrDefault(m => m.IsPublished() && string.Equals(relativePath, m.GetPublishedPath()));

            using (Stream fileStream = File.OpenRead(entry))
            {
                if (fileName.StartsWith(".") || fileStream.Length == 0)
                {
                    // Skip hidden and empty files - this would case rmplint errors.
                    return;
                }

                using (var hasher = IncrementalHash.CreateHash(HashAlgorithmName.SHA256))
                using (var md5hasher = IncrementalHash.CreateHash(HashAlgorithmName.MD5))
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
                        md5hasher.AppendData(buffer, 0, read);
                        isAscii = isAscii && buffer.All(c => c < 128);

                        if (read < buffer.Length)
                        {
                            break;
                        }
                    }

                    hash = hasher.GetHashAndReset();
                    md5hash = md5hasher.GetHashAndReset();
                }

                // Only support ELF32 and ELF64 colors; otherwise default to BLACK.
                ArchiveEntryType entryType = this.GetArchiveEntryType(fileHeader);

                var mode = LinuxFileMode.S_IROTH | LinuxFileMode.S_IRGRP | LinuxFileMode.S_IRUSR | LinuxFileMode.S_IFREG;

                if (entryType == ArchiveEntryType.Executable32 || entryType == ArchiveEntryType.Executable64)
                {
                    mode |= LinuxFileMode.S_IXOTH | LinuxFileMode.S_IXGRP | LinuxFileMode.S_IWUSR | LinuxFileMode.S_IXUSR;
                }

                // If a Linux path has been specified, use that one, else, use the default one based on the prefix
                // + current file name.
                string name = fileMetadata?.GetLinuxPath();

                if (name == null)
                {
                    name = prefix + "/" + fileName;
                }

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
                    Group = fileMetadata.GetGroup(),
                    Owner = fileMetadata.GetOwner(),
                    Modified = File.GetLastAccessTimeUtc(entry),
                    SourceFilename = entry,
                    TargetPath = name,
                    Sha256 = hash,
                    Md5Hash = md5hash,
                    Type = entryType,
                    LinkTo = linkTo,
                    Inode = this.inode++,
                    IsAscii = isAscii,
                    Mode = mode
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
