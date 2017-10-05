using Packaging.Targets.Rpm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace Packaging.Targets.Tests.Rpm
{
    /// <summary>
    /// Tests the <see cref="RpmSignature"/> class.
    /// </summary>
    public class RpmSignatureTests
    {
        /// <summary>
        /// Reads the various properties of the <see cref="RpmSignature"/> class and makes sure they have the expected values.
        /// </summary>
        [Fact]
        public void SignaturePropertiesTest()
        {
            using (Stream stream = File.OpenRead(@"Rpm/libplist-2.0.1.151-1.1.x86_64.rpm"))
            {
                var package = RpmPackageReader.Read(stream);
                var signature = new RpmSignature(package);

                Assert.Equal(package, signature.Package);
                Assert.Equal(unchecked((long)0xed356a6f4dec2b0a), signature.HeaderAndPayloadPgpSignature.KeyId);
                Assert.Equal(0xfd84, signature.HeaderAndPayloadSize);
                Assert.Equal(0x27430, signature.UncompressedPayloadSize);
                Assert.Equal(unchecked((long)0xed356a6f4dec2b0a), signature.HeaderPgpSignature.KeyId);
                Assert.Equal(-112, signature.ImmutableRegionSize);
                Assert.Equal(new byte[] { 0xd4, 0x8e, 0x07, 0xc5, 0x68, 0xbe, 0x27, 0xe2, 0x26, 0xcb, 0xb8, 0xaf, 0x0e, 0xf7, 0x32, 0xec }, signature.MD5Hash);
                Assert.Equal(new byte[] { 0xd7, 0x2d, 0x1e, 0x8a, 0x93, 0x26, 0x43, 0x14, 0x72, 0xa1, 0xe3, 0x9e, 0x1b, 0x79, 0x16, 0xad, 0x07, 0xcc, 0xc3, 0x1b }, signature.Sha1Hash);
            }
        }

        /// <summary>
        /// Tests the <see cref="RpmSignature.Verify(Stream)"/> method by using a well-known RPM package.
        /// </summary>
        [Fact]
        public void VerifyTest()
        {
            using (Stream stream = File.OpenRead(@"Rpm/libplist-2.0.1.151-1.1.x86_64.rpm"))
            using (Stream pgpKey = File.OpenRead(@"Rpm/_key.pub"))
            {
                var package = RpmPackageReader.Read(stream);
                var signature = new RpmSignature(package);
                signature.Verify(pgpKey);
            }
        }
    }
}
