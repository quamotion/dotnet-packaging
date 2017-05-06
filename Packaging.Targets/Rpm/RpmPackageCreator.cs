using Packaging.Targets.IO;
using System;
using System.Collections.ObjectModel;
using System.Security.Cryptography;
using System.Text;

namespace Packaging.Targets.Rpm
{
    /// <summary>
    /// Supports creating <see cref="RpmPackage"/> objects based on a <see cref="CpioFile"/>.
    /// </summary>
    internal class RpmPackageCreator
    {
        /// <summary>
        /// The <see cref="IFileAnalyzer"/> which analyzes the files in this package and provides required
        /// metadata for the files.
        /// </summary>
        private readonly IFileAnalyzer analyzer;

        /// <summary>
        /// Initializes a new instance of the <see cref="RpmPackageCreator"/> class.
        /// </summary>
        public RpmPackageCreator()
            : this(new FileAnalyzer())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RpmPackageCreator"/> class.
        /// </summary>
        /// <param name="analyzer">
        /// An <see cref="IFileAnalyzer"/> used to analyze the files in this package and provides required
        /// meatata for the files.
        /// </param>
        public RpmPackageCreator(IFileAnalyzer analyzer)
        {
            if (analyzer == null)
            {
                throw new ArgumentNullException(nameof(analyzer));
            }

            this.analyzer = analyzer;
        }

        /// <summary>
        /// Creates the metadata for all files in the <see cref="CpioFile"/>.
        /// </summary>
        /// <param name="payload">
        /// The payload for which to generate the metadata.
        /// </param>
        /// <returns>
        /// A <see cref="Collection{RpmFile}"/> which contains all the metadata.
        /// </returns>
        public Collection<RpmFile> CreateFiles(CpioFile payload)
        {
            Collection<RpmFile> files = new Collection<RpmFile>();

            while (payload.Read())
            {
                byte[] hash;
                byte[] buffer = new byte[1024];
                byte[] header = null;
                int read = 0;

                using (var stream = payload.Open())
                using (var hasher = IncrementalHash.CreateHash(HashAlgorithmName.SHA256))
                {
                    while (true)
                    {
                        read = stream.Read(buffer, 0, buffer.Length);

                        if (header == null)
                        {
                            header = new byte[read];
                            Buffer.BlockCopy(buffer, 0, header, 0, read);
                        }

                        hasher.AppendData(buffer, 0, read);

                        if (read < buffer.Length)
                        {
                            break;
                        }
                    }

                    hash = hasher.GetHashAndReset();
                }

                string fileName = payload.EntryName;
                int fileSize = (int)payload.EntryHeader.FileSize;

                if (fileName.StartsWith("."))
                {
                    fileName = fileName.Substring(1);
                }

                string linkTo = string.Empty;

                if (payload.EntryHeader.Mode.HasFlag(LinuxFileMode.S_IFLNK))
                {
                    // Find the link text
                    int stringEnd = 0;

                    while (stringEnd < header.Length - 1 && header[stringEnd] != 0)
                    {
                        stringEnd++;
                    }

                    linkTo = Encoding.UTF8.GetString(header, 0, stringEnd + 1);
                    hash = new byte[] { };
                }
                else if (payload.EntryHeader.Mode.HasFlag(LinuxFileMode.S_IFDIR))
                {
                    fileSize = 0x00001000;
                    hash = new byte[] { };
                }

                RpmFile file = new RpmFile()
                {
                    Size = fileSize,
                    Mode = payload.EntryHeader.Mode,
                    Rdev = (short)payload.EntryHeader.RDevMajor,
                    ModifiedTime = payload.EntryHeader.Mtime,
                    MD5Hash = hash,
                    LinkTo = linkTo,
                    Flags = this.analyzer.DetermineFlags(fileName, payload.EntryHeader, header),
                    UserName = "root",
                    GroupName = "root",
                    VerifyFlags = RpmVerifyFlags.RPMVERIFY_ALL,
                    Device = 1,
                    Inode = (int)payload.EntryHeader.Ino,
                    Lang = "",
                    Color = this.analyzer.DetermineColor(fileName, payload.EntryHeader, header),
                    Class = this.analyzer.DetermineClass(fileName, payload.EntryHeader, header),
                    Requires = this.analyzer.DetermineRequires(fileName, payload.EntryHeader, header),
                    Provides = this.analyzer.DetermineProvides(fileName, payload.EntryHeader, header),
                    Name = fileName
                };

                files.Add(file);
            }

            return files;
        }
    }
}
