namespace Packaging.Targets.Rpm
{
    /// <summary>
    /// Represents the various hashing algorithms.
    /// </summary>
    internal enum PgpHashAlgo
    {
        /// <summary>
        /// The MD5 hashing algorithm.
        /// </summary>
        PGPHASHALGO_MD5 = 1,

        /// <summary>
        /// The SHA1 hashing algorithm.
        /// </summary>
        PGPHASHALGO_SHA1 = 2,
        PGPHASHALGO_RIPEMD160 = 3,
        PGPHASHALGO_MD2 = 5,
        PGPHASHALGO_TIGER192 = 6,
        PGPHASHALGO_HAVAL_5_160 = 7,

        /// <summary>
        /// The SHA 256 hashing algorithm.
        /// </summary>
        PGPHASHALGO_SHA256 = 8,

        /// <summary>
        /// The SHA 384 hashing algorithm.
        /// </summary>
        PGPHASHALGO_SHA384 = 9,
        /// <summary>
        /// The SHA 512 hashing algorithm.
        /// </summary>
        PGPHASHALGO_SHA512 = 10,
    }
}
