using Packaging.Targets.IO;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;

namespace Packaging.Targets.Rpm
{
    /// <summary>
    /// Allows interacting with the payload of a RPM package.
    /// </summary>
    internal class RpmPayloadReader
    {
        public static void Read(RpmPackage package)
        {
            if (package == null)
            {
                throw new ArgumentNullException(nameof(package));
            }
            
            using (var payloadDecompressedStream = GetDecompressedPayloadStream(package))
            using (CpioFile file = new CpioFile(payloadDecompressedStream, leaveOpen: true))
            {
                while (file.Read())
                {
                    using (Stream stream = file.Open())
                    {
                        byte[] data = new byte[stream.Length];
                        stream.Read(data, 0, data.Length);
                    }
                }
            }
        }

        /// <summary>
        /// Gets a <see cref="Stream"/> which allows reading the decompressed payload data.
        /// </summary>
        /// <param name="package">
        /// The package for which to read the decompressed payload data.
        /// </param>
        /// <returns>
        /// A <see cref="Stream"/> which allows reading the decompressed payload data.
        /// </returns>
        public static Stream GetDecompressedPayloadStream(RpmPackage package)
        {
            if (package == null)
            {
                throw new ArgumentNullException(nameof(package));
            }

            var compressor = (string)package.Header.Records[IndexTag.RPMTAG_PAYLOADCOMPRESSOR].Value;

            SubStream payloadStream = new SubStream(
                stream: package.Stream,
                offset: package.PayloadOffset,
                length: package.Stream.Length - package.PayloadOffset,
                leaveParentOpen: true,
                readOnly: true);
            var payloadDecompressedStream = GetPayloadDecompressor(payloadStream, compressor);

            return payloadDecompressedStream;
        }

        private static Stream GetPayloadDecompressor(Stream payloadStream, string compressor)
        {
            if (string.Equals(compressor, "gzip"))
            {
                return new GZipStream(payloadStream, CompressionMode.Decompress, leaveOpen: false);
            }
            else if (string.Equals(compressor, "xz"))
            {
                return new XZInputStream(payloadStream);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(compressor));
            }
        }
    }
}
