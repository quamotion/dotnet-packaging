using Org.BouncyCastle.Bcpg.OpenPgp;
using Packaging.Targets.Rpm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace Packaging.Targets.Tests.Rpm
{
    internal class DummySigner : IPackageSigner
    {
        public Dictionary<string, PgpSignature> Signatures
        { get; } = new Dictionary<string, PgpSignature>();

        public void Add(string hash, byte[] rawSignature)
        {
            using (MemoryStream stream = new MemoryStream(rawSignature))
            using (var signatureStream = PgpUtilities.GetDecoderStream(stream))
            {
                PgpObjectFactory pgpFactory = new PgpObjectFactory(signatureStream);
                PgpSignatureList signatureList = (PgpSignatureList)pgpFactory.NextPgpObject();
                PgpSignature signature = signatureList[0];

                this.Signatures.Add(hash, signature);
            }
        }

        public PgpSignature Sign(Stream payload)
        {
            using (SHA1 hash = SHA1.Create())
            {
                string key = BitConverter.ToString(hash.ComputeHash(payload)).Replace("-", string.Empty);
                return this.Signatures[key];
            }
        }
    }
}
