namespace Packaging.Targets.Rpm
{
#pragma warning disable SA1307 // Accessible fields must begin with upper-case letter
    /// <summary>
    /// Represents the first fields of a header of an ELF file.
    /// </summary>
    /// <seealso href="https://en.wikipedia.org/wiki/Executable_and_Linkable_Format#File_header"/>
    internal struct ElfHeader
    {
        /// <summary>
        /// The <see cref="ElfClass"/>. Indicates whether this is 64 or 32-bit.
        /// </summary>
        public ElfClass @class;

        /// <summary>
        /// Set to 1 or 2 to indicate the endianness of the file.
        /// </summary>
        public byte data;

        /// <summary>
        /// Should always be 1 to indicate the original ELF file format.
        /// </summary>
        public byte version;

        /// <summary>
        /// Identifies the target operating system ABI.
        /// </summary>
        public byte osAbi;

        /// <summary>
        /// Further specifies the ABI version. Its interpretation depends on the target ABI.
        /// </summary>
        public byte abiVersion;

        /// <summary>
        /// Indicates whether the object is relocatable, executable, shared, or core.
        /// </summary>
        public ElfType type;

        /// <summary>
        /// Specifies target instruction set architecture.
        /// </summary>
        public ElfMachine machine;
    }
#pragma warning restore SA1307 // Accessible fields must begin with upper-case letter
}
