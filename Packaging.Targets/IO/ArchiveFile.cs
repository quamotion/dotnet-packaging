using System;
using System.IO;

namespace Packaging.Targets.IO
{
    /// <summary>
    /// Base class for archive files, such as CPIO files or ar files.
    /// </summary>
    public abstract class ArchiveFile : IDisposable
    {
        /// <summary>
        /// The <see cref="Stream"/> around which this <see cref="CpioFile"/> wraps.
        /// </summary>
        private readonly Stream stream;

        /// <summary>
        /// Indicates whether <see cref="stream"/> should be disposed of when this <see cref="CpioFile"/> is disposed of.
        /// </summary>
        private readonly bool leaveOpen;

        /// <summary>
        /// Tracks whether this <see cref="ArchiveFile"/> has been disposed of or not.
        /// </summary>
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArchiveFile"/> class.
        /// </summary>
        /// <param name="stream">
        /// A <see cref="Stream"/> which represents the archive data.
        /// </param>
        /// <param name="leaveOpen">
        /// <see langword="true"/> to leave the underlying <paramref name="stream"/> open when this <see cref="ArchiveFile"/>
        /// is disposed of; otherwise, <see langword="false"/>.
        /// </param>
        public ArchiveFile(Stream stream, bool leaveOpen)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            this.stream = stream;
            this.leaveOpen = leaveOpen;
        }

        /// <summary>
        /// Gets or sets a <see cref="Stream"/> which represents the current entry.
        /// </summary>
        public SubStream EntryStream
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets or sets the name of the current entry.
        /// </summary>
        public string FileName
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets or sets the header for the current file.
        /// </summary>
        public IArchiveHeader FileHeader
        {
            get;
            protected set;
        }

        /// <summary>
        /// Returns a <see cref="Stream"/> which represents the content of the current entry in the <see cref="CpioFile"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="Stream"/> which represents the content of the current entry in the <see cref="CpioFile"/>.
        /// </returns>
        public Stream Open()
        {
            return this.EntryStream;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (!this.leaveOpen)
            {
                this.Stream.Dispose();
            }

            this.disposed = true;
        }

        /// <summary>
        /// Gets the <see cref="Stream"/> which underpins this <see cref="ArchiveFile"/>.
        /// </summary>
        protected Stream Stream
        {
            get
            {
                this.EnsureNotDisposed();
                return this.stream;
            }
        }

        /// <summary>
        /// Throws a <see cref="ObjectDisposedException"/> if <see cref="Dispose"/> has been previously called.
        /// </summary>
        protected void EnsureNotDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }
        }

        /// <summary>
        /// Aligns the current position.
        /// </summary>
        /// <param name="alignmentBase">
        /// The value to which to align.
        /// </param>
        protected void Align(int alignmentBase)
        {
            var currentIndex =
                (int) (EntryStream != null ? (EntryStream.Offset + EntryStream.Length) : Stream.Position);

            if (Stream.CanSeek)
                Stream.Seek(currentIndex + PaddingSize(alignmentBase, currentIndex), SeekOrigin.Begin);
            else
            {
                byte[] buffer = new byte[PaddingSize(alignmentBase, currentIndex)];
                Stream.Read(buffer, 0, buffer.Length);
            }
        }

        /// <summary>
        /// Reads the next entry in the archive.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if an entry is present, <see langword="false"/> if no more entries
        /// could be found in the archive.
        /// </returns>
        public abstract bool Read();

        /// <summary>
        /// Skips the current entry, without reading the entry data.
        /// </summary>
        public void Skip()
        {
            byte[] buffer = new byte[60 * 1024];

            while (this.EntryStream.Read(buffer, 0, buffer.Length) > 0)
            {
                // Keep reading until we're at the end of the stream.
            }
        }

        public static int PaddingSize(int multiple, int value)
        {
            if (value % multiple == 0)
            {
                return 0;
            }
            else
            {
                return multiple - value % multiple;
            }
        }
    }
}
