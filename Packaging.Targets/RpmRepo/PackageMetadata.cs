using System.Collections.Generic;

namespace Packaging.Targets.RpmRepo
{
    /// <summary>
    /// Represents metadata of a package.
    /// </summary>
    public class PackageMetadata
    {
        /// <summary>
        /// Gets or sets the package ID. This is usually the SHA256 hash of the file.
        /// </summary>
        public string PkgId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the type of package. This is usually <c>rpm</c> to indicate a RPM package.
        /// </summary>
        public string Type
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the package.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the architecture which this package targets.
        /// </summary>
        public string Arch
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the epoch of the package version number.
        /// </summary>
        public int VersionEpoch
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the package version number.
        /// </summary>
        public string Version
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the package release number.
        /// </summary>
        public string Release
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the checksum type - for example, <c>sha256</c>.
        /// </summary>
        public string ChecksumType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the package checksum is used as the
        /// package identifier.
        /// </summary>
        public bool ChecksumIsPkgId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the checksum of the package.
        /// </summary>
        public string Checksum
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the package summary text.
        /// </summary>
        public string Summary
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the package description.
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the packager tool.
        /// </summary>
        public string Packager
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the URL of the package project.
        /// </summary>
        public string Url
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the time at which the package was generated, as a Unix time stamp, in seconds.
        /// </summary>
        public long FileTime
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the time at which the package was built, as a Unix time stamp, in seconds.
        /// </summary>
        public long BuildTime
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the package size.
        /// </summary>
        public long PackageSize
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the installed size of this package.
        /// </summary>
        public int InstalledSize
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the size of the CPIO archive embedded in this package.
        /// </summary>
        public int ArchiveSize
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the relative path to the package.
        /// </summary>
        public string Location
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the license used by this package.
        /// </summary>
        public string License
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the package vendor.
        /// </summary>
        public string Vendor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the package group.
        /// </summary>
        public string Group
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the host used to build this package.
        /// </summary>
        public string BuildHost
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the source RPM for this package, if any.
        /// </summary>
        public string SourceRpm
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the offset of the start of the RPM header in this file.
        /// </summary>
        public long HeaderStart
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the offset of th estart of the CPIO payload in this file.
        /// </summary>
        public long HeaderEnd
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a list of all dependencies.
        /// </summary>
        public List<RpmEntry> Requires
        { get; } = new List<RpmEntry>();

        /// <summary>
        /// Gets a list of all provided dependencies.
        /// </summary>
        public List<RpmEntry> Provides
        { get; } = new List<RpmEntry>();

        /// <summary>
        /// Gets a list of all files in this package.
        /// </summary>
        public List<string> Files
        { get; } = new List<string>();

        /// <summary>
        /// Gets the changelog entries for this pacakge.
        /// </summary>
        public List<ChangeLogEntry> ChangeLog
        { get; } = new List<ChangeLogEntry>();
    }
}
