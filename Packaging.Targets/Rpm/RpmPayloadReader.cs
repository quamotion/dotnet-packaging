using Packaging.Targets.IO;
using System;
using System.Collections.ObjectModel;
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
        public static Collection<string> Read(RpmPackage package)
        {
            if (package == null)
            {
                throw new ArgumentNullException(nameof(package));
            }

            Collection<string> names = new Collection<string>();

            using (var payloadDecompressedStream = GetDecompressedPayloadStream(package))
            using (CpioFile file = new CpioFile(payloadDecompressedStream, leaveOpen: true))
            {
                while (file.Read())
                {
                    names.Add(file.EntryName);

                    using (Stream stream = file.Open())
                    {
                        byte[] data = new byte[stream.Length];
                        stream.Read(data, 0, data.Length);
                    }
                }
            }

            return names;
        }

        /// <summary>
        /// Gets the compressed payload for a package.
        /// </summary>
        /// <param name="package">
        /// The package for which to get the compressed payload.
        /// </param>
        /// <returns>
        /// A <see cref="Stream"/> which contains the compressed payload.
        /// </returns>
        public static Stream GetCompressedPayloadStream(RpmPackage package)
        {
            if (package == null)
            {
                throw new ArgumentNullException(nameof(package));
            }

            SubStream compressedPayloadStream = new SubStream(
                stream: package.Stream,
                offset: package.PayloadOffset,
                length: package.Stream.Length - package.PayloadOffset,
                leaveParentOpen: true,
                readOnly: true);

            return compressedPayloadStream;
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

            var compressedPayloadStream = GetCompressedPayloadStream(package);

            return GetDecompressedPayloadStream(package, compressedPayloadStream);
        }


        /// <summary>
        /// Gets a <see cref="Stream"/> which allows reading the decompressed payload data.
        /// </summary>
        /// <param name="package">
        /// The package for which to read the decompressed payload data.
        /// </param>
        /// <param name="compressedPayloadStream">
        /// The compressed payload stream.
        /// </param>
        /// <returns>
        /// A <see cref="Stream"/> which allows reading the decompressed payload data.
        /// </returns>
        public static Stream GetDecompressedPayloadStream(RpmPackage package, Stream compressedPayloadStream)
        {
            var compressor = (string)package.Header.Records[IndexTag.RPMTAG_PAYLOADCOMPRESSOR].Value;
            var payloadDecompressedStream = GetPayloadDecompressor(compressedPayloadStream, compressor);

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
