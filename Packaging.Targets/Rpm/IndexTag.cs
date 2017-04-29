namespace Packaging.Targets.Rpm
{
    /// <seealso href="https://github.com/rpm-software-management/rpm/blob/master/lib/rpmtag.h"/>
    internal enum IndexTag
    {
        RPMTAG_HEADERIMAGE = 61,

        /// <summary>
        /// The signature tag differentiates a signature header from a metadata header, and identifies the original contents of the signature header.
        /// </summary>
        RPMTAG_HEADERSIGNATURES = 62,

        /// <summary>
        /// This tag contains an index record which specifies the portion of the Header Record which was used for the calculation of a signature.
        /// This data shall be preserved or any header-only signature will be invalidated.
        /// </summary>
        RPMTAG_HEADERIMMUTABLE = 63,

        RPMTAG_HEADERREGIONS = 64,

        /// <summary>
        /// Contains a list of locales for which strings are provided in other parts of the package.
        /// </summary>
        RPMTAG_HEADERI18NTABLE = 100,

        // Header tags
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

        RPMTAG_EPOCH = 1003,

        /// <summary>
        /// This tag specifies the summary description of the package. The summary value pointed to by this index record contains a one line description of the package.
        /// </summary>
        RPMTAG_SUMMARY = 1004,

        /// <summary>
        /// This tag specifies the description of the package. The description value pointed to by this index record contains a full desription of the package.
        /// </summary>
        RPMTAG_DESCRIPTION = 1005,

        RPMTAG_BUILDTIME = 1006,

        RPMTAG_BUILDHOST = 1007,

        RPMTAG_INSTALLTIME = 1008,

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

        RPMTAG_GIF = 1012,

        RPMTAG_XPM = 1013,

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

        RPMTAG_CHANGELOG = 1017,

        RPMTAG_SOURCE = 1018,

        RPMTAG_PATCH = 1019,

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

        RPMTAG_PREIN = 1023,

        RPMTAG_POSTIN = 1024,

        RPMTAG_PREUN = 1025,

        RPMTAG_POSTUN = 1026,

        /// <summary>
        /// This tag specifies the filenames when not in a compressed format as determined by the absence of rpmlib(CompressedFileNames) in the RPMTAG_REQUIRENAME index.
        /// </summary>
        RPMTAG_OLDFILENAMES = 1027,

        /// <summary>
        /// This tag specifies the size of each file in the archive.
        /// </summary>
        RPMTAG_FILESIZES = 1028,

        RPMTAG_FILESTATES = 1029,

        /// <summary>
        /// This tag specifies the mode of each file in the archive.
        /// </summary>
        RPMTAG_FILEMODES = 1030,

        RPMTAG_FILEUIDS = 1031,

        RPMTAG_FILEGIDS = 1032,

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

        RPMTAG_ROOT = 1038,

        /// <summary>
        /// This tag specifies the owner of the corresponding file.
        /// </summary>
        RPMTAG_FILEUSERNAME = 1039,

        /// <summary>
        /// This tag specifies the group of the corresponding file.
        /// </summary>
        RPMTAG_FILEGROUPNAME = 1040,

        RPMTAG_EXCLUDE = 1041,

        RPMTAG_EXCLUSIVE = 1042,

        RPMTAG_ICON = 1043,

        /// <summary>
        /// This tag specifies the name of the source RPM.
        /// </summary>
        RPMTAG_SOURCERPM = 1044,

        /// <summary>
        /// This tag specifies the bit(s) to control how files are to be verified after install, specifying which checks should be performed.
        /// </summary>
        RPMTAG_FILEVERIFYFLAGS = 1045,

        /// <summary>
        /// This tag specifies the uncompressed size of the Payload archive, including the cpio headers.
        /// </summary>
        RPMTAG_ARCHIVESIZE = 1046,

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

        RPMTAG_NOSOURCE = 1051,

        RPMTAG_NOPATCH = 1052,

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

        RPMTAG_DEFAULTPREFIX = 1056,

        RPMTAG_BUILDROOT = 1057,

        RPMTAG_INSTALLPREFIX = 1058,

        RPMTAG_EXCLUDEARCH = 1059,

        RPMTAG_EXCLUDEOS = 1060,

        RPMTAG_EXCLUSIVEARCH = 1061,

        RPMTAG_EXCLUSIVEOS = 1062,

        RPMTAG_AUTOREQPROV = 1063,

        /// <summary>
        /// This tag indicates the version of RPM tool used to build this package. The value is unused.
        /// </summary>
        RPMTAG_RPMVERSION = 1064,

        RPMTAG_TRIGGERSCRIPTS = 1065,

        RPMTAG_TRIGGERNAME = 1066,

        RPMTAG_TRIGGERVERSION = 1067,

        RPMTAG_TRIGGERFLAGS = 1068,

        RPMTAG_TRIGGERINDEX = 1069,

        RPMTAG_VERIFYSCRIPT = 1079,

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

        RPMTAG_BROKENMD5 = 1083,

        RPMTAG_PREREQ = 1084,

        RPMTAG_PREINPROG = 1085,

        RPMTAG_POSTINPROG = 1086,

        RPMTAG_PREUNPROG = 1087,

        RPMTAG_POSTUNPROG = 1088,

        RPMTAG_BUILDARCHS = 1089,

        /// <summary>
        /// This tag indicates the obsoleted dependencies for this package.
        /// </summary>
        RPMTAG_OBSOLETENAME = 1090,

        RPMTAG_VERIFYSCRIPTPROG = 1091,

        RPMTAG_TRIGGERSCRIPTPROG = 1092,

        RPMTAG_DOCDIR = 1093,

        /// <summary>
        /// This tag contains an opaque string whose contents are undefined.
        /// </summary>
        RPMTAG_COOKIE = 1094,

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
        RPMTAG_PREFIXES = 1098,
        RPMTAG_INSTPREFIXES = 1099,
        RPMTAG_TRIGGERIN = 1100,
        RPMTAG_TRIGGERUN = 1101,
        RPMTAG_TRIGGERPOSTUN = 1102,
        RPMTAG_AUTOREQ = 1103,
        RPMTAG_AUTOPROV = 1104,
        RPMTAG_CAPABILITY = 1105,
        RPMTAG_SOURCEPACKAGE = 1106,
        RPMTAG_OLDORIGFILENAMES = 1107,
        RPMTAG_BUILDPREREQ = 1108,
        RPMTAG_BUILDREQUIRES = 1109,
        RPMTAG_BUILDCONFLICTS = 1110,
        RPMTAG_BUILDMACROS = 1111,

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
        RPMTAG_ORIGDIRINDEXES = 1119,
        RPMTAG_ORIGBASENAMES = 1120,
        RPMTAG_ORIGDIRNAMES = 1121,

        /// <summary>
        /// This tag indicates additional flags which may have been passed to the compiler when building this package.
        /// </summary>
        RPMTAG_OPTFLAGS = 1122,

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
        RPMTAG_INSTALLCOLOR = 1127,
        RPMTAG_INSTALLTID = 1128,
        RPMTAG_REMOVETID = 1129,
        RPMTAG_SHA1RHN = 1130,

        /// <summary>
        /// This tag contains an opaque string whose contents are undefined.
        /// </summary>
        RPMTAG_RHNPLATFORM = 1131,


        /// <summary>
        /// This tag contains an opaque string whose contents are undefined.
        /// </summary>
        RPMTAG_PLATFORM = 1132,

        RPMTAG_PATCHESNAME = 1133,
        RPMTAG_PATCHESFLAGS = 1134,
        RPMTAG_PATCHESVERSION = 1135,
        RPMTAG_CACHECTIME = 1136,
        RPMTAG_CACHEPKGPATH = 1137,
        RPMTAG_CACHEPKGSIZE = 1138,
        RPMTAG_CACHEPKGMTIME = 1139,
        RPMTAG_FILECOLORS = 1140,
        RPMTAG_FILECLASS = 1141,
        RPMTAG_CLASSDICT = 1142,
        RPMTAG_FILEDEPENDSX = 1143,
        RPMTAG_FILEDEPENDSN = 1144,
        RPMTAG_DEPENDSDICT = 1145,
        RPMTAG_SOURCEPKGID = 1146,
        RPMTAG_FILECONTEXTS = 1147,
        RPMTAG_FSCONTEXTS = 1148,
        RPMTAG_RECONTEXTS = 1149,
        RPMTAG_POLICIES = 1150,
        RPMTAG_PRETRANS = 1151,
        RPMTAG_POSTTRANS = 1152,
        RPMTAG_PRETRANSPROG = 1153,
        RPMTAG_POSTTRANSPROG = 1154,
        RPMTAG_DISTTAG = 1155,
        RPMTAG_OLDSUGGESTSNAME = 1156,
        RPMTAG_OLDSUGGESTSVERSION = 1157,
        RPMTAG_OLDSUGGESTSFLAGS = 1158,
        RPMTAG_OLDENHANCESNAME = 1159,
        RPMTAG_OLDENHANCESVERSION = 1160,
        RPMTAG_OLDENHANCESFLAGS = 1161,
        RPMTAG_PRIORITY = 1162,
        RPMTAG_CVSID = 1163,
        RPMTAG_BLINKPKGID = 1164,
        RPMTAG_BLINKHDRID = 1165,
        RPMTAG_BLINKNEVRA = 1166,
        RPMTAG_FLINKPKGID = 1167,
        RPMTAG_FLINKHDRID = 1168,
        RPMTAG_FLINKNEVRA = 1169,
        RPMTAG_PACKAGEORIGIN = 1170,
        RPMTAG_TRIGGERPREIN = 1171,
        RPMTAG_BUILDSUGGESTS = 1172,
        RPMTAG_BUILDENHANCES = 1173,
        RPMTAG_SCRIPTSTATES = 1174,
        RPMTAG_SCRIPTMETRICS = 1175,
        RPMTAG_BUILDCPUCLOCK = 1176,
        RPMTAG_FILEDIGESTALGOS = 1177,
        RPMTAG_VARIANTS = 1178,
        RPMTAG_XMAJOR = 1179,
        RPMTAG_XMINOR = 1180,
        RPMTAG_REPOTAG = 1181,
        RPMTAG_KEYWORDS = 1182,
        RPMTAG_BUILDPLATFORMS = 1183,
        RPMTAG_PACKAGECOLOR = 1184,
        RPMTAG_PACKAGEPREFCOLOR = 1185,
        RPMTAG_XATTRSDICT = 1186,
        RPMTAG_FILEXATTRSX = 1187,
        RPMTAG_DEPATTRSDICT = 1188,
        RPMTAG_CONFLICTATTRSX = 1189,
        RPMTAG_OBSOLETEATTRSX = 1190,
        RPMTAG_PROVIDEATTRSX = 1191,
        RPMTAG_REQUIREATTRSX = 1192,
        RPMTAG_BUILDPROVIDES = 1193,
        RPMTAG_BUILDOBSOLETES = 1194,
        RPMTAG_DBINSTANCE = 1195,
        RPMTAG_NVRA = 1196,

        /* tags 1997-4999 reserved */
        RPMTAG_FILENAMES = 5000,
        RPMTAG_FILEPROVIDE = 5001,
        RPMTAG_FILEREQUIRE = 5002,
        RPMTAG_FSNAMES = 5003,
        RPMTAG_FSSIZES = 5004,
        RPMTAG_TRIGGERCONDS = 5005,
        RPMTAG_TRIGGERTYPE = 5006,
        RPMTAG_ORIGFILENAMES = 5007,
        RPMTAG_LONGFILESIZES = 5008,
        RPMTAG_LONGSIZE = 5009,
        RPMTAG_FILECAPS = 5010,
        RPMTAG_FILEDIGESTALGO = 5011,
        RPMTAG_BUGURL = 5012,
        RPMTAG_EVR = 5013,
        RPMTAG_NVR = 5014,
        RPMTAG_NEVR = 5015,
        RPMTAG_NEVRA = 5016,
        RPMTAG_HEADERCOLOR = 5017,
        RPMTAG_VERBOSE = 5018,
        RPMTAG_EPOCHNUM = 5019,
        RPMTAG_PREINFLAGS = 5020,
        RPMTAG_POSTINFLAGS = 5021,
        RPMTAG_PREUNFLAGS = 5022,
        RPMTAG_POSTUNFLAGS = 5023,
        RPMTAG_PRETRANSFLAGS = 5024,
        RPMTAG_POSTTRANSFLAGS = 5025,
        RPMTAG_VERIFYSCRIPTFLAGS = 5026,
        RPMTAG_TRIGGERSCRIPTFLAGS = 5027,
        RPMTAG_COLLECTIONS = 5029,
        RPMTAG_POLICYNAMES = 5030,
        RPMTAG_POLICYTYPES = 5031,
        RPMTAG_POLICYTYPESINDEXES = 5032,
        RPMTAG_POLICYFLAGS = 5033,
        RPMTAG_VCS = 5034,
        RPMTAG_ORDERNAME = 5035,
        RPMTAG_ORDERVERSION = 5036,
        RPMTAG_ORDERFLAGS = 5037,
        RPMTAG_MSSFMANIFEST = 5038,
        RPMTAG_MSSFDOMAIN = 5039,
        RPMTAG_INSTFILENAMES = 5040,
        RPMTAG_REQUIRENEVRS = 5041,
        RPMTAG_PROVIDENEVRS = 5042,
        RPMTAG_OBSOLETENEVRS = 5043,
        RPMTAG_CONFLICTNEVRS = 5044,
        RPMTAG_FILENLINKS = 5045,
        RPMTAG_RECOMMENDNAME = 5046,
        RPMTAG_RECOMMENDVERSION = 5047,
        RPMTAG_RECOMMENDFLAGS = 5048,
        RPMTAG_SUGGESTNAME = 5049,
        RPMTAG_SUGGESTVERSION = 5050,
        RPMTAG_SUGGESTFLAGS = 5051,
        RPMTAG_SUPPLEMENTNAME = 5052,
        RPMTAG_SUPPLEMENTVERSION = 5053,
        RPMTAG_SUPPLEMENTFLAGS = 5054,
        RPMTAG_ENHANCENAME = 5055,
        RPMTAG_ENHANCEVERSION = 5056,
        RPMTAG_ENHANCEFLAGS = 5057,
        RPMTAG_RECOMMENDNEVRS = 5058,
        RPMTAG_SUGGESTNEVRS = 5059,
        RPMTAG_SUPPLEMENTNEVRS = 5060,
        RPMTAG_ENHANCENEVRS = 5061,
        RPMTAG_ENCODING = 5062,
        RPMTAG_FILETRIGGERIN = 5063,
        RPMTAG_FILETRIGGERUN = 5064,
        RPMTAG_FILETRIGGERPOSTUN = 5065,
        RPMTAG_FILETRIGGERSCRIPTS = 5066,
        RPMTAG_FILETRIGGERSCRIPTPROG = 5067,
        RPMTAG_FILETRIGGERSCRIPTFLAGS = 5068,
        RPMTAG_FILETRIGGERNAME = 5069,
        RPMTAG_FILETRIGGERINDEX = 5070,
        RPMTAG_FILETRIGGERVERSION = 5071,
        RPMTAG_FILETRIGGERFLAGS = 5072,
        RPMTAG_TRANSFILETRIGGERIN = 5073,
        RPMTAG_TRANSFILETRIGGERUN = 5074,
        RPMTAG_TRANSFILETRIGGERPOSTUN = 5075,
        RPMTAG_TRANSFILETRIGGERSCRIPTS = 5076,
        RPMTAG_TRANSFILETRIGGERSCRIPTPROG = 5077,
        RPMTAG_TRANSFILETRIGGERSCRIPTFLAGS = 5078,
        RPMTAG_TRANSFILETRIGGERNAME = 5079,
        RPMTAG_TRANSFILETRIGGERINDEX = 5080,
        RPMTAG_TRANSFILETRIGGERVERSION = 5081,
        RPMTAG_TRANSFILETRIGGERFLAGS = 5082,
        RPMTAG_REMOVEPATHPOSTFIXES = 5083,
        RPMTAG_FILETRIGGERPRIORITIES = 5084,
        RPMTAG_TRANSFILETRIGGERPRIORITIES = 5085,
        RPMTAG_FILETRIGGERCONDS = 5086,
        RPMTAG_FILETRIGGERTYPE = 5087,
        RPMTAG_TRANSFILETRIGGERCONDS = 5088,
        RPMTAG_TRANSFILETRIGGERTYPE = 5089,
        RPMTAG_FILESIGNATURES = 5090,
        RPMTAG_FILESIGNATURELENGTH = 5091,
        RPMTAG_PAYLOADDIGEST = 5092,
        RPMTAG_PAYLOADDIGESTALGO = 5093,

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
    }
}
