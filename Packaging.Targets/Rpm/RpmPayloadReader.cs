using Packaging.Targets.IO;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;

namespace Packaging.Targets.Rpm
{
    class RpmPayloadReader
    {
        public static void Read(RpmPackage package)
        {
            if (package == null)
            {
                throw new ArgumentNullException(nameof(package));
            }

            var compressor = (string)package.Header.Records[IndexTag.RPMTAG_PAYLOADCOMPRESSOR].Value;

            using (SubStream payloadStream = new SubStream(
                stream: package.Stream,
                offset: package.PayloadOffset,
                length: package.Stream.Length - package.PayloadOffset,
                leaveParentOpen: true,
                readOnly: true))
            using (var payloadDecompressedStream = GetPayloadDecompressor(payloadStream, compressor))
            using (CpioFile file = new CpioFile(payloadDecompressedStream, leaveOpen: true))
            {
                while (file.Read())
                {
                    Debug.WriteLine(file.EntryName);

                    using (Stream stream = file.Open())
                    {
                        byte[] data = new byte[stream.Length];
                        stream.Read(data, 0, data.Length);
                    }
                }
            }
        }

        private static Stream GetPayloadDecompressor(Stream payloadStream, string compressor)
        {
            if (string.Equals(compressor, "gzip"))
            {
                return new GZipStream(payloadStream, CompressionMode.Decompress);
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
