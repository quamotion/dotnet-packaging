namespace Packaging.Targets.Pkg
{
    /// <summary>
    /// Represents the type of file.
    /// </summary>
    internal enum BomFileType : byte
    {
        /// <summary>
        /// The file is a regular file.
        /// </summary>
        File = 1,

        /// <summary>
        /// The file is a directory.
        /// </summary>
        Directory = 2,

        /// <summary>
        /// The file is a symbolic link.
        /// </summary>
        Link = 3,

        /// <summary>
        /// The file is a device.
        /// </summary>
        Device = 4
    }
}
