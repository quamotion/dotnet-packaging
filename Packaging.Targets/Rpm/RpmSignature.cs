using Org.BouncyCastle.Bcpg.OpenPgp;
using Packaging.Targets.IO;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace Packaging.Targets.Rpm
{
    /// <summary>
    /// Enables interacting with the signature of a <see cref="RpmPackage"/>.
    /// </summary>
    internal class RpmSignature
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RpmSignature"/> class.
        /// </summary>
        /// <param name="package">
        /// The package for which to access the signature.
        /// </param>
        public RpmSignature(RpmPackage package)
        {
            if (package == null)
            {
                throw new ArgumentNullException(nameof(package));
            }

            this.Package = package;
        }

        /// <summary>
        /// Gets the package for which the signature is being inspected.
        /// </summary>
        public RpmPackage Package
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the length of the immutable region, starting from the position of the <see cref="SignatureTag.RPMTAG_HEADERSIGNATURES"/>
        /// record. This record should be the last record in the signature block, and the value is negative, indicating the previous
        /// blocks are considered immutable.
        /// </summary>
        public int ImmutableRegionSize
        {
            get
            {
                // For more information about immutable regions, see:
                // http://ftp.rpm.org/api/4.4.2.2/hregions.html
                // https://dentrassi.de/2016/04/15/writing-rpm-files-in-plain-java/
                // https://blog.bethselamin.de/posts/argh-pm.html
                // For now, we're always assuming the entire header and the entire signature is immutable

                var immutableSignatureRegion = (byte[])this.Package.Signature.Records[SignatureTag.RPMTAG_HEADERSIGNATURES].Value;

                using (MemoryStream s = new MemoryStream(immutableSignatureRegion))
                {
                    var h = s.ReadStruct<IndexHeader>();
                    return h.Offset;
                }
            }
        }

        /// <summary>
        /// Gets the SHA1 hash of the header section.
        /// </summary>
        public byte[] Sha1Hash
        {
            get
            {
                return StringToByteArray((string)this.Package.Signature.Records[SignatureTag.RPMSIGTAG_SHA1].Value);
            }
        }

        /// <summary>
        /// Gets the MD5 hash of the header section and the compressed payload.
        /// </summary>
        public byte[] MD5Hash
        {
            get
            {
                return (byte[])this.Package.Signature.Records[SignatureTag.RPMSIGTAG_MD5].Value;
            }
        }

        /// <summary>
        /// Gets the <see cref="PgpSignature"/> of the header section.
        /// </summary>
        public PgpSignature HeaderPgpSignature
        {
            get
            {
                return this.GetPgpSignature(SignatureTag.RPMSIGTAG_RSA);
            }
        }

        /// <summary>
        /// Gets the <see cref="PgpSignature"/> of the header section and compressed payload.
        /// </summary>
        public PgpSignature HeaderAndPayloadPgpSignature
        {
            get
            {
                return this.GetPgpSignature(SignatureTag.RPMSIGTAG_PGP);
            }
        }

        /// <summary>
        /// Gets the combined size of the header section and the compressed payload.
        /// </summary>
        public int HeaderAndPayloadSize
        {
            get
            {
                return (int)this.Package.Signature.Records[SignatureTag.RPMSIGTAG_SIZE].Value;
            }
        }

        /// <summary>
        /// Gets the size of the uncompressed payload.
        /// </summary>
        public int UncompressedPayloadSize
        {
            get
            {
                return (int)this.Package.Signature.Records[SignatureTag.RPMSIGTAG_PAYLOADSIZE].Value;
            }
        }

        /// <summary>
        /// Gets a PGP signature.
        /// </summary>
        /// <param name="tag">
        /// The tag which contains the PGP signature.
        /// </param>
        /// <returns>
        /// A <see cref="PgpSignature"/> object which represents the PGP signature.
        /// </returns>
        private PgpSignature GetPgpSignature(SignatureTag tag)
        {
            byte[] value = (byte[])this.Package.Signature.Records[tag].Value;

            using (MemoryStream stream = new MemoryStream(value))
            using (var signatureStream = PgpUtilities.GetDecoderStream(stream))
            {
                PgpObjectFactory pgpFactory = new PgpObjectFactory(signatureStream);
                PgpSignatureList signatureList = (PgpSignatureList)pgpFactory.NextPgpObject();
                PgpSignature signature = signatureList[0];
                return signature;
            }
        }

        /// <summary>
        /// Converst a hexadecimal string to a <see cref="byte"/> array.
        /// </summary>
        /// <param name="hex">
        /// A <see cref="string"/> containing the hexadecimal representation of the value.
        /// </param>
        /// <returns>
        /// A byte array representing the same value.
        /// </returns>
        internal static byte[] StringToByteArray(string hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];

            for (int i = 0; i < NumberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);

            }

            return bytes;
        }

        /// <summary>
        /// Verifys the signature of a RPM package.
        /// </summary>
        /// <param name="pgpPublicKey">
        /// A <see cref="Stream"/> which contains the public key used to verify the data.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the verification was successful; otherwise, <see langword="false"/>.
        /// </returns>
        public bool Verify(Stream pgpPublicKey)
        {
            // 1 Verify the header signature immutable block: make sure all records are marked as immutable
            var immutableRegionSize = this.ImmutableRegionSize;
            var immutableEntryCount = -immutableRegionSize / Marshal.SizeOf<IndexHeader>();

            if (this.Package.Signature.Records.Count != immutableEntryCount)
            {
                return false;
            }

            // Store the header data and the header + payload data in substreams. This enables us to calculate hashes and
            // verify signatures using these data ranges.
            using (Stream headerStream = new SubStream(
                this.Package.Stream,
                this.Package.HeaderOffset,
                this.Package.PayloadOffset - this.Package.HeaderOffset,
                leaveParentOpen: true, readOnly: true))
            using (Stream headerAndPayloadStream = new SubStream(
                this.Package.Stream,
                this.Package.HeaderOffset,
                this.Package.Stream.Length - this.Package.HeaderOffset,
                leaveParentOpen: true,
                readOnly: true))
            {
                // Verify the SHA1 hash. This one covers the header block
                SHA1 sha = SHA1.Create();
                var calculatedShaValue = sha.ComputeHash(headerStream);
                var actualShaValue = this.Sha1Hash;

                if (!calculatedShaValue.SequenceEqual(actualShaValue))
                {
                    return false;
                }

                // Verify the MD5 hash. This one covers the header and the payload block
                MD5 md5 = MD5.Create();
                var calculatedMd5Value = md5.ComputeHash(headerAndPayloadStream);
                var actualMd5Value = this.MD5Hash;

                if (!calculatedMd5Value.SequenceEqual(actualMd5Value))
                {
                    return false;
                }

                // Verify the PGP signatures
                // 3 for the header
                var headerPgpSignature = this.HeaderPgpSignature;
                headerStream.Position = 0;
                pgpPublicKey.Position = 0;

                if (!PgpSigner.VerifySignature(headerPgpSignature, pgpPublicKey, headerStream))
                {
                    return false;
                }

                var headerAndPayloadPgpSignature = this.HeaderAndPayloadPgpSignature;
                headerAndPayloadStream.Position = 0;
                pgpPublicKey.Position = 0;

                if (!PgpSigner.VerifySignature(headerAndPayloadPgpSignature, pgpPublicKey, headerAndPayloadStream))
                {
                    return false;
                }
            }

            // Verify the signature size (header + compressed payload)
            var headerSize = this.HeaderAndPayloadSize;

            if (headerSize != this.Package.Stream.Length - this.Package.HeaderOffset)
            {
                return false;
            }

            // Verify the payload size (header + uncompressed payload)
            var expectedDecompressedPayloadSize = this.UncompressedPayloadSize;

            long actualDecompressedPayloadLength = 0;

            using (Stream payloadStream = RpmPayloadReader.GetDecompressedPayloadStream(this.Package))
            {
                actualDecompressedPayloadLength = payloadStream.Length;
            }

            if (expectedDecompressedPayloadSize != actualDecompressedPayloadLength)
            {
                return false;
            }

            return true;
        }
    }
}
