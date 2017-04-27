namespace Packaging.Targets.IO
{
    /// <summary>
    /// Type of the integrity check (Check ID)
    /// </summary>
    /// <remarks>
    /// The.xz format supports multiple types of checks that are calculated
    /// from the uncompressed data. They vary in both speed and ability to
    /// detect errors.
    /// </remarks>
    /// <seealso href="https://github.com/nobled/xz/blob/master/src/liblzma/api/lzma/check.h"/>
    internal enum LzmaCheck
    {
        /// <summary>
        /// No Check is calculated.
        /// </summary>
        None = 0,

        /// <summary>
        /// CRC32 using the polynomial from the IEEE 802.3 standard
        /// </summary>
        Crc32 = 1,

        /// <summary>
        /// CRC64 using the polynomial from the ECMA-182 standard
        /// </summary>
        Crc64 = 4,

        /// <summary>
        /// SHA-256
        /// </summary>
        Sha256 = 10
    }
}
