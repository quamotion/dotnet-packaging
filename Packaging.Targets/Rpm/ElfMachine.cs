namespace Packaging.Targets.Rpm
{
#pragma warning disable SA1300 // Element must begin with upper-case letter
    /// <summary>
    /// Specifies target instruction set architecture.
    /// </summary>
    internal enum ElfMachine
    {
        /// <summary>
        /// The file is generic.
        /// </summary>
        Generic = 0,

        /// <summary>
        /// The file targets the SPARC architecture.
        /// </summary>
        SPARC = 2,

        /// <summary>
        /// The file targets the x64 architecture.
        /// </summary>
        x64 = 3,

        /// <summary>
        /// The file targets the MIPS architecture.
        /// </summary>
        MIPS = 8,

        /// <summary>
        /// The file targets the PowerPC architecture.
        /// </summary>
        PowerPC = 0x14,

        /// <summary>
        /// The file targets the ARM architecture.
        /// </summary>
        ARM = 0x28,

        /// <summary>
        /// The file targets the SuperH architecture.
        /// </summary>
        SuperH = 0x2A,

        /// <summary>
        /// THe file targets the IA-64 architecture.
        /// </summary>
        IA64 = 0x32,

        /// <summary>
        /// The file targets the x86-64 architecture.
        /// </summary>
        x8664 = 0x3E,

        /// <summary>
        /// The file targets the AArch64 (ARM64) architecture.
        /// </summary>
        AArch64 = 0xb7,

        /// <summary>
        /// The file targets the RISC V architecture.
        /// </summary>
        RiscV = 0xF3
    }
#pragma warning restore SA1300 // Element must begin with upper-case letter
}
