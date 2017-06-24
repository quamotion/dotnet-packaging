namespace Packaging.Targets
{
    /// <summary>
    /// Determines whether a file is copied to its output directory or not.
    /// </summary>
    internal enum CopyToDirectoryValue
    {
        /// <summary>
        /// The file is never copied.
        /// </summary>
        DoNotCopy,

        /// <summary>
        /// The file is alwasy copied.
        /// </summary>
        Always,

        /// <summary>
        /// The file is copied only if it is newer.
        /// </summary>
        PreserveNewest
    }
}
