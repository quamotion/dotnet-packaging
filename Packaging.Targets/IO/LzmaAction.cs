namespace Packaging.Targets.IO
{
    /// <summary>
    /// The `action' argument for lzma_code()
    /// </summary>
    /// <remarks>
    /// After the first use of <see cref="SyncFlush"/>, <see cref="FullBarrier"/> , <see cref="FullBarrier"/>,
    /// or <see cref="Finish"/> , the same `action' must is used until <see cref="NativeMethods.lzma_code(ref LzmaStream, LzmaAction)"/>  returns
    /// <see cref="LzmaResult.StreamEnd"/>. Also, the amount of input (that is, strm->avail_in) must
    /// not be modified by the application until <see cref="NativeMethods.lzma_code(ref LzmaStream, LzmaAction)"/> returns
    /// <see cref="LzmaResult.StreamEnd"/>. Changing the `action' or modifying the amount of input
    /// will make <see cref="NativeMethods.lzma_code(ref LzmaStream, LzmaAction)"/> return <see cref="LzmaResult.ProgError"/>.
    /// </remarks>
    internal enum LzmaAction
    {
        /// <summary>
        /// Continue coding
        /// </summary>
        /// <remarks>
        /// <para>
        /// Encoder: Encode as much input as possible. Some internal
        /// buffering will probably be done(depends on the filter
        /// chain in use), which causes latency: the input used won't
        /// usually be decodeable from the output of the same
        /// <see cref="NativeMethods.lzma_code(ref LzmaStream, LzmaAction)"/>  call.
        /// </para>
        /// <para>
        /// Decoder: Decode as much input as possible and produce as
        /// much output as possible.
        /// </para>
        /// </remarks>
        Run = 0,

        /// <summary>
        /// Make all the input available at output
        /// </summary>
        /// <remarks>
        /// <para>
        /// Normally the encoder introduces some latency.
        /// <see cref="SyncFlush"/>  forces all the buffered data to be
        /// available at output without resetting the internal
        /// </para>
        /// <para>
        /// state of the encoder.This way it is possible to use
        /// compressed stream for example for communication over
        /// network.
        /// </para>
        /// <para>
        /// Only some filters support <see cref="SyncFlush"/>. Trying to use
        /// <see cref="SyncFlush"/>  with filters that don't support it will
        /// make <see cref="NativeMethods.lzma_code(ref LzmaStream, LzmaAction)"/> return <see cref="LzmaResult.OptionsError"/>. For example,
        /// LZMA1 doesn't support <see cref="SyncFlush"/> but LZMA2 does.
        /// </para>
        /// <para>
        /// Using <see cref="SyncFlush"/> very often can dramatically reduce
        /// the compression ratio. With some filters (for example,
        /// LZMA2), fine-tuning the compression options may help
        /// mitigate this problem significantly (for example,
        /// match finder with LZMA2).
        /// </para>
        /// <para>
        /// Decoders don't support <see cref="SyncFlush"/>.
        /// </para>
        /// </remarks>
        SyncFlush = 1,

        /// <summary>
        /// Finish encoding of the current Block
        /// </summary>
        /// <remarks>
        /// <para>
        /// All the input data going to the current Block must have
        /// been given to the encoder (the last bytes can still be
        /// pending next_in). Call <see cref="NativeMethods.lzma_code(ref LzmaStream, LzmaAction)"/> with
        /// <see cref="FullFlush"/> until it returns <see cref="LzmaResult.StreamEnd"/>. Then continue normally
        /// with <see cref="Run"/> or finish the Stream with <see cref="Finish"/>.
        /// </para>
        /// <para>
        /// This action is currently supported only by Stream encoder
        /// and easy encoder (which uses Stream encoder). If there is
        /// no unfinished Block, no empty Block is created.
        /// </para>
        /// </remarks>
        FullFlush = 2,

        /// <summary>
        /// Finish the coding operation
        /// </summary>
        /// <remarks>
        /// <para>
        /// All the input data must have been given to the encoder
        /// (the last bytes can still be pending in next_in).
        /// Call <see cref="NativeMethods.lzma_code(ref LzmaStream, LzmaAction)"/>  with <see cref="Finish"/>  until it returns
        /// <see cref="LzmaResult.StreamEnd"/> . Once <see cref="Finish"/> has been used,
        /// the amount of input must no longer be changed by
        /// the application.
        /// </para>
        /// <para>
        /// When decoding, using <see cref="Finish"/> is optional unless the
        /// <see cref="LzmaDecodeFlags.Concatenated"/>  flag was used when the decoder was
        /// initialized. When <see cref="LzmaDecodeFlags.Concatenated"/> was not used, the only
        /// effect of <see cref="Finish"/> is that the amount of input must not
        /// be changed just like in the encoder.
        /// </para>
        /// </remarks>
        Finish = 3,

        /// <summary>
        /// Finish encoding of the current Block
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is like <see cref="FullFlush"/>  except that this doesn't
        /// necessarily wait until all the input has been made
        /// available via the output buffer. That is, <see cref="NativeMethods.lzma_code(ref LzmaStream, LzmaAction)"/>
        /// might return <see cref="LzmaResult.StreamEnd"/> as soon as all the input
        /// has been consumed (avail_in == 0).
        /// </para>
        /// <para>
        /// <see cref="FullFlush"/> is useful with a threaded encoder if
        /// one wants to split the .xz Stream into Blocks at specific
        /// offsets but doesn't care if the output isn't flushed
        /// immediately. Using <see cref="FullFlush"/> allows keeping
        /// the threads busy while <see cref="FullFlush"/> would make
        /// <see cref="NativeMethods.lzma_code(ref LzmaStream, LzmaAction)"/> wait until all the threads have finished
        /// until more data could be passed to the encoder.
        /// </para>
        /// <para>
        /// With a <see cref="LzmaStream"/>  initialized with the single-threaded
        /// lzma_stream_encoder() or lzma_easy_encoder(),
        /// <see cref="FullBarrier"/> is an alias for <see cref="FullFlush"/>.
        /// </para>
        /// </remarks>
        FullBarrier = 4
    }
}
