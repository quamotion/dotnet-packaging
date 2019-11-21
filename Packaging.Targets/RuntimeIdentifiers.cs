using System;
using System.Runtime.InteropServices;

namespace Packaging.Targets
{
    /// <summary>
    /// Provides methods for working with .NET Runtime Identifiers.
    /// </summary>
    public static class RuntimeIdentifiers
    {
        /// <summary>
        /// Parses a runtime identifier.
        /// </summary>
        /// <param name="runtimeId">
        /// The runtime identifier to parse.
        /// </param>
        /// <param name="osName">
        /// The operating system name embedded in the runtime identifier.
        /// </param>
        /// <param name="version">
        /// The operating system version embedded in the runtime identifier.
        /// </param>
        /// <param name="architecture">
        /// The processor architecture embedded in the runtime identifier.
        /// </param>
        /// <param name="qualifiers">
        /// Any additional qualifiers (such as <c>aot</c>) embedded in the runtime identifier.
        /// </param>
        public static void ParseRuntimeId(string runtimeId, out string osName, out string version, out Architecture? architecture, out string qualifiers)
        {
            osName = null;
            version = null;
            architecture = null;
            qualifiers = null;

            if (string.IsNullOrEmpty(runtimeId))
            {
                return;
            }

            // We use the following convention in all newly-defined RIDs. Some RIDs (win7-x64, win8-x64) predate this convention and don't follow it, but all new RIDs should follow it. 
            // [os name].[version]-[architecture]-[additional qualifiers]
            // See https://github.com/dotnet/corefx/blob/master/pkg/Microsoft.NETCore.Platforms/readme.md#naming-convention
            int versionSeparator = runtimeId.IndexOf('.');
            if (versionSeparator >= 0)
            {
                osName = runtimeId.Substring(0, versionSeparator);
            }
            else
            {
                osName = null;
            }

            // As a special case, for "linux-musl", we consider "-musl" to be part of the osName
            int muslSeparator = runtimeId.IndexOf("-musl", versionSeparator + 1);
            int architectureSeparator = runtimeId.IndexOf('-', muslSeparator + 1);
            if (architectureSeparator >= 0)
            {
                if (versionSeparator >= 0)
                {
                    version = runtimeId.Substring(versionSeparator + 1, architectureSeparator - versionSeparator - 1);
                }
                else
                {
                    osName = runtimeId.Substring(0, architectureSeparator);
                    version = null;
                }

                qualifiers = runtimeId.Substring(architectureSeparator + 1);
            }
            else
            {
                if (versionSeparator >= 0)
                {
                    version = runtimeId.Substring(versionSeparator + 1);
                }
                else
                {
                    osName = runtimeId;
                    version = null;
                }

                qualifiers = null;
            }

            // As a special case, os names like win7, win81 and win10 are processed separately
            if (osName.StartsWith("win") && osName.Length > 3)
            {
                version = osName.Substring(3);
                osName = "win";
            }

            architecture = null;

            if (!string.IsNullOrEmpty(qualifiers))
            {
                string architectureString = qualifiers;
                qualifiers = null;

                architectureSeparator = architectureString.IndexOf('-');
                if (architectureSeparator > 0)
                {
                    qualifiers = architectureString.Substring(architectureSeparator + 1);
                    architectureString = architectureString.Substring(0, architectureSeparator);
                }

                // As a special case, "armel" is mapped to "arm" for now
                if (architectureString == "armel")
                {
                    architectureString = "arm";
                }

                if (Enum.TryParse<Architecture>(architectureString, ignoreCase: true, out Architecture parsedArchitecture))
                {
                    architecture = parsedArchitecture;
                }
                else
                {
                    // E.g. in the case of "win-aot"
                    qualifiers = architectureString;
                }
            }
        }
    }
}
