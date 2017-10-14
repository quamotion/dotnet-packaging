using System.Collections.Generic;

namespace Packaging.Targets.Pkg
{
    /// <summary>
    /// Represents a container of file entries.
    /// </summary>
    internal interface IEntryContainer
    {
        /// <summary>
        /// Gets the child entries.
        /// </summary>
        IEnumerable<FileEntry> Entries { get; }
    }
}
