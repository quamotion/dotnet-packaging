namespace Packaging.Targets.IO
{
    /// <summary>
    /// Flags that control the decoding behavior of liblzma.
    /// </summary>
    internal enum LzmaDecodeFlags : uint
    {
        /// <summary>
        /// This flag makes <see cref="NativeMethods.lzma_code(ref LzmaStream, LzmaAction)"/> return
        /// <see cref="LzmaResult.NoCheck"/> if the input stream
        /// being decoded has no integrity check.
        /// </summary>
        TellNoCheck = 0x01,

        /// <summary>
        /// This flag makes <see cref="NativeMethods.lzma_code(ref LzmaStream, LzmaAction)"/> return
        /// <see cref="LzmaResult.UnsupportedCheck"/> if the input
        /// stream has an integrity check, but the type of the integrity check is not
        /// supported by this liblzma version or build. Such files can still be
        /// decoded, but the integrity check cannot be verified.
        /// </summary>
        TellUnsupportedCheck = 0x02,

        /// <summary>
        /// This flag makes <see cref="NativeMethods.lzma_code(ref LzmaStream, LzmaAction)"/> return
        /// <see cref="LzmaResult.GetCheck"/> as soon as the type
        /// of the integrity check is known.
        /// </summary>
        TellAnyCheck = 0x04,

        /// <summary>
        /// <para>
        /// This flag enables decoding of concatenated files with file formats that
        /// allow concatenating compressed files as is. From the formats currently
        /// supported by liblzma, only the .xz format allows concatenated files.
        /// Concatenated files are not allowed with the legacy .lzma format.
        /// </para>
        /// <para>
        /// This flag also affects the usage of the `action' argument for <see cref="NativeMethods.lzma_code(ref LzmaStream, LzmaAction)"/>
        /// When <see cref="Concatenated"/> is used, <see cref="NativeMethods.lzma_code(ref LzmaStream, LzmaAction)"/>  won't return
        /// <see cref="LzmaResult.StreamEnd"/> unless <see cref="LzmaAction.Finish"/> is used as `action'. Thus, the application has to set
        /// <see cref="LzmaAction.Finish"/> in the same way as it does when encoding.
        /// </para>
        /// </summary>
        Concatenated = 0x08
    }
}
