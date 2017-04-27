using System;

namespace Packaging.Targets.IO
{
    /// <summary>
    /// Return values used by several functions in liblzma.
    /// </summary>
    internal enum LzmaResult : uint
    {
        /// <summary>
        /// The operation completed successfully.
        /// </summary>
        OK = 0,

        /// <summary>
        /// End of stream was reached
        /// </summary>
        /// <remarks>
        /// <para>
        /// In encoder, <see cref="LzmaAction.SyncFlush"/>, <see cref="LzmaAction.FullFlush"/>, or
        /// <see cref="LzmaAction.Finish"/> was finished. In decoder, this indicates
        /// that all the data was successfully decoded.
        /// </para>
        /// <para>
        /// In all cases, when <see cref="StreamEnd"/>  is returned, the last
        /// output bytes should be picked from <see cref="LzmaStream.NextOut"/>.
        /// </para>
        /// </remarks>
        StreamEnd = 1,

        /// <summary>
        /// Input stream has no integrity check
        /// </summary>
        /// <remarks>
        /// <para>
        /// This return value can be returned only if the
        /// <see cref="LzmaDecodeFlags.TellNoCheck"/>  flag was used when initializing
        /// the decoder. <see cref="NoCheck"/> is just a warning, and
        /// the decoding can be continued normally.
        /// </para>
        /// <para>
        /// It is possible to call lzma_get_check() immediately after
        /// lzma_code has returned <see cref="NoCheck"/>. The result will
        /// naturally be <see cref="LzmaCheck.None"/>, but the possibility to call
        /// lzma_get_check() may be convenient in some applications.
        /// </para>
        /// </remarks>
        NoCheck = 2,

        /// <summary>
        /// Cannot calculate the integrity check
        /// </summary>
        /// <remarks>
        /// <para>
        /// The usage of this return value is different in encoders
        /// and decoders.
        /// </para>
        /// <para>
        /// Encoders can return this value only from the initialization
        /// function. If initialization fails with this value, the
        /// encoding cannot be done, because there's no way to produce
        /// output with the correct integrity check.
        /// </para>
        /// <para>
        /// Decoders can return this value only from lzma_code() and
        /// only if the <see cref="LzmaDecodeFlags.TellUnsupportedCheck"/> flag was used when
        /// initializing the decoder. The decoding can still be
        /// continued normally even if the check type is unsupported,
        /// but naturally the check will not be validated, and possible
        /// errors may go undetected.
        /// </para>
        /// <para>
        /// With decoder, it is possible to call lzma_get_check()
        /// immediately after lzma_code() has returned
        /// <see cref="UnsupportedCheck"/>. This way it is possible to find
        /// out what the unsupported Check ID was.
        /// </para>
        /// </remarks>
        UnsupportedCheck = 3,

        /// <summary>
        /// Integrity check type is now available
        /// </summary>
        /// <remarks>
        /// This value can be returned only by the lzma_code() function
        /// and only if the decoder was initialized with the
        /// <see cref="LzmaDecodeFlags.TellAnyCheck"/> flag. <see cref="GetCheck"/> tells the
        /// application that it may now call lzma_get_check() to find
        /// out the Check ID. This can be used, for example, to
        /// implement a decoder that accepts only files that have
        /// strong enough integrity check.
        /// </remarks>
        GetCheck = 4,

        /// <summary>
        /// Cannot allocate memory
        /// </summary>
        /// <remarks>
        /// <para>
        /// Memory allocation failed, or the size of the allocation
        /// would be greater than SIZE_MAX.
        /// </para>
        /// <para>
        /// Due to internal implementation reasons, the coding cannot
        /// be continued even if more memory were made available after
        /// <see cref="MemError"/>.
        /// </para>
        /// </remarks>
        MemError = 5,

        /// <summary>
        /// Memory usage limit was reached
        /// </summary>
        /// <remarks>
        /// Decoder would need more memory than allowed by the
        /// specified memory usage limit. To continue decoding,
        /// the memory usage limit has to be increased with
        /// lzma_memlimit_set().
        /// </remarks>
        MemlimitError = 6,

        /// <summary>
        /// File format not recognized
        /// </summary>
        /// <remarks>
        /// The decoder did not recognize the input as supported file
        /// format. This error can occur, for example, when trying to
        /// decode .lzma format file with <see cref="NativeMethods.lzma_stream_decoder(ref LzmaStream, ulong, LzmaDecodeFlags)"/>,
        /// because <see cref="NativeMethods.lzma_stream_decoder(ref LzmaStream, ulong, LzmaDecodeFlags)"/> accepts only the .xz format.
        /// </remarks>
        FormatError = 7,

        /// <summary>
        /// Invalid or unsupported options
        /// </summary>
        /// <remarks>
        /// <para>
        /// Invalid or unsupported options, for example
        ///  - unsupported filter(s) or filter options; or
        ///  - reserved bits set in headers (decoder only).
        /// </para>
        /// <para>
        /// Rebuilding liblzma with more features enabled, or
        /// upgrading to a newer version of liblzma may help.
        /// </para>
        /// </remarks>
        OptionsError = 8,

        /// <summary>
        /// Data is corrupt
        /// </summary>
        /// <remarks>
        /// <para>
        /// The usage of this return value is different in encoders
        /// and decoders. In both encoder and decoder, the coding
        /// cannot continue after this error.
        /// </para>
        /// <para>
        /// Encoders return this if size limits of the target file
        /// format would be exceeded. These limits are huge, thus
        /// getting this error from an encoder is mostly theoretical.
        /// For example, the maximum compressed and uncompressed
        /// size of a .xz Stream is roughly 8 EiB (2^63 bytes).
        /// </para>
        /// <para>
        /// Decoders return this error if the input data is corrupt.
        /// This can mean, for example, invalid CRC32 in headers
        /// or invalid check of uncompressed data.
        /// </para>
        /// </remarks>
        DataError = 9,

        /// <summary>
        /// No progress is possible
        /// </summary>
        /// <remarks>
        /// <para>
        /// This error code is returned when the coder cannot consume
        /// any new input and produce any new output.The most common
        /// reason for this error is that the input stream being
        /// decoded is truncated or corrupt.
        /// </para>
        /// <para>
        /// This error is not fatal. Coding can be continued normally
        /// by providing more input and/or more output space, if
        /// possible.
        /// </para>
        /// <para>
        /// Typically the first call to <see cref="NativeMethods.lzma_code(ref LzmaStream, LzmaAction)"/> that can do no
        /// progress returns <see cref="OK"/> instead of <see cref="BufferError"/>. Only
        /// the second consecutive call doing no progress will return
        /// <see cref="BufferError"/> . This is intentional.
        /// </para>
        /// <para>
        /// With zlib, Z_BUF_ERROR may be returned even if the
        /// application is doing nothing wrong, so apps will need
        /// to handle Z_BUF_ERROR specially. The above hack
        /// guarantees that liblzma never returns <see cref="BufferError"/>
        /// to properly written applications unless the input file
        /// is truncated or corrupt. This should simplify the
        /// applications a little.
        /// </para>
        /// </remarks>
        BufferError = 10,

        /// <summary>
        /// Programming error
        /// </summary>
        /// <remarks>
        /// <para>
        /// This indicates that the arguments given to the function are
        /// invalid or the internal state of the decoder is corrupt.
        ///   - Function arguments are invalid or the structures
        ///     pointed by the argument pointers are invalid
        ///     e.g. if <see cref="LzmaStream.NextOut"/> has been set to <see cref="IntPtr.Zero"/> and
        ///     <see cref="LzmaStream.AvailOut"/> &gt; 0 when calling <see cref="NativeMethods.lzma_code(ref LzmaStream, LzmaAction)"/>.
        ///   - lzma_/// functions have been called in wrong order
        ///     e.g. <see cref="NativeMethods.lzma_code(ref LzmaStream, LzmaAction)"/>  was called right after <see cref="NativeMethods.lzma_end(ref LzmaStream)"/>.
        ///   - If errors occur randomly, the reason might be flaky
        ///     hardware.
        /// </para>
        /// <para>
        /// If you think that your code is correct, this error code
        /// can be a sign of a bug in liblzma. See the documentation
        /// how to report bugs.
        /// </para>
        /// </remarks>
        ProgError = 11
    }
}
