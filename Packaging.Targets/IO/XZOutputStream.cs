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
using System.IO;
using Joveler.Compression.XZ;

namespace Packaging.Targets.IO
{
    /// <summary>
    /// Represents a <see cref="Stream"/> which can compress data using xz compression.
    /// This is a clean wrapper around Joveler.Compression.XZ.XZStream that provides the original XZOutputStream API.
    /// </summary>
    public class XZOutputStream : Stream
    {
        /// <summary>
        /// Default compression preset.
        /// </summary>
        public const uint DefaultPreset = 6;

        /// <summary>
        /// Default number of threads to use.
        /// </summary>
        public static readonly int DefaultThreads = Environment.ProcessorCount;

        // Th stream we're wrapping. It is sealed, so we can't inherit from it
        private readonly XZStream xzStream;

        public XZOutputStream(Stream s)
            : this(s, DefaultThreads)
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
            if (s == null)
            {
                throw new ArgumentNullException(nameof(s));
            }

            if (threads <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(threads));
            }

            XZHelper.EnsureInitialized();

            var compOpts = CreateCompressOptions(preset, leaveOpen);

            // Use parallel compression if threads > 1
            if (threads > 1)
            {
                var threadOpts = CreateParallelOptions(threads);
                this.xzStream = new XZStream(s, compOpts, threadOpts);
            }
            else
            {
                this.xzStream = new XZStream(s, compOpts);
            }
        }

        /// <inheritdoc/>
        public override bool CanRead => this.xzStream.CanRead;

        /// <inheritdoc/>
        public override bool CanSeek => this.xzStream.CanSeek;

        /// <inheritdoc/>
        public override bool CanWrite => this.xzStream.CanWrite;

        /// <inheritdoc/>
        public override long Length => this.xzStream.Length;

        /// <inheritdoc/>
        public override long Position { get => this.xzStream.Position; set => this.xzStream.Position = value; }

        /// <inheritdoc/>
        public override void Flush() => this.xzStream.Flush();

        /// <inheritdoc/>
        public override long Seek(long offset, SeekOrigin origin) => this.xzStream.Seek(offset, origin);

        /// <inheritdoc/>
        public override void SetLength(long value) => this.xzStream.SetLength(value);

        /// <inheritdoc/>
        public override int Read(byte[] buffer, int offset, int count) => this.xzStream.Read(buffer, offset, count);

        /// <inheritdoc/>
        public override void Write(byte[] buffer, int offset, int count) => this.xzStream.Write(buffer, offset, count);

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            this.xzStream?.Dispose();
            base.Dispose(disposing);
        }

        private static XZCompressOptions CreateCompressOptions(uint preset, bool leaveOpen)
        {
            return new XZCompressOptions
            {
                Level = (LzmaCompLevel)Math.Min((int)preset, 9), // Map preset to compression level
                LeaveOpen = leaveOpen // Forward leaveOpen parameter directly
            };
        }

        private static XZParallelCompressOptions CreateParallelOptions(int threads)
        {
            return new XZParallelCompressOptions
            {
                Threads = threads
            };
        }
    }
}
