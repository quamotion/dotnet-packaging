using System;

namespace Packaging.Targets.IO
{
#pragma warning disable SA1307 // Accessible fields must begin with upper-case letter
#pragma warning disable SA1310 // Field names must not contain underscore
#pragma warning disable CS0169 // The field '' is never used
#pragma warning disable IDE0051 // Remove unused private members
    /// <summary>
    /// Multithreading options
    /// </summary>
    internal struct LzmaMT
    {
        /// <summary>
        /// Set this to zero if no flags are wanted.
        /// </summary>
        /// <remarks>
        /// No flags are currently supported.
        /// </remarks>
        public uint flags;

        /// <summary>
        /// Number of worker threads to use
        /// </summary>
        public uint threads;

        /// <summary>
        /// Maximum uncompressed size of a Block
        /// </summary>
        /// <remarks>
        /// <para>
        /// The encoder will start a new .xz Block every block_size bytes.
        /// Using LZMA_FULL_FLUSH or LZMA_FULL_BARRIER with lzma_code()
        /// the caller may tell liblzma to start a new Block earlier.
        /// </para>
        /// <para>
        /// With LZMA2, a recommended block size is 2-4 times the LZMA2
        /// dictionary size. With very small dictionaries, it is recommended
        /// to use at least 1 MiB block size for good compression ratio, even
        /// if this is more than four times the dictionary size. Note that
        /// these are only recommendations for typical use cases; feel free
        /// to use other values. Just keep in mind that using a block size
        /// less than the LZMA2 dictionary size is waste of RAM.
        /// </para>
        /// <para>
        /// Set this to 0 to let liblzma choose the block size depending
        /// on the compression options. For LZMA2 it will be 3*dict_size
        /// or 1 MiB, whichever is more.
        /// </para>
        /// <para>
        /// For each thread, about 3 * block_size bytes of memory will be
        /// allocated. This may change in later liblzma versions. If so,
        /// the memory usage will probably be reduced, not increased.
        /// </para>
        /// </remarks>
        public ulong block_size;

        /// <summary>
        /// Timeout to allow lzma_code() to return early
        /// </summary>
        /// <remarks>
        /// <para>
        /// Multithreading can make liblzma to consume input and produce
        /// output in a very bursty way: it may first read a lot of input
        /// to fill internal buffers, then no input or output occurs for
        /// a while.
        /// </para>
        /// <para>
        /// In single-threaded mode, lzma_code() won't return until it has
        /// either consumed all the input or filled the output buffer. If
        /// this is done in multithreaded mode, it may cause a call
        /// lzma_code() to take even tens of seconds, which isn't acceptable
        /// in all applications.
        /// </para>
        /// <para>
        /// To avoid very long blocking times in lzma_code(), a timeout
        /// (in milliseconds) may be set here. If lzma_code() would block
        /// longer than this number of milliseconds, it will return with
        /// LZMA_OK. Reasonable values are 100 ms or more. The xz command
        /// line tool uses 300 ms.
        /// </para>
        /// <para>
        /// If long blocking times are fine for you, set timeout to a special
        /// value of 0, which will disable the timeout mechanism and will make
        /// lzma_code() block until all the input is consumed or the output
        /// buffer has been filled.
        /// </para>
        /// <para>
        /// Even with a timeout, lzma_code() might sometimes take
        /// somewhat long time to return. No timing guarantees
        /// are made.
        /// </para>
        /// </remarks>
        public uint timeout;

        /// <summary>
        /// Compression preset (level and possible flags)
        /// </summary>
        /// <remarks>
        /// <para>
        /// The preset is set just like with lzma_easy_encoder().
        /// </para>
        /// <para>
        /// The preset is ignored if filters below is non-NULL.
        /// </para>
        /// </remarks>
        public uint preset;

        /// <summary>
        /// Filter chain (alternative to a preset).
        /// </summary>
        /// <remarks>
        /// If this is NULL, the preset above is used. Otherwise the preset
        /// is ignored and the filter chain specified here is used.
        /// </remarks>
        public IntPtr filters;

        /// <summary>
        /// Integrity check type.
        /// </summary>
        /// <remarks>
        /// The xz command line tool defaults to LZMA_CHECK_CRC64, which is
        /// a good choice if you are unsure.
        /// </remarks>
        public LzmaCheck check;

        /// <summary>
        /// Reserved space to allow possible future extensions without
        /// breaking the ABI. You should not touch these, because the names
        /// of these variables may change. These are and will never be used
        /// with the currently supported options, so it is safe to leave these
        /// uninitialized.
        /// </summary>
        private readonly int reserved_enum1;
        private readonly int reserved_enum2;
        private readonly int reserved_enum3;
        private readonly int reserved_int1;
        private readonly int reserved_int2;
        private readonly int reserved_int3;
        private readonly int reserved_int4;
        private readonly ulong reserved_int5;
        private readonly ulong reserved_int6;
        private readonly ulong reserved_int7;
        private readonly ulong reserved_int8;
        private readonly IntPtr reserved_ptr1;
        private readonly IntPtr reserved_ptr2;
        private readonly IntPtr reserved_ptr3;
        private readonly IntPtr reserved_ptr4;
    }
#pragma warning restore SA1307 // Accessible fields must begin with upper-case letter
#pragma warning restore SA1310 // Field names must not contain underscore
#pragma warning restore CS0169 // The field '' is never used
#pragma warning restore IDE0051 // Remove unused private members
}
