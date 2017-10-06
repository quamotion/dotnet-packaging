namespace Packaging.Targets.Rpm
{
    /// <summary>
    /// Specifies how a file should be verified.
    /// </summary>
    internal enum RpmVerifyFlags
    {
        RPMVERIFY_NONE = 0,
        RPMVERIFY_MD5 = 1 << 0,
        RPMVERIFY_FILEDIGEST = 1 << 0,
        RPMVERIFY_FILESIZE = 1 << 1,
        RPMVERIFY_LINKTO = 1 << 2,
        RPMVERIFY_USER = 1 << 3,
        RPMVERIFY_GROUP = 1 << 4,
        RPMVERIFY_MTIME = 1 << 5,
        RPMVERIFY_MODE = 1 << 6,
        RPMVERIFY_RDEV = 1 << 7,
        RPMVERIFY_CAPS = 1 << 8,
        RPMVERIFY_CONTEXTS = 1 << 15,
        RPMVERIFY_READLINKFAIL = 1 << 28,
        RPMVERIFY_READFAIL = 1 << 29,
        RPMVERIFY_LSTATFAIL = 1 << 30,
        RPMVERIFY_LGETFILECONFAIL = 1 << 31,
        RPMVERIFY_ALL = ~RPMVERIFY_NONE
    }
}
