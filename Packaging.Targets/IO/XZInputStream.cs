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
    /// Represents a <see cref="Stream"/> which can decompress xz-compressed data.
    /// This is a thin wrapper around Joveler.Compression.XZ that mimics the original XZInputStream behavior.
    /// </summary>
    public class XZInputStream : Stream
    {
        private readonly Stream innerStream;
        private readonly XZStream xzStream;

        private long position;
        private long length;

        /// <summary>
        /// Initializes a new instance of the <see cref="XZInputStream"/> class.
        /// </summary>
        /// <param name="stream">
        /// The underlying <see cref="Stream"/> from which to decompress the data.
        /// </param>
        public XZInputStream(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            XZHelper.EnsureInitialized();
            this.innerStream = stream;
            var decompOpts = new XZDecompressOptions { LeaveOpen = true };

            this.xzStream = new XZStream(stream, decompOpts);
        }

        /// <inheritdoc/>
        public override bool CanRead => this.xzStream.CanRead;

        /// <inheritdoc/>
        public override bool CanSeek => false; // XZ decompression streams don't support seeking

        /// <inheritdoc/>
        public override bool CanWrite => false; // Input streams don't support writing

        /// <inheritdoc/>
        public override long Length
        {
            get
            {
                if (this.length == 0)
                {
                    // Try to parse XZ format directly
                    this.length = this.ParseXZLength();
                }

                return this.length;
            }
        }

        /// <inheritdoc/>
        public override long Position
        {
            get => this.position;
            set => throw new NotSupportedException("XZ Stream does not support setting position");
        }

        /// <inheritdoc/>
        public override void Flush() => this.xzStream.Flush();

        /// <inheritdoc/>
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException("XZ Stream does not support seek");

        /// <inheritdoc/>
        public override void SetLength(long value) => throw new NotSupportedException("XZ Stream does not support setting length");

        /// <inheritdoc/>
        public override int Read(byte[] buffer, int offset, int count)
        {
            int bytesRead = this.xzStream.Read(buffer, offset, count);
            this.position += bytesRead;
            return bytesRead;
        }

        /// <inheritdoc/>
        public override void Write(byte[] buffer, int offset, int count) =>
            throw new NotSupportedException("XZ Input stream does not support writing");

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            this.xzStream?.Dispose();
            base.Dispose(disposing);
        }

        private long ParseXZLength()
        {
            const int streamFooterSize = 12;

            var streamFooter = new byte[streamFooterSize];
            this.innerStream.Seek(-streamFooterSize, SeekOrigin.End);
            this.innerStream.Read(streamFooter, 0, streamFooterSize);

            var backwardSize = this.DecodeFooter(streamFooter);
            var indexPointer = new byte[backwardSize];

            this.innerStream.Seek(-streamFooterSize - backwardSize, SeekOrigin.End);
            this.innerStream.Read(indexPointer, 0, (int)backwardSize);
            this.innerStream.Seek(0, SeekOrigin.Begin);

            return this.DecodeIndex(indexPointer);
        }

        private long DecodeFooter(byte[] streamFooter)
        {
            // Verify footer magic "YZ" (bytes 10-11)
            if (streamFooter[10] != 0x59 || streamFooter[11] != 0x5A)
            {
                throw new InvalidDataException("Invalid XZ stream footer");
            }

            // Extract backward size from footer (bytes 4-7, little-endian)
            // This is the size of the index in multiples of 4, minus 1
            var backwardSizeEncoded = BitConverter.ToUInt32(streamFooter, 4);
            var backwardSize = (backwardSizeEncoded + 1) * 4;

            return (long)backwardSize;
        }

        private long DecodeIndex(byte[] indexData)
        {
            if (indexData.Length < 1)
            {
                throw new InvalidDataException("XZ index data is empty");
            }

            int pos = 0;

            // The index starts with a null byte (0x00) as an indicator
            if (indexData[0] != 0x00)
            {
                throw new InvalidDataException($"XZ index should start with 0x00, but found 0x{indexData[0]:X2}");
            }

            pos++;

            // Read record count (variable-length integer)
            var recordCount = this.ReadVarInt(indexData, ref pos);

            if (recordCount == 0)
            {
                throw new InvalidDataException("XZ index contains no records");
            }

            long totalUncompressedSize = 0;

            // Read each record
            for (ulong i = 0; i < recordCount; i++)
            {
                // Each record contains unpadded size and uncompressed size
                var unpaddedSize = this.ReadVarInt(indexData, ref pos);
                var uncompressedSize = this.ReadVarInt(indexData, ref pos);

                totalUncompressedSize += (long)uncompressedSize;
            }

            return totalUncompressedSize;
        }

        private ulong ReadVarInt(byte[] data, ref int pos)
        {
            if (pos >= data.Length)
            {
                throw new InvalidDataException("Unexpected end of XZ index data while reading variable-length integer");
            }

            ulong result = 0;
            int shift = 0;

            while (pos < data.Length)
            {
                byte b = data[pos++];
                result |= (ulong)(b & 0x7F) << shift;

                if ((b & 0x80) == 0)
                {
                    break;
                }

                shift += 7;
                if (shift >= 64)
                {
                    throw new InvalidDataException("Variable-length integer too large in XZ index");
                }
            }

            return result;
        }
    }
}
