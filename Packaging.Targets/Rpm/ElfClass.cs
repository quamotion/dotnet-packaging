namespace Packaging.Targets.Rpm
{
    /// <summary>
    /// Specifies whether an ELF file is 32-bit or 64-bit.
    /// </summary>
    internal enum ElfClass : byte
    {
        /// <summary>
        /// The file is 32-bit.
        /// </summary>
        Elf32 = 1,

        /// <summary>
        /// The file is 64-bit.
        /// </summary>
        Elf64 = 2
    }
}
