using System;
using System.Runtime.InteropServices;

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
    public class NativeMethods
    {
        /// <summary>
        /// The name of the lzma library.
        /// </summary>
        /// <remarks>
        /// You can fetch liblzma from https://github.com/RomanBelkov/XZ.NET/blob/master/XZ.NET/liblzma.dll
        /// </remarks>
        private const string LibraryName = @"lzma";

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
        [DllImport(LibraryName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern LzmaResult lzma_stream_decoder(ref LzmaStream stream, ulong memLimit, LzmaDecodeFlags flags);

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
        /// <seealso href="https://github.com/nobled/xz/blob/master/src/liblzma/api/lzma/base.h"/>
        [DllImport(LibraryName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern LzmaResult lzma_code(ref LzmaStream stream, LzmaAction action);

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
        [DllImport(LibraryName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern LzmaResult lzma_stream_footer_decode(ref LzmaStreamFlags options, byte[] inp);

        /// <summary>
        /// Get the uncompressed size of the file
        /// </summary>
        /// <seealso href="https://github.com/nobled/xz/blob/master/src/liblzma/api/lzma/index.h"/>
        [DllImport(LibraryName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern ulong lzma_index_uncompressed_size(IntPtr i);

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
        [DllImport(LibraryName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint lzma_index_buffer_decode(ref IntPtr i, ref ulong memLimit, IntPtr allocator, byte[] indexBuffer, ref uint inPosition, ulong inSize);

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
        [DllImport(LibraryName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void lzma_index_end(IntPtr i, IntPtr allocator);

        /// <summary>
        /// Free memory allocated for the coder data structures
        /// </summary>
        /// <param name="stream">
        /// Pointer to lzma_stream that is at least initialized with LZMA_STREAM_INIT.
        /// </param>
        /// <remarks>
        /// After <see cref="lzma_end(ref LzmaStream)"/>, <see cref="LzmaStream.internalState"/> is guaranteed to be <see cref="IntPtr.Zero"/>.
        /// No other members of the <see cref="LzmaStream"/> structure are touched.
        /// zlib indicates an error if application end()s unfinished stream structure.
        /// liblzma doesn't do this, and assumes that
        /// application knows what it is doing.
        /// </remarks>
        /// <seealso href="https://github.com/nobled/xz/blob/master/src/liblzma/api/lzma/base.h"/>
        [DllImport(LibraryName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void lzma_end(ref LzmaStream stream);
    }
}
