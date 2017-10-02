using System;
using System.IO;
using System.IO.Compression;

namespace Packaging.Targets.IO
{
    /// <summary>
    /// Provides read-only access do a decompressed <see cref="GZipStream"/>, and keeps track of the current position.
    /// </summary>
    internal class GZipDecompressor : GZipStream
    {
        private long position = 0;

        public GZipDecompressor(Stream stream, bool leaveOpen)
            : base(stream, CompressionMode.Decompress, leaveOpen)
        {
        }

        /// <inheritdoc/>
        public override long Position
        {
            get { return this.position; }
            set { throw new NotSupportedException(); }
        }

        /// <inheritdoc/>
        public override int Read(byte[] array, int offset, int count)
        {
            var read = base.Read(array, offset, count);
            this.position += read;
            return read;
        }
    }
}
