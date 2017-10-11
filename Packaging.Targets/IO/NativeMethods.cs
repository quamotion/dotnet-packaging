using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using FunctionLoader = Packaging.Targets.Native.FunctionLoader;

namespace Packaging.Targets.IO
{
    /// <summary>
    /// Provides access to the liblzma API. liblzma is part of the xz suite.
    /// </summary>
    /// <remarks>
    /// You can download pre-built binaries from Windows from https://tukaani.org/xz/.
    /// </remarks>
    /// <seealso href="http://git.tukaani.org/?p=xz.git;a=blob;f=src/liblzma/api/lzma/index.h;h=dda60ec1c185c5c1f8475122ff35fbcf67f1bb6f;hb=446e4318fa79788e09299d5953b5dd428953d14b"/>
    /// <seealso href="https://github.com/nobled/xz/blob/master/src/liblzma/api/lzma/index.h"/>
    internal static class NativeMethods
    {
        /// <summary>
        /// The name of the lzma library.
        /// </summary>
        /// <remarks>
        /// You can fetch liblzma from https://github.com/RomanBelkov/XZ.NET/blob/master/XZ.NET/liblzma.dll
        /// </remarks>
        private const string LibraryName = @"lzma";

        private static lzma_stream_decoder_delegate lzma_stream_decoder_ptr;
        private static lzma_code_delegate lzma_code_ptr;
        private static lzma_stream_footer_decode_delegate lzma_stream_footer_decode_ptr;
        private static lzma_index_uncompressed_size_delegate lzma_index_uncompressed_size_ptr;
        private static lzma_index_buffer_decode_delegate lzma_index_buffer_decode_ptr;
        private static lzma_index_end_delegate lzma_index_end_ptr;
        private static lzma_end_delegate lzma_end_ptr;
        private static lzma_easy_encoder_delegate lzma_easy_encoder_ptr;
        private static lzma_stream_encoder_mt_delegate lzma_stream_encoder_mt_ptr;
        private static lzma_stream_buffer_bound_delegate lzma_stream_buffer_bound_ptr;
        private static lzma_easy_buffer_encode_delegate lzma_easy_buffer_encode_ptr;

        static NativeMethods()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if (RuntimeInformation.OSArchitecture != Architecture.X64)
                {
                    throw new InvalidOperationException(".NET packaging only supports 64-bit Windows processes");
                }
            }

            var libraryPath = Path.GetDirectoryName(typeof(NativeMethods).GetTypeInfo().Assembly.Location);
            var lzmaWindowsPath = Path.GetFullPath(Path.Combine(libraryPath, "../../runtimes/win7-x64/native/lzma.dll"));

            IntPtr library = FunctionLoader.LoadNativeLibrary(
                new string[] { lzmaWindowsPath, "lzma.dll" }, // lzma.dll is used when running unit tests.
                new string[] { "liblzma.so.5", "liblzma.so" },
                new string[] { "liblzma.dylib" });

            if (library == IntPtr.Zero)
            {
                throw new FileLoadException("Could not load liblzma. On Linux, make sure you've installed liblzma-dev or an equivalent package.");
            }

            lzma_stream_decoder_ptr = FunctionLoader.LoadFunctionDelegate<lzma_stream_decoder_delegate>(library, nameof(lzma_stream_decoder));
            lzma_code_ptr = FunctionLoader.LoadFunctionDelegate<lzma_code_delegate>(library, nameof(lzma_code));
            lzma_stream_footer_decode_ptr = FunctionLoader.LoadFunctionDelegate<lzma_stream_footer_decode_delegate>(library, nameof(lzma_stream_footer_decode));
            lzma_index_uncompressed_size_ptr = FunctionLoader.LoadFunctionDelegate<lzma_index_uncompressed_size_delegate>(library, nameof(lzma_index_uncompressed_size));
            lzma_index_buffer_decode_ptr = FunctionLoader.LoadFunctionDelegate<lzma_index_buffer_decode_delegate>(library, nameof(lzma_index_buffer_decode));
            lzma_index_end_ptr = FunctionLoader.LoadFunctionDelegate<lzma_index_end_delegate>(library, nameof(lzma_index_end));
            lzma_end_ptr = FunctionLoader.LoadFunctionDelegate<lzma_end_delegate>(library, nameof(lzma_end));
            lzma_easy_encoder_ptr = FunctionLoader.LoadFunctionDelegate<lzma_easy_encoder_delegate>(library, nameof(lzma_easy_encoder));
            lzma_stream_encoder_mt_ptr = FunctionLoader.LoadFunctionDelegate<lzma_stream_encoder_mt_delegate>(library, nameof(lzma_stream_encoder_mt));
            lzma_stream_buffer_bound_ptr = FunctionLoader.LoadFunctionDelegate<lzma_stream_buffer_bound_delegate>(library, nameof(lzma_stream_buffer_bound));
            lzma_easy_buffer_encode_ptr = FunctionLoader.LoadFunctionDelegate<lzma_easy_buffer_encode_delegate>(library, nameof(lzma_easy_buffer_encode));
        }

        private delegate LzmaResult lzma_stream_decoder_delegate(ref LzmaStream stream, ulong memLimit, LzmaDecodeFlags flags);

        private unsafe delegate LzmaResult lzma_easy_buffer_encode_delegate(uint preset, LzmaCheck check, void* allocator, byte[] @in, UIntPtr in_size, byte[] @out, UIntPtr* out_pos, UIntPtr out_size);

        private delegate UIntPtr lzma_stream_buffer_bound_delegate(UIntPtr uncompressed_size);

        private delegate LzmaResult lzma_stream_encoder_mt_delegate(ref LzmaStream stream, ref LzmaMT mt);

        private delegate LzmaResult lzma_easy_encoder_delegate(ref LzmaStream stream, uint preset, LzmaCheck check);

        private delegate void lzma_end_delegate(ref LzmaStream stream);

        private delegate void lzma_index_end_delegate(IntPtr i, IntPtr allocator);

        private delegate uint lzma_index_buffer_decode_delegate(ref IntPtr i, ref ulong memLimit, IntPtr allocator, byte[] indexBuffer, ref uint inPosition, ulong inSize);

        private delegate ulong lzma_index_uncompressed_size_delegate(IntPtr i);

        private delegate LzmaResult lzma_stream_footer_decode_delegate(ref LzmaStreamFlags options, byte[] inp);

        private delegate LzmaResult lzma_code_delegate(ref LzmaStream stream, LzmaAction action);

        /// <summary>
        /// Initialize .xz Stream decoder
        /// </summary>
        /// <param name="stream">
        /// Pointer to properly prepared <see cref="LzmaStream"/>
        /// </param>
        /// <param name="memLimit">
        /// Memory usage limit as bytes. Use UINT64_MAX
        /// to effectively disable the limiter.
        /// </param>
        /// <param name="flags">
        /// Bitwise-or of zero or more of the decoder flags:
        /// <see cref="LzmaDecodeFlags.TellNoCheck"/>, <see cref="LzmaDecodeFlags.TellUnsupportedCheck"/>,
        /// <see cref="LzmaDecodeFlags.TellAnyCheck"/>, <see cref="LzmaDecodeFlags.Concatenated"/>.
        /// </param>
        /// <returns>
        /// <see cref="LzmaResult.OK"/>: Initialization was successful,
        /// <see cref="LzmaResult.MemError"/>: Cannot allocate memory,
        /// <see cref="LzmaResult.OptionsError"/>: Unsupported flags,
        /// <see cref="LzmaResult.ProgError"/>.
        /// </returns>
        /// <seealso href="https://github.com/nobled/xz/blob/master/src/liblzma/api/lzma/container.h"/>
        public static LzmaResult lzma_stream_decoder(ref LzmaStream stream, ulong memLimit, LzmaDecodeFlags flags) => lzma_stream_decoder_ptr(ref stream, memLimit, flags);

        /// <summary>
        /// Encode or decode data
        /// </summary>
        /// <param name="stream">
        /// The <see cref="LzmaStream"/> for which to read the data.
        /// </param>
        /// <param name="action">
        /// The action to perform.
        /// </param>
        /// <returns>
        /// A <see cref="LzmaResult"/> value.
        /// </returns>
        /// <remarks>
        /// <para>
        /// Once the <see cref="LzmaStream"/>  has been successfully initialized (e.g. with
        /// lzma_stream_encoder()), the actual encoding or decoding is done
        /// using this function.The application has to update <see cref="LzmaStream.NextIn"/>,
        /// <see cref="LzmaStream.AvailIn"/>, <see cref="LzmaStream.NextOut"/>, and <see cref="LzmaStream.AvailOut"/> to pass input
        /// to and get output from liblzma.
        /// </para>
        /// <para>
        /// See the description of the coder-specific initialization function to find
        /// out what <paramref name="action"/> values are supported by the coder.
        /// </para>
        /// </remarks>
        public static LzmaResult lzma_code(ref LzmaStream stream, LzmaAction action) => lzma_code_ptr(ref stream, action);

        /// <summary>
        /// Decode Stream Footer
        /// </summary>
        /// <param name="options">
        /// Target for the decoded Stream Header options.
        /// </param>
        /// <param name="inp">
        /// Beginning of the input buffer of
        /// LZMA_STREAM_HEADER_SIZE bytes.
        /// </param>
        /// <returns>
        /// <see cref="LzmaResult.OK"/>: Decoding was successful.
        /// <see cref="LzmaResult.FormatError"/>: Magic bytes don't match, thus the given buffer cannot be Stream Footer.
        /// <see cref="LzmaResult.DataError"/>: CRC32 doesn't match, thus the Stream Footer is corrupt.
        /// <see cref="LzmaResult.OptionsError"/>: Unsupported options are present in Stream Footer.
        /// </returns>
        /// <remarks>
        /// If Stream Header was already decoded successfully, but
        /// decoding Stream Footer returns <see cref="LzmaResult.FormatError"/>, the
        /// application should probably report some other error message
        /// than "file format not recognized", since the file more likely
        /// is corrupt(possibly truncated). Stream decoder in liblzma
        /// uses <see cref="LzmaResult.DataError"/> in this situation.
        /// </remarks>
        /// <seealso href="https://github.com/nobled/xz/blob/master/src/liblzma/api/lzma/stream_flags.h"/>
        public static LzmaResult lzma_stream_footer_decode(ref LzmaStreamFlags options, byte[] inp) => lzma_stream_footer_decode_ptr(ref options, inp);

        /// <summary>
        /// Get the uncompressed size of the file
        /// </summary>
        /// <seealso href="https://github.com/nobled/xz/blob/master/src/liblzma/api/lzma/index.h"/>
        public static ulong lzma_index_uncompressed_size(IntPtr i) => lzma_index_uncompressed_size_ptr(i);

        /// <summary>
        /// Single-call .xz Index decoder
        /// </summary>
        /// <param name="i">If decoding succeeds, <paramref name="i"/> will point to a new lzma_index, which the application has to
        /// later free with lzma_index_end(). If an error occurs, <paramref name="i"/> will be <see cref="IntPtr.Zero"/>. The old value of <paramref name="i"/>
        /// is always ignored and thus doesn't need to be initialized by the caller.
        /// </param>
        /// <param name="memLimit">
        /// Pointer to how much memory the resulting lzma_index is allowed to require. The value
        /// pointed by this pointer is modified if and only if <see cref="LzmaResult.MemlimitError"/> is returned.
        /// </param>
        /// <param name="allocator">
        /// Pointer to lzma_allocator, or <see cref="IntPtr.Zero"/> to use malloc()
        /// </param>
        /// <param name="indexBuffer">
        /// Beginning of the input buffer
        /// </param>
        /// <param name="inPosition">
        /// The next byte will be read from in[*in_pos]. *in_pos is updated only if decoding succeeds.
        /// </param>
        /// <param name="inSize">
        /// Size of the input buffer; the first byte that won't be read is in[in_size].
        /// </param>
        /// <returns>
        /// <see cref="LzmaResult.OK"/>: Decoding was successful.
        /// <see cref="LzmaResult.MemError"/>,
        /// <see cref="LzmaResult.MemlimitError"/>: Memory usage limit was reached. The minimum required memlimit value was stored to* memlimit.
        /// <see cref="LzmaResult.DataError"/>,
        /// <see cref="LzmaResult.ProgError"/>.
        /// </returns>
        /// <seealso href="https://github.com/nobled/xz/blob/master/src/liblzma/api/lzma/index.h"/>
        public static uint lzma_index_buffer_decode(ref IntPtr i, ref ulong memLimit, IntPtr allocator, byte[] indexBuffer, ref uint inPosition, ulong inSize)
            => lzma_index_buffer_decode_ptr(ref i, ref memLimit, allocator, indexBuffer, ref inPosition, inSize);

        /// <summary>
        /// Deallocate lzma_index
        /// </summary>
        /// <param name="i">
        /// The index to deallocate
        /// </param>
        /// <param name="allocator">
        /// The allocated used to allocate the memory.
        /// </param>
        /// <remarks>
        /// If <paramref name="i"/> is <see cref="IntPtr.Zero"/>, this does nothing.
        /// </remarks>
        /// <seealso href="https://github.com/nobled/xz/blob/master/src/liblzma/api/lzma/index.h"/>
        public static void lzma_index_end(IntPtr i, IntPtr allocator) => lzma_index_end_ptr(i, allocator);

        /// <summary>
        /// Free memory allocated for the coder data structures
        /// </summary>
        /// <param name="stream">
        /// Pointer to lzma_stream that is at least initialized with LZMA_STREAM_INIT.
        /// </param>
        /// <remarks>
        /// After <see cref="lzma_end(ref LzmaStream)"/>, <see cref="LzmaStream.InternalState"/> is guaranteed to be <see cref="IntPtr.Zero"/>.
        /// No other members of the <see cref="LzmaStream"/> structure are touched.
        /// zlib indicates an error if application end()s unfinished stream structure.
        /// liblzma doesn't do this, and assumes that
        /// application knows what it is doing.
        /// </remarks>
        /// <seealso href="https://github.com/nobled/xz/blob/master/src/liblzma/api/lzma/base.h"/>
        public static void lzma_end(ref LzmaStream stream) => lzma_end_ptr(ref stream);

        /// <summary>
        /// <para>
        /// Initialize .xz Stream encoder using a preset number.
        /// </para>
        /// <para>
        /// This function is intended for those who just want to use the basic features if liblzma(that is, most developers out there).
        /// </para>
        /// <para>
        /// If initialization fails(return value is not LZMA_OK), all the memory allocated for *strm by liblzma is always freed.Thus, there is no need to call lzma_end() after failed initialization.
        /// </para>
        /// <para>If initialization succeeds, use lzma_code() to do the actual encoding.Valid values for `action' (the second argument of lzma_code()) are LZMA_RUN, LZMA_SYNC_FLUSH, LZMA_FULL_FLUSH, and LZMA_FINISH. In future, there may be compression levels or flags that don't support LZMA_SYNC_FLUSH.
        /// </para>
        /// </summary>
        /// <param name="stream">
        /// Pointer to lzma_stream that is at least initialized with LZMA_STREAM_INIT.
        /// </param>
        /// <param name="preset">
        /// Compression preset to use. A preset consist of level number and zero or more flags. Usually flags aren't used, so preset is simply a number [0, 9] which match the options -0 ... -9 of the xz command line tool. Additional flags can be be set using bitwise-or with the preset level number, e.g. 6 | LZMA_PRESET_EXTREME.
        /// </param>
        /// <param name="check">
        /// Integrity check type to use. See check.h for available checks. The xz command line tool defaults to LZMA_CHECK_CRC64, which is a good choice if you are unsure. LZMA_CHECK_CRC32 is good too as long as the uncompressed file is not many gigabytes.
        /// </param>
        /// <returns>
        /// <para>
        /// - LZMA_OK: Initialization succeeded. Use lzma_code() to encode your data.
        /// </para>
        /// <para>- LZMA_MEM_ERROR: Memory allocation failed.
        /// </para>
        /// <para>
        /// - LZMA_OPTIONS_ERROR: The given compression preset is not supported by this build of liblzma.
        /// </para>
        /// <para>
        /// - LZMA_UNSUPPORTED_CHECK: The given check type is not supported by this liblzma build.
        /// </para>
        /// <para>
        /// - LZMA_PROG_ERROR: One or more of the parameters have values that will never be valid. For example, strm == NULL.
        /// </para>
        /// </returns>
        public static LzmaResult lzma_easy_encoder(ref LzmaStream stream, uint preset, LzmaCheck check) => lzma_easy_encoder_ptr(ref stream, preset, check);

        /// <summary>
        /// <para>
        /// Initialize multithreaded .xz Stream encoder
        /// </para>
        /// <para>
        /// This provides the functionality of lzma_easy_encoder() and
        /// lzma_stream_encoder() as a single function for multithreaded use.
        /// </para>
        /// <para>
        /// The supported actions for lzma_code() are LZMA_RUN, LZMA_FULL_FLUSH,
        /// LZMA_FULL_BARRIER, and LZMA_FINISH. Support for LZMA_SYNC_FLUSH might be
        /// added in the future.
        /// </para>
        /// </summary>
        /// <param name="stream">
        /// Pointer to properly prepared lzma_stream
        /// </param>
        /// <param name="mt">
        /// Pointer to multithreaded compression options
        /// </param>
        /// <returns>
        /// A <see cref="LzmaResult"/> value which indicates success or failure.
        /// </returns>
        public static LzmaResult lzma_stream_encoder_mt(ref LzmaStream stream, ref LzmaMT mt) => lzma_stream_encoder_mt_ptr(ref stream, ref mt);

        /// <summary>
        /// <para>
        /// Calculate output buffer size for single-call Stream encoder
        /// </para>
        /// <para>
        /// When trying to compress uncompressible data, the encoded size will be slightly bigger than the input data.This function calculates how much output buffer space is required to be sure that lzma_stream_buffer_encode() doesn't return LZMA_BUF_ERROR.
        /// </para>
        /// <para>
        /// The calculated value is not exact, but it is guaranteed to be big enough.The actual maximum output space required may be slightly smaller (up to about 100 bytes). This should not be a problem in practice.
        /// </para>
        /// <para>
        /// If the calculated maximum size doesn't fit into size_t or would make the Stream grow past LZMA_VLI_MAX (which should never happen in practice), zero is returned to indicate the error.
        /// </para>
        /// </summary>
        /// <param name="uncompressed_size">
        /// The uncompressed size.
        /// </param>
        /// <returns>
        /// The buffer output size.
        /// </returns>
        /// <remarks>
        /// The limit calculated by this function applies only to single-call encoding. Multi-call encoding may (and probably will) have larger maximum expansion when encoding uncompressible data. Currently there is no function to calculate the maximum expansion of multi-call encoding.
        /// </remarks>
        public static UIntPtr lzma_stream_buffer_bound(UIntPtr uncompressed_size) => lzma_stream_buffer_bound_ptr(uncompressed_size);

        /// <summary>
        /// Single-call .xz Stream encoding using a preset number.
        /// </summary>
        /// <param name="preset">
        /// Compression preset to use. See the description in lzma_easy_encoder().
        /// </param>
        /// <param name="check">
        /// Type of the integrity check to calculate from uncompressed data.
        /// </param>
        /// <param name="allocator">
        /// lzma_allocator for custom allocator functions. Set to NULL to use malloc() and free().
        /// </param>
        /// <param name="in">
        /// Beginning of the input buffer
        /// </param>
        /// <param name="in_size">
        /// Size of the input buffer
        /// </param>
        /// <param name="out">
        /// Beginning of the output buffer
        /// </param>
        /// <param name="out_pos">
        /// The next byte will be written to out[*out_pos]. *out_pos is updated only if encoding succeeds.
        /// </param>
        /// <param name="out_size">
        /// Size of the out buffer; the first byte into which no data is written to is out[out_size].
        /// </param>
        /// <returns>
        /// <para>
        /// - LZMA_OK: Encoding was successful.
        /// </para>
        /// <para>
        /// - LZMA_BUF_ERROR: Not enough output buffer space.
        /// </para>
        /// <para>
        /// - LZMA_OPTIONS_ERROR
        /// </para>
        /// <para>
        /// - LZMA_MEM_ERROR
        /// </para>
        /// <para>
        /// - LZMA_DATA_ERROR
        /// </para>
        /// <para>
        /// - LZMA_PROG_ERROR
        /// </para>
        /// </returns>
        /// <remarks>
        /// The maximum required output buffer size can be calculated with lzma_stream_buffer_bound()
        /// </remarks>
        public static unsafe LzmaResult lzma_easy_buffer_encode(uint preset, LzmaCheck check, void* allocator, byte[] @in, UIntPtr in_size, byte[] @out, UIntPtr* out_pos, UIntPtr out_size)
            => lzma_easy_buffer_encode_ptr(preset, check, allocator, @in, in_size, @out, out_pos, out_size);
    }
}
