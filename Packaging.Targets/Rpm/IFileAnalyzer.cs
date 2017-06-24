using Packaging.Targets.IO;
using System.Collections.ObjectModel;

namespace Packaging.Targets.Rpm
{
    /// <summary>
    /// Provides a common interface for classes which are able to analyze the contents of a file and provide metadata for that file.
    /// </summary>
    internal interface IFileAnalyzer
    {
        /// <summary>
        /// Gets the <see cref="RpmFileFlags"/> which apply to this file.
        /// </summary>
        /// <param name="entry">
        /// The entry to analyze.
        /// <returns>
        /// The <see cref="RpmFileFlags"/> for the file.
        /// </returns>
        RpmFileFlags DetermineFlags(ArchiveEntry entry);

        /// <summary>
        /// Gets the dependencies for this file.
        /// </summary>
        /// <param name="entry">
        /// The entry to analyze.
        /// <returns>
        /// <returns>
        /// The dependencies for the file.
        /// </returns>
        Collection<PackageDependency> DetermineRequires(ArchiveEntry entry);

        /// <summary>
        /// Gets the dependencies fulfilled by this file.
        /// </summary>
        /// <param name="entry">
        /// The entry to analyze.
        /// <returns>
        /// <returns>
        /// The dependencies fulfilled by the file.
        /// </returns>
        Collection<PackageDependency> DetermineProvides(ArchiveEntry entry);

        /// <summary>
        /// Gets the <see cref="RpmFileColor"/> for this file.
        /// </summary>
        /// <param name="entry">
        /// The entry to analyze.
        /// <returns>tes of the file to analyze.
        /// </param>
        /// <returns>
        /// The <see cref="RpmFileColor"/> for the file.
        /// </returns>
        RpmFileColor DetermineColor(ArchiveEntry entry);

        /// <summary>
        /// Gets the class of this file.
        /// </summary>
        /// <param name="entry">
        /// The entry to analyze.
        /// <returns>
        /// <returns>
        /// The class of this file.
        /// </returns>
        string DetermineClass(ArchiveEntry entry);

        bool IsExecutable(ArchiveEntry entry);
    }
}
