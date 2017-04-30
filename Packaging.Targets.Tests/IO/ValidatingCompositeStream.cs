using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Packaging.Targets.Tests
{
    /// <summary>
    /// A <see cref="Stream"/> which reads its input from a <see cref="Stream"/>,
    /// writes its output to a <see cref="Stream"/> and validates that the data which is being written to
    /// the output stream matches the data in a reference stream (usually a recorded trace).
    /// Used with unit tests which replay recorded traces.
    /// </summary>
    internal class ValidatingCompositeStream : Stream
    {
        private Stream input;
        private Stream output;
        private Stream expectedOutput;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidatingCompositeStream"/> class.
        /// </summary>
        /// <param name="input">
        /// The <see cref="Stream"/> from which to read data.
        /// </param>
        /// <param name="output">
        /// The <see cref="Stream"/> to which to write data.
        /// </param>
        /// <param name="expectedOutput">
        /// The reference stream for <paramref name="output"/>.
        /// </param>
        public ValidatingCompositeStream(Stream input, Stream output, Stream expectedOutput)
        {
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            if (expectedOutput == null)
            {
                throw new ArgumentNullException(nameof(expectedOutput));
            }

            this.input = input;
            this.output = output;
            this.expectedOutput = expectedOutput;
        }

        /// <inheritdoc/>
        public override bool CanRead
        {
            get { return true; }
        }

        /// <inheritdoc/>
        public override bool CanSeek
        {
            get { return false; }
        }

        /// <inheritdoc/>
        public override bool CanWrite
        {
            get { return true; }
        }

        /// <inheritdoc/>
        public override long Length
        {
            get { throw new NotImplementedException(); }
        }

        /// <inheritdoc/>
        public override long Position
        {
            get { return this.output.Position; }
            set { throw new NotImplementedException(); }
        }

        /// <inheritdoc/>
        public override void Flush()
        {
            this.input?.Flush();
            this.output.Flush();
        }

        /// <inheritdoc/>
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (this.input == null)
            {
                throw new InvalidOperationException();
            }

            return this.input.Read(buffer, offset, count);
        }

        /// <inheritdoc/>
        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            if (this.input == null)
            {
                throw new InvalidOperationException();
            }

            return this.input.ReadAsync(buffer, offset, count, cancellationToken);
        }

        /// <inheritdoc/>
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override void Write(byte[] buffer, int offset, int count)
        {
            byte[] expected = new byte[buffer.Length];
            this.expectedOutput.Read(expected, offset, count);

            byte[] bufferChunk = buffer.Skip(offset).Take(count).ToArray();
            byte[] expectedChunk = expected.Skip(offset).Take(count).ToArray();

            // Make sure the data being writen matches the data which was written to the expected stream.
            // This will detect any errors as the invalid data is being written out - as opposed to post-
            // test binary validation.
            Assert.Equal(expectedChunk, bufferChunk);
            this.output.Write(buffer, offset, count);
        }

        /// <inheritdoc/>
        public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            byte[] expected = new byte[buffer.Length];
            await this.expectedOutput.ReadAsync(expected, offset, count, cancellationToken).ConfigureAwait(false);

            byte[] bufferChunk = buffer.Skip(offset).Take(count).ToArray();
            byte[] expectedChunk = expected.Skip(offset).Take(count).ToArray();

            // Make sure the data being writen matches the data which was written to the expected stream.
            // This will detect any errors as the invalid data is being written out - as opposed to post-
            // test binary validation.
            Assert.Equal(expectedChunk, bufferChunk);

            await this.output.WriteAsync(buffer, offset, count, cancellationToken).ConfigureAwait(false);
        }
    }
}
