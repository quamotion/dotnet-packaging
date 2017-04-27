using System.Runtime.InteropServices;

namespace Packaging.Targets.IO
{
    /// <summary>
    /// Options for encoding/decoding Stream Header and Stream Footer
    /// </summary>
    /// <seealso href="https://github.com/nobled/xz/blob/master/src/liblzma/api/lzma/stream_flags.h"/>
    [StructLayout(LayoutKind.Sequential)]
    internal struct LzmaStreamFlags
    {
        /// <summary>
        /// Stream Flags format version
        /// </summary>
        /// <remarks>
        /// <para>
        /// To prevent API and ABI breakages if new features are needed in
        /// Stream Header or Stream Footer, a version number is used to
        /// indicate which fields in this structure are in use.For now,
        /// version must always be zero.With non-zero version, the
        /// lzma_stream_header_encode() and lzma_stream_footer_encode()
        /// will return <see cref="LzmaResult.OptionsError"/>.
        /// </para>
        ///
        /// <para>
        /// lzma_stream_header_decode() and <see cref="NativeMethods.lzma_stream_footer_decode(ref LzmaStreamFlags, byte[])"/>
        /// will always set this to the lowest value that supports all the
        /// features indicated by the Stream Flags field.The application
        /// must check that the version number set by the decoding functions
        /// is supported by the application. Otherwise it is possible that
        /// the application will decode the Stream incorrectly.
        /// </para>
        /// </remarks>
        public readonly uint Version;

        /// <summary>
        /// Backward Size
        /// </summary>
        /// <remarks>
        /// <para>
        /// Backward Size must be a multiple of four bytes.In this Stream
        /// format version, Backward Size is the size of the Index field.
        /// </para>
        ///
        /// <para>
        /// Backward Size isn't actually part of the Stream Flags field, but
        /// it is convenient to include in this structure anyway. Backward
        /// Size is present only in the Stream Footer.There is no need to
        /// initialize backward_size when encoding Stream Header.
        /// </para>
        ///
        /// <para>
        /// lzma_stream_header_decode() always sets backward_size to
        /// LZMA_VLI_UNKNOWN so that it is convenient to use
        /// lzma_stream_flags_compare() when both Stream Header and Stream
        /// Footer have been decoded.
        /// </para>
        /// </remarks>
        public ulong BackwardSize;

        /// <summary>
        /// Check ID
        /// </summary>
        /// <remarks>
        /// This indicates the type of the integrity check calculated from
        /// uncompressed data.
        /// </remarks>
        public LzmaCheck Check;

        /// <summary>
        /// <para>
        /// Reserved space to allow possible future extensions without
        /// breaking the ABI.You should not touch these, because the
        /// names of these variables may change.
        /// </para>
        ///
        /// <para>
        /// (We will never be able to use all of these since Stream Flags
        /// is just two bytes plus Backward Size of four bytes.But it's
        /// nice to have the proper types when they are needed.)
        /// </para>
        /// </summary>
        private readonly int reservedEnum1;
        private readonly int reservedEnum2;
        private readonly int reservedEnum3;
        private readonly int reservedEnum4;
        private readonly char reservedBool1;
        private readonly char reservedBool2;
        private readonly char reservedBool3;
        private readonly char reservedBool4;
        private readonly char reservedBool5;
        private readonly char reservedBool6;
        private readonly char reservedBool7;
        private readonly char reservedBool8;
        private readonly uint reservedInt1;
        private readonly uint reservedInt2;
    }
}
