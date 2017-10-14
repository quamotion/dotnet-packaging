//-----------------------------------------------------------------------
// <copyright file="XarFile.cs" company="Quamotion">
//     Copyright (c) 2016 Quamotion. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Packaging.Targets.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Packaging.Targets.Pkg
{
    /// <summary>
    /// Xar (Extensible ARchives) is a compression file format used by Apple.
    /// The file consists of three sections, the header, the table of contents and the heap.
    /// The header is a normal C struct, the table of contents is a zlib-compressed XML
    /// document and the heap contains the compressed data.
    /// </summary>
    /// <seealso href="https://github.com/mackyle/xar/wiki/xarformat"/>
    internal class XarFile : IDisposable, IEntryContainer
    {
        /// <summary>
        /// The <see cref="Stream"/> around which this <see cref="XarFile"/> wraps.
        /// </summary>
        private readonly Stream stream;

        /// <summary>
        /// Indicates whether to close <see cref="stream"/> when we are disposed of.
        /// </summary>
        private readonly bool leaveOpen;

        /// <summary>
        /// The table of contents, listing the entries compressed in this archive.
        /// </summary>
        private readonly XDocument toc;

        /// <summary>
        /// The entries contained in the table of contents.
        /// </summary>
        private readonly FileEntry[] files;

        /// <summary>
        /// The start of the heap.
        /// </summary>
        private readonly ulong heapStart;

        /// <summary>
        /// Initializes a new instance of the <see cref="XarFile"/> class.
        /// </summary>
        /// <param name="stream">
        /// The <see cref="Stream"/> which represents the XAR archive.
        /// </param>
        /// <param name="leaveOpen">
        /// Indicates whether to close <paramref name="stream"/> when the <see cref="XarFile"/>
        /// is disposed of.
        /// </param>
        public XarFile(Stream stream, bool leaveOpen)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            this.leaveOpen = leaveOpen;
            this.stream = stream;

            var header = stream.ReadStruct<XarHeader>();

            // Basic validation
            if (header.Signature != XarHeader.Magic)
            {
                throw new InvalidDataException("The XAR header signature is incorrect");
            }

            if (header.Version != XarHeader.CurrentVersion)
            {
                throw new InvalidDataException("The XAR header version is incorrect");
            }

            // Read the digest name, if available.
            int messageDigestNameLength = header.Size - 28;
            byte[] messageDigestNameBytes = new byte[messageDigestNameLength];
            stream.Read(messageDigestNameBytes, 0, messageDigestNameLength);
            string messageDigestName = Encoding.UTF8.GetString(messageDigestNameBytes);

            // Read the table of contents
            using (SubStream compressedTocStream = new SubStream(this.stream, this.stream.Position, (long)header.TocLengthCompressed, leaveParentOpen: true))
            using (DeflateStream decompressedTocStream = new DeflateStream(compressedTocStream, CompressionMode.Decompress, leaveOpen: true))
            {
                // Make sure we're reading zlib-compressed data
                this.EnsureGzip(compressedTocStream);

                // Read the TOC
                this.toc = XDocument.Load(decompressedTocStream);
                this.files = (from file
                             in this.toc
                             .Element("xar")
                             .Element("toc")
                             .Elements("file")
                              select new FileEntry(file)).ToArray();
            }

            this.heapStart = header.Size + header.TocLengthCompressed;
        }

        /// <summary>
        /// Gets all the entries in this <see cref="XarFile"/>.
        /// </summary>
        public IEnumerable<FileEntry> Entries
        {
            get
            {
                return this.files;
            }
        }

        public FileEntry GetEntry(string entryName)
        {
            if (entryName == null)
            {
                throw new ArgumentNullException(nameof(entryName));
            }

            var parts = entryName.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            var current = (IEntryContainer)this;

            foreach (var part in parts)
            {
                current = current.Entries.SingleOrDefault(e => e.Name == part);

                if (current == null)
                {
                    return null;
                }
            }

            return current as FileEntry;
        }

        /// <summary>
        /// Opens an entry in the <see cref="XarFile"/>.
        /// </summary>
        /// <param name="entryName">
        /// The name of the entry to open.
        /// </param>
        /// <returns>
        /// A <see cref="Stream"/> which represents the entry.
        /// </returns>
        public Stream Open(string entryName)
        {
            if (entryName == null)
            {
                throw new ArgumentOutOfRangeException(nameof(entryName));
            }

            // Fetch the entry details
            var entry = this.GetEntry(entryName);

            if (entry == null)
            {
                throw new FileNotFoundException();
            }

            if (entry.Type != "file")
            {
                throw new InvalidOperationException();
            }

            // Construct a substream which maps to the compressed data
            var start = this.heapStart + entry.DataOffset;

            SubStream substream = new SubStream(this.stream, (long)start, (long)entry.DataLength, leaveParentOpen: true);

            // Special case: uncompressed data can be returned 'as is'
            if (entry.Encoding == "application/octet-stream")
            {
                return substream;
            }

            // Make sure we're reading gzip-compressed data
            if (entry.Encoding != "application/x-gzip")
            {
                throw new InvalidDataException("Only gzip-compressed data is supported");
            }

            this.EnsureGzip(substream);

            // Create a new deflate stream, and return it.
            DeflateStream gzipStream = new DeflateStream(substream, CompressionMode.Decompress, leaveOpen: false);

            return gzipStream;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (!this.leaveOpen)
            {
                this.stream.Dispose();
            }
        }

        /// <summary>
        /// Reads the header of a gzip-compressed file, and throws an exception if the file
        /// is not in the correct format. Leaves the <see cref="Stream"/> at the first byte after
        /// the header, so that it can be opened using a <see cref="DeflateStream"/>.
        /// </summary>
        /// <param name="stream">
        /// The <see cref="Stream"/> for which to verify the format.
        /// </param>
        private void EnsureGzip(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            // Read the CMF and FLG values of the zlib-compressed stream. This format is
            // specified in https://tools.ietf.org/html/rfc1950
            var cmf = stream.ReadByte();
            if (cmf != 0x78)
            {
                throw new InvalidDataException("The data is not encoded using zlib compression");
            }

            var flg = stream.ReadByte();
            if (flg != 0x01 && flg != 0x9c && flg != 0xda)
            {
                throw new InvalidDataException("The data is not encoded using zlib compression");
            }
        }
    }
}
