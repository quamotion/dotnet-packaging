using System;
using System.IO;

namespace Packaging.Targets.IO
{
    /// <summary>
    /// Represents a <c>Tar</c> archive.
    /// </summary>
    public class TarFile : ArchiveFile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TarFile"/> class.
        /// </summary>
        /// <param name="stream">
        /// A <see cref="Stream"/> which represents the tar file.
        /// </param>
        /// <param name="leaveOpen">
        /// <see langword="true"/> to leave the underlying <paramref name="stream"/> open when this <see cref="TarFile"/>
        /// is disposed of; otherwise, <see langword="false"/>.
        /// </param>
        public TarFile(Stream stream, bool leaveOpen)
            : base(stream, leaveOpen)
        {
        }

        private TarHeader entryHeader;

        /// <inheritdoc/>
        public override bool Read()
        {
            if (this.EntryStream != null)
            {
                this.EntryStream.Dispose();
            }

            this.Align(512);

            this.entryHeader = this.Stream.ReadStruct<TarHeader>();
            this.FileHeader = this.entryHeader;
            this.FileName = this.entryHeader.FileName;

            // There are two empty blocks at the end of the file.
            if (this.entryHeader.Magic == string.Empty)
            {
                return false;
            }

            if (this.entryHeader.Magic != "ustar")
            {
                throw new InvalidDataException("The magic for the file entry is invalid");
            }

            this.Align(512);

            // TODO: Validate Checksum
            this.EntryStream = new SubStream(this.Stream, this.Stream.Position, this.entryHeader.FileSize, leaveParentOpen: true);

            return true;
        }
    }
}
