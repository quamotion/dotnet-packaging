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
        /// <param name="fileName">
        /// The name of the file to analyze.
        /// </param>
        /// <param name="fileHeader">
        /// The <see cref="CpioHeader"/> of the file to analyze.
        /// </param>
        /// <param name="header">
        /// The first bytes of the file to analyze.
        /// </param>
        /// <returns>
        /// The <see cref="RpmFileFlags"/> for the file.
        /// </returns>
        RpmFileFlags DetermineFlags(string fileName, CpioHeader fileHeader, byte[] header);

        /// <summary>
        /// Gets the dependencies for this file.
        /// </summary>
        /// <param name="fileName">
        /// The name of the file to analyze.
        /// </param>
        /// <param name="fileHeader">
        /// The <see cref="CpioHeader"/> of the file to analyze.
        /// </param>
        /// <param name="header">
        /// The first bytes of the file to analyze.
        /// </param>
        /// <returns>
        /// The dependencies for the file.
        /// </returns>
        Collection<string> DetermineRequires(string fileName, CpioHeader fileHeader, byte[] header);

        /// <summary>
        /// Gets the dependencies fulfilled by this file.
        /// </summary>
        /// <param name="fileName">
        /// The name of the file to analyze.
        /// </param>
        /// <param name="fileHeader">
        /// The <see cref="CpioHeader"/> of the file to analyze.
        /// </param>
        /// <param name="header">
        /// The first bytes of the file to analyze.
        /// </param>
        /// <returns>
        /// The dependencies fulfilled by the file.
        /// </returns>
        Collection<string> DetermineProvides(string fileName, CpioHeader fileHeader, byte[] header);

        /// <summary>
        /// Gets the <see cref="RpmFileColor"/> for this file.
        /// </summary>
        /// <param name="fileName">
        /// The name of the file to analyze.
        /// </param>
        /// <param name="fileHeader">
        /// The <see cref="CpioHeader"/> of the file to analyze.
        /// </param>
        /// <param name="header">
        /// The first bytes of the file to analyze.
        /// </param>
        /// <returns>
        /// The <see cref="RpmFileColor"/> for the file.
        /// </returns>
        RpmFileColor DetermineColor(string fileName, CpioHeader fileHeader, byte[] header);

        /// <summary>
        /// Gets the class of this file.
        /// </summary>
        /// <param name="fileName">
        /// The name of the file to analyze.
        /// </param>
        /// <param name="fileHeader">
        /// The <see cref="CpioHeader"/> of the file to analyze.
        /// </param>
        /// <param name="header">
        /// The first bytes of the file to analyze.
        /// </param>
        /// <returns>
        /// The class of this file.
        /// </returns>
        string DetermineClass(string fileName, CpioHeader fileHeader, byte[] header);
    }
}
