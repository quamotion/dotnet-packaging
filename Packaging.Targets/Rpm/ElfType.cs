namespace Packaging.Targets.Rpm
{
    /// <summary>
    /// Represents the different types of ELF files.
    /// </summary>
    internal enum ElfType : ushort
    {
        /// <summary>
        /// The file is relocatable.
        /// </summary>
        Relocatable = 1,

        /// <summary>
        /// The file is executable.
        /// </summary>
        Executable = 2,

        /// <summary>
        /// The file is shared.
        /// </summary>
        Shared = 3,

        /// <summary>
        /// The file is core.
        /// </summary>
        Core = 4
    }
}
