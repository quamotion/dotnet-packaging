using System;
using System.IO;
using System.Text;

namespace Packaging.Targets.IO
{
    /// <summary>
    /// Represents an <c>ar</c> archive, the format used by Debian installer packages.
    /// </summary>
    /// <seealso href="https://en.wikipedia.org/wiki/Ar_%28Unix%29"/>
    public class ArFile : ArchiveFile
    {
        /// <summary>
        /// The magic used to identify <c>ar</c> files.
        /// </summary>
        private const string Magic = "!<arch>\n";

        /// <summary>
        /// Tracks whether the magic has been read or not.
        /// </summary>
        private bool magicRead;

        /// <summary>
        /// The header of the current entry.
        /// </summary>
        private ArHeader entryHeader;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArFile"/> class.
        /// </summary>
        /// <param name="stream">
        /// A <see cref="Stream"/> which represents the CPIO data.
        /// </param>
        /// <param name="leaveOpen">
        /// <see langword="true"/> to leave the underlying <paramref name="stream"/> open when this <see cref="ArFile"/>
        /// is disposed of; otherwise, <see langword="false"/>.
        /// </param>
        public ArFile(Stream stream, bool leaveOpen)
            : base(stream, leaveOpen)
        {
        }

        /// <inheritdoc/>
        public override bool Read()
        {
            this.EnsureMagicRead();

            if (this.EntryStream != null)
            {
                this.EntryStream.Dispose();
            }

            this.Align(2);

            if(this.Stream.Position == this.Stream.Length)
            {
                return false;
            }

            this.entryHeader = this.Stream.ReadStruct<ArHeader>();
            this.FileHeader = this.entryHeader;
            this.FileName = this.entryHeader.FileName;

            if (this.entryHeader.EndChar != "`\n")
            {
                throw new InvalidDataException("The magic for the file entry is invalid");
            }
            
            this.EntryStream = new SubStream(this.Stream, this.Stream.Position, this.entryHeader.FileSize, leaveParentOpen: true);

            return true;
        }

        /// <summary>
        /// Reads the magic if it has not been read previously.
        /// </summary>
        protected void EnsureMagicRead()
        {
            if (!this.magicRead)
            {
                byte[] buffer = new byte[Magic.Length];
                this.Stream.Read(buffer, 0, buffer.Length);
                var magic = Encoding.ASCII.GetString(buffer);

                if (!string.Equals(magic, Magic, StringComparison.Ordinal))
                {
                    throw new InvalidDataException("The .ar file did not start with the expected magic");
                }

                this.magicRead = true;
            }
        }
    }
}
