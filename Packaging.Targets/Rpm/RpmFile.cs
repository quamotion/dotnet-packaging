using System;
using System.Collections.ObjectModel;

namespace Packaging.Targets.Rpm
{
    /// <summary>
    /// Represents a file in an RPM package.
    /// </summary>
    internal class RpmFile
    {
        /// <summary>
        /// Gets or sets the size of the file.
        /// </summary>
        public int Size
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the file mode.
        /// </summary>
        public LinuxFileMode Mode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the device ID of the file, if the file is a special file.
        /// </summary>
        public short Rdev
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the time at which the file was last modified.
        /// </summary>
        public DateTimeOffset ModifiedTime
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the MD5 hash of the file.
        /// </summary>
        public byte[] MD5Hash
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the file to which this file links.
        /// </summary>
        public string LinkTo
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets information which describes this file.
        /// </summary>
        public RpmFileFlags Flags
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the user name of the owner of the file.
        /// </summary>
        public string UserName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the group name of the owner of the file.
        /// </summary>
        public string GroupName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating how the file should be validated.
        /// </summary>
        public RpmVerifyFlags VerifyFlags
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the ID of device containing file.
        /// </summary>
        public int Device
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the inode number of the file.
        /// </summary>
        public int Inode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a per-file locale marker used to install only locale specific subsets of files when the package is installed.
        /// </summary>
        public string Lang
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or set sthe file color, a classification of file types.
        /// </summary>
        public RpmFileColor Color
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the class of the file.
        /// </summary>
        public string Class
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a list of all dependencies of this file.
        /// </summary>
        public Collection<Dependency> Dependencies
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return this.Name;
        }
    }
}
