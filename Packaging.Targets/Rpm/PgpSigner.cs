using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        /// <summary>
        /// Generates a <see cref="PgpSignature"/> as a digital signature for a certain payload.
        /// </summary>
        /// <param name="key">
        /// The key with which to sign the data.
        /// </param>
        /// <param name="payload">
        /// The payload to sign.
        /// </param>
        /// <returns>
        /// A <see cref="PgpSignature"/> which represents the signature.
        /// </returns>
        public static PgpSignature Sign(PgpPrivateKey key, Stream payload)
        {
            PgpV3SignatureGenerator signer = new PgpV3SignatureGenerator(PublicKeyAlgorithmTag.RsaGeneral, HashAlgorithmTag.Sha256);
            signer.InitSign(0, key);

            byte[] buffer = new byte[1024];

            int read;
            while ((read = payload.Read(buffer, 0, buffer.Length)) > 0)
            {
                signer.Update(buffer, 0, read);
            }

            return signer.Generate();
        }

        /// <summary>
        /// Generates a <see cref="PgpKeyRingGenerator"/>
        /// </summary>
        /// <param name="identity">
        /// The name of the identity.
        /// </param>
        /// <param name="password">
        /// The passphrase used to protect the keyring.</param>
        /// <returns>
        /// A <see cref="PgpKeyRingGenerator"/>.
        /// </returns>
        public static PgpKeyRingGenerator GenerateKeyRingGenerator(string identity, string password)
        {
            var rsaParams = new RsaKeyGenerationParameters(BigInteger.ValueOf(0x10001), new SecureRandom(), 2048, 12);
            var symmetricAlgorithms = new SymmetricKeyAlgorithmTag[] {
                    SymmetricKeyAlgorithmTag.Aes256,
                    SymmetricKeyAlgorithmTag.Aes192,
                    SymmetricKeyAlgorithmTag.Aes128
                }.Select(a => (int)a).ToArray();

            var hashAlgorithms = new HashAlgorithmTag[] {
                    HashAlgorithmTag.Sha256,
                    HashAlgorithmTag.Sha1,
                    HashAlgorithmTag.Sha384,
                    HashAlgorithmTag.Sha512,
                    HashAlgorithmTag.Sha224,
                }.Select(a => (int)a).ToArray();

            IAsymmetricCipherKeyPairGenerator generator = GeneratorUtilities.GetKeyPairGenerator("RSA");
            generator.Init(rsaParams);

            // Create the master (signing-only) key.
            PgpKeyPair masterKeyPair = new PgpKeyPair(
                PublicKeyAlgorithmTag.RsaSign,
                generator.GenerateKeyPair(),
                DateTime.UtcNow);

            PgpSignatureSubpacketGenerator masterSubpckGen
                = new PgpSignatureSubpacketGenerator();
            masterSubpckGen.SetKeyFlags(false, PgpKeyFlags.CanSign
                | PgpKeyFlags.CanCertify);
            masterSubpckGen.SetPreferredSymmetricAlgorithms(false, symmetricAlgorithms);
            masterSubpckGen.SetPreferredHashAlgorithms(false, hashAlgorithms);

            // Create a signing and encryption key for daily use.
            PgpKeyPair encKeyPair = new PgpKeyPair(
                PublicKeyAlgorithmTag.RsaGeneral,
                generator.GenerateKeyPair(),
                DateTime.UtcNow);

            PgpSignatureSubpacketGenerator encSubpckGen = new PgpSignatureSubpacketGenerator();
            encSubpckGen.SetKeyFlags(false, PgpKeyFlags.CanEncryptCommunications | PgpKeyFlags.CanEncryptStorage);

            masterSubpckGen.SetPreferredSymmetricAlgorithms(false, symmetricAlgorithms);
            masterSubpckGen.SetPreferredHashAlgorithms(false, hashAlgorithms);

            // Create the key ring.
            PgpKeyRingGenerator keyRingGen = new PgpKeyRingGenerator(
                PgpSignature.DefaultCertification,
                masterKeyPair,
                identity,
                SymmetricKeyAlgorithmTag.Aes128,
                password.ToCharArray(),
                true,
                masterSubpckGen.Generate(),
                null,
                new SecureRandom());

            // Add encryption subkey.
            keyRingGen.AddSubKey(encKeyPair, encSubpckGen.Generate(), null);

            return keyRingGen;
        }
    }
}
