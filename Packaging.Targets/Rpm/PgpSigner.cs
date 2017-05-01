using Org.BouncyCastle.Bcpg.OpenPgp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Packaging.Targets.Rpm
{
    /// <summary>
    /// Provides helper methods for working with PGP signatures.
    /// </summary>
    internal class PgpSigner
    {
        /// <summary>
        /// Verifies a PGP signature.
        /// </summary>
        /// <param name="signature">
        /// The PGP signature to verify.
        /// </param>
        /// <param name="publicKey">
        /// A <see cref="Stream"/> which contains the PGP public key.
        /// </param>
        /// <param name="payload">
        /// The payload for which the signature was generated.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the signature is valid; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool VerifySignature(PgpSignature signature, Stream publicKey, Stream payload)
        {
            PgpPublicKeyRingBundle keyRing = new PgpPublicKeyRingBundle(PgpUtilities.GetDecoderStream(publicKey));
            PgpPublicKey key = keyRing.GetPublicKey(signature.KeyId);

            return VerifySignature(signature, key, payload);
        }

        /// <summary>
        /// Verifies a PGP signature.
        /// </summary>
        /// <param name="signature">
        /// The PGP signature to verify.
        /// </param>
        /// <param name="key">
        /// The public key of the signer.
        /// </param>
        /// <param name="payload">
        /// The payload for which the signature was generated.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the signature is valid; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool VerifySignature(PgpSignature signature, PgpPublicKey key, Stream payload)
        {
            signature.InitVerify(key);

            byte[] buffer = new byte[1024];

            int read;
            while ((read = payload.Read(buffer, 0, buffer.Length)) > 0)
            {
                signature.Update(buffer, 0, read);
            }

            return signature.Verify();
        }
    }
}
