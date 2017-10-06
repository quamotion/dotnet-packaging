/*
 * The MIT License (MIT)

 * Copyright (c) 2015 Roman Belkov, Kirill Melentyev

 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:

 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.

 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
*/

using System;
using System.Diagnostics;
using System.IO;

namespace Packaging.Targets.IO
{
    public unsafe class XZOutputStream : Stream
    {
        /// <summary>
        /// Default compression preset.
        /// </summary>
        public const uint DefaultPreset = 6;
        public const uint PresetExtremeFlag = (uint)1 << 31;

        // You can tweak BufSize value to get optimal results
        // of speed and chunk size
        private const int BufSize = 4096;

        private readonly Stream innerStream;
        private readonly bool leaveOpen;
        private readonly byte[] outbuf;
        private LzmaStream lzmaStream;
        private bool disposed;

        public XZOutputStream(Stream s)
            : this(s, 1)
        {
        }

        public XZOutputStream(Stream s, int threads)
            : this(s, threads, DefaultPreset)
        {
        }

        public XZOutputStream(Stream s, int threads, uint preset)
            : this(s, threads, preset, false)
        {
        }

        public XZOutputStream(Stream s, int threads, uint preset, bool leaveOpen)
        {
            this.innerStream = s;
            this.leaveOpen = leaveOpen;

            LzmaResult ret;
            if (threads == 1)
            {
                ret = NativeMethods.lzma_easy_encoder(ref this.lzmaStream, preset, LzmaCheck.Crc64);
            }
            else
            {
                if (threads <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(threads));
                }

                if (threads > Environment.ProcessorCount)
                {
                    Trace.TraceWarning("{0} threads required, but only {1} processors available", threads, Environment.ProcessorCount);
                    threads = Environment.ProcessorCount;
                }

                var mt = new LzmaMT()
                {
                    preset = preset,
                    check = LzmaCheck.Crc64,
                    threads = (uint)threads
                };
                ret = NativeMethods.lzma_stream_encoder_mt(ref this.lzmaStream, ref mt);
            }

            if (ret == LzmaResult.OK)
            {
                this.outbuf = new byte[BufSize];
                this.lzmaStream.AvailOut = BufSize;
                return;
            }

            GC.SuppressFinalize(this);
            throw GetError(ret);
        }

        ~XZOutputStream()
        {
            this.Dispose(false);
        }

        /// <inheritdoc/>
        public override bool CanRead
        {
            get
            {
                this.EnsureNotDisposed();
                return false;
            }
        }

        /// <inheritdoc/>
        public override bool CanSeek
        {
            get
            {
                this.EnsureNotDisposed();
                return false;
            }
        }

        /// <inheritdoc/>
        public override bool CanWrite
        {
            get
            {
                this.EnsureNotDisposed();
                return true;
            }
        }

        /// <inheritdoc/>
        public override long Length
        {
            get
            {
                this.EnsureNotDisposed();
                throw new NotSupportedException();
            }
        }

        /// <inheritdoc/>
        public override long Position
        {
            get
            {
                this.EnsureNotDisposed();
                throw new NotSupportedException();
            }

            set
            {
                this.EnsureNotDisposed();
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Single-call buffer encoding
        /// </summary>
        public static byte[] Encode(byte[] buffer, uint preset = DefaultPreset)
        {
            var res = new byte[(long)NativeMethods.lzma_stream_buffer_bound((UIntPtr)buffer.Length)];

            UIntPtr outPos;
            var ret = NativeMethods.lzma_easy_buffer_encode(preset, LzmaCheck.Crc64, null, buffer, (UIntPtr)buffer.Length, res, &outPos, (UIntPtr)res.Length);
            if (ret != LzmaResult.OK)
            {
                throw GetError(ret);
            }

            if ((long)outPos < res.Length)
            {
                Array.Resize(ref res, (int)(ulong)outPos);
            }

            return res;
        }

        /// <inheritdoc/>
        public override void Flush()
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public override long Seek(long offset, SeekOrigin origin)
        {
            this.EnsureNotDisposed();
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public override void SetLength(long value)
        {
            this.EnsureNotDisposed();
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public override int Read(byte[] buffer, int offset, int count)
        {
            this.EnsureNotDisposed();
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public override void Write(byte[] buffer, int offset, int count)
        {
            this.EnsureNotDisposed();

            if (count == 0)
            {
                return;
            }

            var guard = buffer[checked((uint)offset + (uint)count) - 1];

            if (this.lzmaStream.AvailIn != 0)
            {
                throw new InvalidOperationException();
            }

            this.lzmaStream.AvailIn = (uint)count;
            do
            {
                LzmaResult ret;
                fixed (byte* inbuf = &buffer[offset])
                {
                    this.lzmaStream.NextIn = (IntPtr)inbuf;
                    fixed (byte* outbuf = &this.outbuf[BufSize - this.lzmaStream.AvailOut])
                    {
                        this.lzmaStream.NextOut = (IntPtr)outbuf;
                        ret = NativeMethods.lzma_code(ref this.lzmaStream, LzmaAction.Run);
                    }

                    offset += (int)((ulong)this.lzmaStream.NextIn - (ulong)(IntPtr)inbuf);
                }

                if (ret != LzmaResult.OK)
                {
                    throw this.ThrowError(ret);
                }

                if (this.lzmaStream.AvailOut == 0)
                {
                    this.innerStream.Write(this.outbuf, 0, BufSize);
                    this.lzmaStream.AvailOut = BufSize;
                }
            }
            while (this.lzmaStream.AvailIn != 0);
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            // finish encoding only if all input has been successfully processed
            if (this.lzmaStream.InternalState != IntPtr.Zero && this.lzmaStream.AvailIn == 0)
            {
                LzmaResult ret;
                do
                {
                    fixed (byte* outbuf = &this.outbuf[BufSize - (int)this.lzmaStream.AvailOut])
                    {
                        this.lzmaStream.NextOut = (IntPtr)outbuf;
                        ret = NativeMethods.lzma_code(ref this.lzmaStream, LzmaAction.Finish);
                    }

                    if (ret > LzmaResult.StreamEnd)
                    {
                        throw this.ThrowError(ret);
                    }

                    var writeSize = BufSize - (int)this.lzmaStream.AvailOut;
                    if (writeSize != 0)
                    {
                        this.innerStream.Write(this.outbuf, 0, writeSize);
                        this.lzmaStream.AvailOut = BufSize;
                    }
                }
                while (ret != LzmaResult.StreamEnd);
            }

            NativeMethods.lzma_end(ref this.lzmaStream);

            if (disposing && !this.leaveOpen)
            {
                this.innerStream?.Dispose();
            }

            base.Dispose(disposing);

            this.disposed = true;
        }

        private static Exception GetError(LzmaResult ret)
        {
            switch (ret)
            {
                case LzmaResult.MemError: return new OutOfMemoryException("Memory allocation failed");
                case LzmaResult.OptionsError: return new ArgumentException("Specified preset is not supported");
                case LzmaResult.UnsupportedCheck: return new Exception("Specified integrity check is not supported");
                case LzmaResult.DataError: return new InvalidDataException("File size limits exceeded");
                default: return new Exception("Unknown error, possibly a bug: " + ret);
            }
        }

        /// <summary>
        /// Throws an exception if this stream is disposed of.
        /// </summary>
        private void EnsureNotDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(nameof(XZOutputStream));
            }
        }

        private Exception ThrowError(LzmaResult ret)
        {
            NativeMethods.lzma_end(ref this.lzmaStream);
            return GetError(ret);
        }
    }
}