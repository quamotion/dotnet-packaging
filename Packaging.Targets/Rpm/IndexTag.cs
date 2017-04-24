namespace Packaging.Targets.Rpm
{
    internal enum IndexTag
    {
        /// <summary>
        /// The signature tag differentiates a signature header from a metadata header, and identifies the original contents of the signature header.
        /// </summary>
        RPMTAG_HEADERSIGNATURES = 62,

        /// <summary>
        /// This tag contains an index record which specifies the portion of the Header Record which was used for the calculation of a signature.
        /// This data shall be preserved or any header-only signature will be invalidated.
        /// </summary>
        RPMTAG_HEADERIMMUTABLE = 63,

        /// <summary>
        /// Contains a list of locales for which strings are provided in other parts of the package.
        /// </summary>
        RPMTAG_HEADERI18NTABLE = 100,

        /// <summary>
        /// This tag specifies the combined size of the Header and Payload sections.
        /// </summary>
        RPMSIGTAG_SIZE = 1000,

        /// <summary>
        /// This tag specifies the uncompressed size of the Payload archive, including the cpio headers.
        /// </summary>
        RPMSIGTAG_PAYLOADSIZE = 1007,

        /// <summary>
        /// This index contains the SHA1 checksum of the entire Header Section, including the Header Record, Index Records and Header store.
        /// </summary>
        RPMSIGTAG_SHA1 = 269,

        /// <summary>
        /// This tag specifies the 128-bit MD5 checksum of the combined Header and Archive sections.
        /// </summary>
        RPMSIGTAG_MD5 = 1004,

        /// <summary>
        /// The tag contains the DSA signature of the Header section. The data is formatted as a Version 3 Signature Packet as specified in
        /// RFC 2440: OpenPGP Message Format. If this tag is present, then the SIGTAG_GPG tag shall also be present.
        /// </summary>
        RPMSIGTAG_DSA = 267,

        /// <summary>
        /// The tag contains the RSA signature of the Header section.The data is formatted as a Version 3 Signature Packet as specified in
        /// RFC 2440: OpenPGP Message Format. If this tag is present, then the SIGTAG_PGP shall also be present.
        /// </summary>
        RPMSIGTAG_RSA = 268,

        /// <summary>
        /// This tag specifies the RSA signature of the combined Header and Payload sections. The data is formatted as a Version 3 Signature Packet
        /// as specified in RFC 2440: OpenPGP Message Format.
        /// </summary>
        RPMSIGTAG_PGP = 1002,

        /// <summary>
        /// The tag contains the DSA signature of the combined Header and Payload sections. The data is formatted as a Version 3 Signature Packet
        /// as specified in RFC 2440: OpenPGP Message Format.
        /// </summary>
        RPMSIGTAG_GPG = 1005,

        /// <summary>
        /// This tag specifies the name of the package.
        /// </summary>
        RPMTAG_NAME = 1000,

        /// <summary>
        /// This tag specifies the version of the package.
        /// </summary>
        RPMTAG_VERSION = 1001,

        /// <summary>
        /// This tag specifies the release of the package.
        /// </summary>
        RPMTAG_RELEASE = 1002,

        /// <summary>
        /// This tag specifies the summary description of the package. The summary value pointed to by this index record contains a one line description of the package.
        /// </summary>
        RPMTAG_SUMMARY = 1004,

        /// <summary>
        /// This tag specifies the description of the package. The description value pointed to by this index record contains a full desription of the package.
        /// </summary>
        RPMTAG_DESCRIPTION = 1005,
        /// <summary>
        /// This tag specifies the sum of the sizes of the regular files in the archive.
        /// </summary>
        RPMTAG_SIZE = 1009,

        /// <summary>
        /// A string containing the name of the distribution on which the package was built.
        /// </summary>
        RPMTAG_DISTRIBUTION = 1010,

        /// <summary>
        /// A string containing the name of the organization that produced the package.
        /// </summary>
        RPMTAG_VENDOR = 1011,

        /// <summary>
        /// This tag specifies the license which applies to this package.
        /// </summary>
        RPMTAG_LICENSE = 1014,

        /// <summary>
        /// A string identifying the tool used to build the package.
        /// </summary>
        RPMTAG_PACKAGER = 1015,

        /// <summary>
        /// This tag specifies the administrative group to which this package belongs.
        /// </summary>
        RPMTAG_GROUP = 1016,

        /// <summary>
        /// Generic package information URL.
        /// </summary>
        RPMTAG_URL = 1020,

        /// <summary>
        /// This tag specifies the OS of the package. The OS value pointed to by this index record shall be "linux".
        /// </summary>
        RPMTAG_OS = 1021,

        /// <summary>
        /// This tag specifies the architecture of the package. The architecture value pointed to by this index record is defined in architecture specific LSB specification.
        /// </summary>
        RPMTAG_ARCH = 1022,

        /// <summary>
        /// This tag specifies the name of the source RPM.
        /// </summary>
        RPMTAG_SOURCERPM = 1044,

        /// <summary>
        /// This tag specifies the uncompressed size of the Payload archive, including the cpio headers.
        /// </summary>
        RPMTAG_ARCHIVESIZE = 1046,

        /// <summary>
        /// This tag indicates the version of RPM tool used to build this package. The value is unused.
        /// </summary>
        RPMTAG_RPMVERSION = 1064,

        /// <summary>
        /// This tag contains an opaque string whose contents are undefined.
        /// </summary>
        RPMTAG_COOKIE = 1094,

        /// <summary>
        /// URL for package.
        /// </summary>
        RPMTAG_DISTURL = 1123,

        /// <summary>
        /// This tag specifies the format of the Archive section. The format value pointed to by this index record shall be 'cpio'.
        /// </summary>
        RPMTAG_PAYLOADFORMAT = 1124,

        /// <summary>
        /// This tag specifies the compression used on the Archive section. The compression value pointed to by this index record shall be 'gzip'.
        /// </summary>
        RPMTAG_PAYLOADCOMPRESSOR = 1125,

        /// <summary>
        /// This tag indicates the compression level used for the Payload. This value shall always be '9'.
        /// </summary>
        RPMTAG_PAYLOADFLAGS = 1126,

        /// <summary>
        /// This tag specifies the filenames when not in a compressed format as determined by the absence of rpmlib(CompressedFileNames) in the RPMTAG_REQUIRENAME index.
        /// </summary>
        RPMTAG_OLDFILENAMES = 1027,

        /// <summary>
        /// This tag specifies the size of each file in the archive.
        /// </summary>
        RPMTAG_FILESIZES = 1028,

        /// <summary>
        /// This tag specifies the mode of each file in the archive.
        /// </summary>
        RPMTAG_FILEMODES = 1030,

        /// <summary>
        /// This tag specifies the device number from which the file was copied.
        /// </summary>
        RPMTAG_FILERDEVS = 1033,

        /// <summary>
        /// This tag specifies the modification time in seconds since the epoch of each file in the archive.
        /// </summary>
        RPMTAG_FILEMTIMES = 1034,

        /// <summary>
        /// This tag specifies the ASCII representation of the MD5 sum of the corresponding file contents. This value is empty if the corresponding archive entry is not a regular file.
        /// </summary>
        RPMTAG_FILEMD5S = 1035,

        /// <summary>
        /// The target for a symlink, otherwise NULL.
        /// </summary>
        RPMTAG_FILELINKTOS = 1036,

        /// <summary>
        /// This tag specifies the bit(s) to classify and control how files are to be installed. See below.
        /// </summary>
        RPMTAG_FILEFLAGS = 1037,

        /// <summary>
        /// This tag specifies the owner of the corresponding file.
        /// </summary>
        RPMTAG_FILEUSERNAME = 1039,

        /// <summary>
        /// This tag specifies the group of the corresponding file.
        /// </summary>
        RPMTAG_FILEGROUPNAME = 1040,

        /// <summary>
        /// This tag specifies the 16 bit device number from which the file was copied.
        /// </summary>
        RPMTAG_FILEDEVICES = 1095,

        /// <summary>
        /// This tag specifies the inode value from the original file system on the the system on which it was built.
        /// </summary>
        RPMTAG_FILEINODES = 1096,

        /// <summary>
        /// This tag specifies a per-file locale marker used to install only locale specific subsets of files when the package is installed.
        /// </summary>
        RPMTAG_FILELANGS = 1097,

        /// <summary>
        /// This tag specifies the index into the array provided by the RPMTAG_DIRNAMES Index which contains the directory name for the corresponding filename.
        /// </summary>
        RPMTAG_DIRINDEXES = 1116,

        /// <summary>
        /// This tag specifies the base portion of the corresponding filename.
        /// </summary>
        RPMTAG_BASENAMES = 1117,

        /// <summary>
        /// One of RPMTAG_OLDFILENAMES or the tuple RPMTAG_DIRINDEXES,RPMTAG_BASENAMES,RPMTAG_DIRNAMES shall be present, but not both.
        /// </summary>
        RPMTAG_DIRNAMES = 1118,

        /// <summary>
        /// The file is a configuration file, and an existing file should be saved during a package upgrade operation and not removed during a pakage removal operation.
        /// </summary>
        RPMFILE_CONFIG = 1 << 0,

        /// <summary>
        /// The file contains documentation.
        /// </summary>
        RPMFILE_DOC = 1 << 1,

        /// <summary>
        /// This value is reserved for future use; conforming packages may not use this flag.
        /// </summary>
        RPMFILE_DONOTUSE = 1 << 2,
        /// <summary>
        /// The file need not exist on the installed system.
        /// </summary>
        RPMFILE_MISSINGOK = 1 << 3,

        /// <summary>
        /// Similar to the RPMFILE_CONFIG, this flag indicates that during an upgrade operation the original file on the system should not be altered.
        /// </summary>
        RPMFILE_NOREPLACE = 1 << 4,

        /// <summary>
        /// The file is a package specification.
        /// </summary>
        RPMFILE_SPECFILE = 1 << 5,

        /// <summary>
        /// The file is not actually included in the payload, but should still be considered as a part of the package. For example, a log file generated by the application at run time.
        /// </summary>
        RPMFILE_GHOST = 1 << 6,

        /// <summary>
        /// The file contains the license conditions.
        /// </summary>
        RPMFILE_LICENSE = 1 << 7,

        /// <summary>
        /// The file contains high level notes about the package.
        /// </summary>
        RPMFILE_README = 1 << 8,

        /// <summary>
        /// The corresponding file is not a part of the package, and should not be installed.
        /// </summary>
        RPMFILE_EXCLUDE = 1 << 9,

        /// <summary>
        /// This tag indicates the name of the dependency provided by this package.
        /// </summary>
        RPMTAG_PROVIDENAME = 1047,

        /// <summary>
        /// Bits(s) to specify the dependency range and context.
        /// </summary>
        RPMTAG_REQUIREFLAGS = 1048,

        /// <summary>
        /// This tag indicates the dependencies for this package.
        /// </summary>
        RPMTAG_REQUIRENAME = 1049,

        /// <summary>
        /// This tag indicates the versions associated with the values found in the RPMTAG_REQUIRENAME Index.
        /// </summary>
        RPMTAG_REQUIREVERSION = 1050,

        /// <summary>
        /// Bits(s) to specify the conflict range and context.
        /// </summary>
        RPMTAG_CONFLICTFLAGS = 1053,

        /// <summary>
        /// This tag indicates the conflicting dependencies for this package.
        /// </summary>
        RPMTAG_CONFLICTNAME = 1054,

        /// <summary>
        /// This tag indicates the versions associated with the values found in the RPMTAG_CONFLICTNAME Index.
        /// </summary>
        RPMTAG_CONFLICTVERSION = 1055,

        /// <summary>
        /// This tag indicates the obsoleted dependencies for this package.
        /// </summary>
        RPMTAG_OBSOLETENAME = 1090,

        /// <summary>
        /// Bits(s) to specify the conflict range and context.
        /// </summary>
        RPMTAG_PROVIDEFLAGS = 1112,

        /// <summary>
        /// This tag indicates the versions associated with the values found in the RPMTAG_PROVIDENAME Index.
        /// </summary>
        RPMTAG_PROVIDEVERSION = 1113,

        /// <summary>
        /// Bits(s) to specify the conflict range and context.
        /// </summary>
        RPMTAG_OBSOLETEFLAGS = 1114,

        /// <summary>
        /// This tag indicates the versions associated with the values found in the RPMTAG_OBSOLETENAME Index.
        /// </summary>
        RPMTAG_OBSOLETEVERSION = 1115,

        /// <summary>
        /// This tag specifies the time as seconds since the epoch at which the package was built.
        /// </summary>
        RPMTAG_BUILDTIME = 1006,

        /// <summary>
        /// This tag specifies the hostname of the system on which which the package was built.
        /// </summary>
        RPMTAG_BUILDHOST = 1007,

        /// <summary>
        /// This tag specifies the bit(s) to control how files are to be verified after install, specifying which checks should be performed.
        /// </summary>
        RPMTAG_FILEVERIFYFLAGS = 1045,

        /// <summary>
        /// This tag specifies the Unix time in seconds since the epoch associated with each entry in the Changelog file.
        /// </summary>
        RPMTAG_CHANGELOGTIME = 1080,

        /// <summary>
        /// This tag specifies the name of who made a change to this package.
        /// </summary>
        RPMTAG_CHANGELOGNAME = 1081,

        /// <summary>
        /// This tag specifies the changes asssociated with a changelog entry.
        /// </summary>
        RPMTAG_CHANGELOGTEXT = 1082,

        /// <summary>
        /// This tag indicates additional flags which may have been passed to the compiler when building this package.
        /// </summary>
        RPMTAG_OPTFLAGS = 1122,

        /// <summary>
        /// This tag contains an opaque string whose contents are undefined.
        /// </summary>
        RPMTAG_RHNPLATFORM = 1131,

        /// <summary>
        /// This tag contains an opaque string whose contents are undefined.
        /// </summary>
        RPMTAG_PLATFORM = 1132
    }
}
