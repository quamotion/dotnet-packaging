namespace Packaging.Targets.Rpm
{
    /// <summary>
    /// Determines the type of a file.
    /// </summary>
    internal enum RpmFileFlags
    {
        None = 0,
        RPMFILE_CONFIG = 1 << 0,
        RPMFILE_DOC = 1 << 1,
        RPMFILE_DONOTUSE = 1 << 2,
        RPMFILE_MISSINGOK = 1 << 3,
        RPMFILE_NOREPLACE = 1 << 4,
        RPMFILE_SPECFILE = 1 << 5,
        RPMFILE_GHOST = 1 << 6,
        RPMFILE_LICENSE = 1 << 7,
        RPMFILE_README = 1 << 8,
        RPMFILE_EXCLUDE = 1 << 9
    }
}
