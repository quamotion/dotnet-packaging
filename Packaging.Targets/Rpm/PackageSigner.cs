using Org.BouncyCastle.Bcpg.OpenPgp;
using System;
using System.IO;

namespace Packaging.Targets.Rpm
{
    /// <summary>
    /// Generates <see cref="PgpSignature"/> values using a <see cref="Stream"/> and a
    /// <see cref="PgpPrivateKey"/>.
    /// </summary>
    internal class PackageSigner : IPackageSigner
    {
        private readonly PgpPrivateKey privateKey;

        public PackageSigner(PgpPrivateKey privateKey)
        {
            if (privateKey == null)
            {
                throw new ArgumentNullException(nameof(privateKey));
            }

            this.privateKey = privateKey;
        }

        public PgpSignature Sign(Stream payload)
        {
            return PgpSigner.Sign(this.privateKey, payload);
        }
    }
}
