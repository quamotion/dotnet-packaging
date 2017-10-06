// Copyright (c) 2008-2011, Kenneth Bell
//
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace DiscUtils.Internal
{
    /// <summary>
    /// The concatenation of multiple streams (read-only, for now).
    /// </summary>
    internal class ConcatStream : Stream
    {
        private readonly bool canWrite;
        private readonly bool leaveOpen;

        private long position;
        private Stream[] streams;

        public ConcatStream(params Stream[] streams)
            : this(false, streams)
        {
        }

        public ConcatStream(bool leaveOpen, params Stream[] streams)
        {
            this.streams = streams;
            this.leaveOpen = leaveOpen;

            // Only allow writes if all streams can be written
            this.canWrite = true;
            foreach (Stream stream in streams)
            {
                if (!stream.CanWrite)
                {
                    this.canWrite = false;
                }
            }
        }

        public override bool CanRead
        {
            get
            {
                this.CheckDisposed();
                return true;
            }
        }

        public override bool CanSeek
        {
            get
            {
                this.CheckDisposed();
                return true;
            }
        }

        public override bool CanWrite
        {
            get
            {
                this.CheckDisposed();
                return this.canWrite;
            }
        }

        public override long Length
        {
            get
            {
                this.CheckDisposed();
                long length = 0;
                for (int i = 0; i < this.streams.Length; ++i)
                {
                    length += this.streams[i].Length;
                }

                return length;
            }
        }

        public override long Position
        {
            get
            {
                this.CheckDisposed();
                return this.position;
            }

            set
            {
                this.CheckDisposed();
                this.position = value;
            }
        }

        public override void Flush()
        {
            this.CheckDisposed();
            for (int i = 0; i < this.streams.Length; ++i)
            {
                this.streams[i].Flush();
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            this.CheckDisposed();

            int totalRead = 0;
            int numRead = 0;

            do
            {
                long activeStreamStartPos;
                int activeStream = this.GetActiveStream(out activeStreamStartPos);

                this.streams[activeStream].Position = this.position - activeStreamStartPos;

                numRead = this.streams[activeStream].Read(buffer, offset + totalRead, count - totalRead);

                totalRead += numRead;
                this.position += numRead;
            }
            while (numRead != 0);

            return totalRead;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            this.CheckDisposed();

            long effectiveOffset = offset;
            if (origin == SeekOrigin.Current)
            {
                effectiveOffset += this.position;
            }
            else if (origin == SeekOrigin.End)
            {
                effectiveOffset += this.Length;
            }

            if (effectiveOffset < 0)
            {
                throw new IOException("Attempt to move before beginning of disk");
            }

            this.Position = effectiveOffset;
            return this.Position;
        }

        public override void SetLength(long value)
        {
            this.CheckDisposed();

            long lastStreamOffset;
            int lastStream = this.GetStream(this.Length, out lastStreamOffset);
            if (value < lastStreamOffset)
            {
                throw new IOException($"Unable to reduce stream length to less than {lastStreamOffset}");
            }

            this.streams[lastStream].SetLength(value - lastStreamOffset);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            this.CheckDisposed();

            int totalWritten = 0;
            while (totalWritten != count)
            {
                // Offset of the stream = streamOffset
                long streamOffset;
                int streamIdx = this.GetActiveStream(out streamOffset);

                // Offset within the stream = streamPos
                long streamPos = this.position - streamOffset;
                this.streams[streamIdx].Position = streamPos;

                // Write (limited to the stream's length), except for final stream - that may be
                // extendable
                int numToWrite;
                if (streamIdx == this.streams.Length - 1)
                {
                    numToWrite = count - totalWritten;
                }
                else
                {
                    numToWrite = (int)Math.Min(count - totalWritten, this.streams[streamIdx].Length - streamPos);
                }

                this.streams[streamIdx].Write(buffer, offset + totalWritten, numToWrite);

                totalWritten += numToWrite;
                this.position += numToWrite;
            }
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && this.streams != null)
                {
                    if (!this.leaveOpen)
                    {
                        foreach (Stream stream in this.streams)
                        {
                            stream.Dispose();
                        }
                    }

                    this.streams = null;
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        private int GetActiveStream(out long startPos)
        {
            return this.GetStream(this.position, out startPos);
        }

        private int GetStream(long targetPos, out long streamStartPos)
        {
            // Find the stream that _position is within
            streamStartPos = 0;
            int focusStream = 0;
            while (focusStream < this.streams.Length - 1 && streamStartPos + this.streams[focusStream].Length <= targetPos)
            {
                streamStartPos += this.streams[focusStream].Length;
                focusStream++;
            }

            return focusStream;
        }

        private void CheckDisposed()
        {
            if (this.streams == null)
            {
                throw new ObjectDisposedException(nameof(ConcatStream));
            }
        }
    }
}