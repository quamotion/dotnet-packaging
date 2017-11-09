using System;

namespace Packaging.Targets.RpmRepo
{
    /// <summary>
    /// Represents a version number as used by RPM.
    /// </summary>
    public class RpmVersion
    {
        /// <summary>
        /// Gets or sets the version epoch.
        /// </summary>
        public int? Epoch
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the version number.
        /// </summary>
        public string Version
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the release number.
        /// </summary>
        public string Release
        {
            get;
            set;
        }

        /// <summary>
        /// Parses a version number from a string.
        /// </summary>
        /// <param name="version">
        /// The version number to parse.
        /// </param>
        /// <returns>
        /// An equivalent <see cref="RpmVersion"/> object.
        /// </returns>
        public static RpmVersion Parse(string version)
        {
            var parts = version.Split('-');

            if (parts.Length == 1)
            {
                return new RpmVersion()
                {
                    Version = parts[0],
                };
            }
            else if (parts.Length == 2)
            {
                return new RpmVersion()
                {
                    Version = parts[0],
                    Release = parts[1]
                };
            }
            else if (parts.Length == 3)
            {
                return new RpmVersion()
                {
                    Epoch = int.Parse(parts[0]),
                    Release = parts[2],
                    Version = parts[1]
                };
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(version));
            }
        }
    }
}
