//-----------------------------------------------------------------------
// <copyright file="FileEntry.cs" company="Quamotion">
//     Copyright (c) 2016 Quamotion. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Packaging.Targets.Pkg
{
    /// <summary>
    /// Represents a file inside a xar archive.
    /// </summary>
    internal class FileEntry : IEntryContainer
    {
        /// <summary>
        /// The <see cref="XElement"/> in the TOC of the XAR file which contains information about
        /// the entry.
        /// </summary>
        private readonly XElement element;

        private FinderCreateTime finderCreateTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileEntry"/> class.
        /// </summary>
        /// <param name="element">
        /// The <see cref="XElement"/> in the TOC of the XAR file which contains information about
        /// the entry.
        /// </param>
        public FileEntry(XElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            this.element = element;
            this.Entries =
                from file
                in this.element.Elements("file")
                select new FileEntry(file);
        }

        public IEnumerable<FileEntry> Entries
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets an Id which uniquely identifies the entry.
        /// </summary>
        public int Id
        {
            get { return (int)this.element.Attribute("id"); }
        }

        public FinderCreateTime FinderCreateTime
        {
            get
            {
                if (this.finderCreateTime == null)
                {
                    this.finderCreateTime = new FinderCreateTime(this.element.Element("FinderCreateTime"));
                }

                return this.finderCreateTime;
            }
        }

        public DateTime CreateTime
        {
            get { return (DateTime)this.element.Element("ctime"); }
        }

        public DateTime ModifiedTime
        {
            get { return (DateTime)this.element.Element("mtime"); }
        }

        public DateTime ArchivedTime
        {
            get { return (DateTime)this.element.Element("atime"); }
        }

        public string Group
        {
            get { return (string)this.element.Element("group"); }
        }

        public int GroupId
        {
            get { return (int)this.element.Element("gid"); }
        }

        public string User
        {
            get { return (string)this.element.Element("user"); }
        }

        public int UserId
        {
            get { return (int)this.element.Element("uid"); }
        }

        public int Mode
        {
            get { return (int)this.element.Element("mode"); }
        }

        public int DeviceNumber
        {
            get { return (int)this.element.Element("deviceno"); }
        }

        public int Inode
        {
            get { return (int)this.element.Element("inode"); }
        }

        /// <summary>
        /// Gets the name of the entry.
        /// </summary>
        public string Name
        {
            get { return this.element.Element("name").Value; }
        }

        /// <summary>
        /// Gets the type of the entry, such as <c>file</c> for a file entry.
        /// </summary>
        public string Type
        {
            get { return this.element.Element("type").Value; }
        }

        /// <summary>
        /// Gets the uncompressed size of the entry. This is the size of the uncompressed data, together
        /// with the extended attributes.
        /// </summary>
        public ulong Size
        {
            get { return ulong.Parse(this.element.Element("size").Value); }
        }

        /// <summary>
        /// Gets the offset of the compressed data in the XAR archive, relative to the start of the
        /// heap.
        /// </summary>
        public ulong DataOffset
        {
            get { return ulong.Parse(this.element.Element("data").Element("offset").Value); }
        }

        /// <summary>
        /// Gets the size of the uncompressed data.
        /// </summary>
        public ulong DataSize
        {
            get { return ulong.Parse(this.element.Element("data").Element("size").Value); }
        }

        /// <summary>
        /// Gets the length of the compressed data in the XAR archive.
        /// </summary>
        public ulong DataLength
        {
            get { return ulong.Parse(this.element.Element("data").Element("length").Value); }
        }

        /// <summary>
        /// Gets the encoding used to compress the data. Currently the only supported values are
        /// <c>application/x-gzip</c> for Deflate encoding and <c>application/octet-stream</c>
        /// for uncompressed data.
        /// </summary>
        public string Encoding
        {
            get { return (string)this.element.Element("data").Element("encoding").Attribute("style"); }
        }

        /// <summary>
        /// Gets the checksum of the uncompressed data.
        /// </summary>
        public string ExtractedChecksum
        {
            get { return this.element.Element("data").Element("extracted-checksum").Value; }
        }

        /// <summary>
        /// Gets the algorithm used to calculate the checksum of the uncompressed data.
        /// </summary>
        public string ExtractedChecksumStyle
        {
            get { return (string)this.element.Element("data").Element("extracted-checksum").Attribute("style"); }
        }

        /// <summary>
        /// Gets the checksum of the compressed data.
        /// </summary>
        public string ArchivedChecksum
        {
            get { return this.element.Element("data").Element("archived-checksum").Value; }
        }

        /// <summary>
        /// Gets the algorithm used to calculate the checksum of the compressed data.
        /// </summary>
        public string ArchivedChecksumStyle
        {
            get { return (string)this.element.Element("data").Element("archived-checksum").Attribute("style"); }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return this.Name;
        }
    }
}
