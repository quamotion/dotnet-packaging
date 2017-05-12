using System;
using System.IO;

namespace Packaging.Targets.IO
{
    /// <summary>
    /// Represents parts of a <see cref="Stream"/>, from a start byte offset for a given length.
    /// </summary>
    public class SubStream : Stream
    {
        /// <summary>
        /// The parent stream of this stream.
        /// </summary>
        private Stream stream;

        /// <summary>
        /// The offset, that is, the location at which the <see cref="SubStream"/> starts.
        /// </summary>
        private long subStreamOffset;

        /// <summary>
        /// The length of the <see cref="SubStream"/>.
        /// </summary>
        private long subStreamLength;

        /// <summary>
        /// A value indicating whether the parent stream should be closed when this
        /// <see cref="SubStream"/> is closed, or not.
        /// </summary>
        private bool leaveParentOpen;

        /// <summary>
        /// A value indicating whether this <see cref="SubStream"/> should only support read-only operations.
        /// </summary>
        private bool readOnly;

        /// <summary>
        /// The current position of the <see cref="SubStream"/>. This allows us keep the <see cref="Position"/>
        /// consistent accross multiple calls, even if the position of <see cref="stream"/> changes (e.g. because
        /// another <see cref="SubStream"/> operates on it).
        /// </summary>
        private long position;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubStream"/> class.
        /// </summary>
        /// <param name="stream">
        /// The parent stream of this stream.
        /// </param>
        /// <param name="offset">
        /// The offset at which the stream starts.
        /// </param>
        /// <param name="length">
        /// The length of the <see cref="SubStream"/>.
        /// </param>
        /// <param name="leaveParentOpen">
        /// A value indicating whether the parent stream should be closed when this
        /// <see cref="SubStream"/> is closed, or not.
        /// </param>
        /// <param name="readOnly">
        /// A value indicating whether the <see cref="SubStream"/> be opened in read-only mode or not.
        /// </param>
        public SubStream(Stream stream, long offset, long length, bool leaveParentOpen = false, bool readOnly = false)
        {
            this.stream = stream;
            this.subStreamOffset = offset;
            this.subStreamLength = length;
            this.leaveParentOpen = leaveParentOpen;
            this.readOnly = readOnly;
            this.position = 0;

            if (this.stream.CanSeek)
            {
                this.Seek(0, SeekOrigin.Begin);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports reading
        /// </summary>
        public override bool CanRead
        {
            get
            {
                return this.stream.CanRead;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports seeking.
        /// </summary>
        public override bool CanSeek
        {
            get
            {
                return this.stream.CanSeek;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports writing.
        /// </summary>
        public override bool CanWrite
        {
            get
            {
                return !this.readOnly && this.stream.CanWrite;
            }
        }

        /// <summary>
        /// Gets the length in bytes of the stream.
        /// </summary>
        public override long Length
        {
            get
            {
                return this.subStreamLength;
            }
        }

        /// <summary>
        /// Gets or sets the position within the current <see cref="SubStream"/>.
        /// </summary>
        public override long Position
        {
            get
            {
                return this.position;
            }

            set
            {
                lock (this.stream)
                {
                    this.stream.Position = value + this.Offset;
                    this.position = value;
                }
            }
        }

        /// <summary>
        /// Gets the parent stream of this <see cref="SubStream"/>.
        /// </summary>
        internal Stream Stream
        {
            get
            {
                return this.stream;
            }
        }

        /// <summary>
        /// Gets the offset at which the <see cref="SubStream"/> starts.
        /// </summary>
        internal long Offset
        {
            get
            {
                return this.subStreamOffset;
            }
        }

        /// <summary>
        /// Clears all buffers for this stream and causes any buffered data to be written to the underlying device.
        /// </summary>
        public override void Flush()
        {
            lock (this.stream)
            {
                this.stream.Flush();
            }
        }

        /// <summary>
        /// Reads a sequence of bytes from the current stream and advances the position within the stream by the number of bytes read.
        /// </summary>
        /// <param name="buffer">
        /// An array of bytes. When this method returns, the buffer contains the specified byte array with the values between offset
        /// and (offset + count - 1) replaced by the bytes read from the current source.
        /// </param>
        /// <param name="offset">
        /// The zero-based byte offset in buffer at which to begin storing the data read from the current stream.
        /// </param>
        /// <param name="count">
        /// The maximum number of bytes to be read from the current stream.
        /// </param>
        /// <returns>
        /// The total number of bytes read into the buffer. This can be less than the number of bytes requested if that many bytes are
        /// not currently available, or zero (0) if the end of the stream has been reached.
        /// </returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            lock (this.stream)
            {
                this.EnsurePosition();

                // Make sure we don't pass the size of the substream
                long bytesRemaining = this.Length - this.Position;
                long bytesToRead = Math.Min(count, bytesRemaining);

                if (bytesToRead < 0)
                {
                    bytesToRead = 0;
                }

                var read = this.stream.Read(buffer, offset, (int)bytesToRead);
                this.position += read;
                return read;
            }
        }

        /// <summary>
        /// Writes a sequence of bytes to the current stream and advances the current position
        /// within this stream by the number of bytes written.
        /// </summary>
        /// <param name="buffer">
        /// An array of bytes. This method copies count bytes from buffer to the current stream.
        /// </param>
        /// <param name="offset">
        /// The zero-based byte offset in buffer at which to begin copying bytes to the current stream.
        /// </param>
        /// <param name="count">
        /// The number of bytes to be written to the current stream.
        /// </param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (this.readOnly)
            {
                throw new NotSupportedException();
            }

            lock (this.stream)
            {
                this.EnsurePosition();

                if (this.Position + offset + count > this.Length || this.Position < 0)
                {
                    throw new InvalidOperationException("This write operation would exceed the current length of the substream.");
                }

                this.stream.Write(buffer, offset, count);
                this.position += count;
            }
        }

        /// <summary>
        /// Writes a byte to the current position in the stream and advances the position within the stream by one byte.
        /// </summary>
        /// <param name="value">
        /// The byte to write to the stream.
        /// </param>
        public override void WriteByte(byte value)
        {
            if (this.readOnly)
            {
                throw new NotSupportedException();
            }

            lock (this.stream)
            {
                this.EnsurePosition();

                if (this.Position > this.Length || this.Position < 0)
                {
                    throw new InvalidOperationException("This write operation would exceed the current length of the substream.");
                }

                this.stream.WriteByte(value);
                this.position++;
            }
        }

        /// <summary>
        /// Sets the position within the current stream.
        /// </summary>
        /// <param name="offset">
        /// A byte offset relative to the origin parameter.
        /// </param>
        /// <param name="origin">
        /// A value of type SeekOrigin indicating the reference point used to obtain the new position.
        /// </param>
        /// <returns>
        /// The new position within the current stream.
        /// </returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            lock (this.stream)
            {
                switch (origin)
                {
                    case SeekOrigin.Begin:
                        offset += this.subStreamOffset;
                        break;

                    case SeekOrigin.End:
                        long enddelta = this.subStreamOffset + this.subStreamLength - this.stream.Length;
                        offset += enddelta;
                        break;

                    case SeekOrigin.Current:
                        // Nothing to do, because we'll pass SeekOrigin.Current to the
                        // parent stream.
                        break;
                }

                // If we're doing an absolute seek, we don't care about the position,
                // but if the seek is relative, make sure we start from the correct position
                if (origin == SeekOrigin.Current)
                {
                    this.EnsurePosition();
                }

                var parentPosition = this.stream.Seek(offset, origin);
                this.position = parentPosition - this.Offset;
                return this.position;
            }
        }

        /// <summary>
        /// Sets the length of the current stream.
        /// </summary>
        /// <param name="value">
        /// The desired length of the current stream in bytes.
        /// </param>
        public override void SetLength(long value)
        {
            if (this.readOnly)
            {
                throw new NotSupportedException();
            }

            this.subStreamLength = value;
        }

        /// <summary>
        /// Updates the size of this <see cref="SubStream"/> relative to its parent stream.
        /// </summary>
        /// <param name="offset">
        /// The new offset.
        /// </param>
        /// <param name="length">
        /// The new length.
        /// </param>
        public void UpdateWindow(long offset, long length)
        {
            this.subStreamOffset = offset;
            this.subStreamLength = length;
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (!this.leaveParentOpen)
            {
                this.stream.Dispose();
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Makes sure the position of the parent stream is aligned with the position we have stored locally.
        /// </summary>
        /// <remarks>
        /// Take a scenario where you have two <see cref="SubStream"/> objects that navigate the same <see cref="Stream"/>.
        /// They can both seek independently, so the parent's position will change without the other <see cref="SubStream"/>
        /// knowing about it. Calling <see cref="EnsurePosition"/> corrects that, enabling scenarios where you can synchronously
        /// do I/O on both streams: things like <see cref="Stream.CopyTo(Stream)"/> should start working.
        /// This will, however, not work for multi-threaded access.
        /// </remarks>
        private void EnsurePosition()
        {
            if (this.stream.Position != this.position + this.Offset)
            {
                this.stream.Position = this.position + this.Offset;
            }
        }
    }
}
