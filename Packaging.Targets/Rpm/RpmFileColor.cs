using System;

namespace Packaging.Targets.Rpm
{
    /// <summary>
    /// Determines the type of the file.
    /// </summary>
    [Flags]
    internal enum RpmFileColor
    {
        RPMFC_BLACK = 0,
        RPMFC_ELF32 = 1 << 0,
        RPMFC_ELF64 = 1 << 1,
        RPMFC_ELFMIPSN32 = 1 << 2,
        RPMFC_PKGCONFIG = 1 << 4,
        RPMFC_LIBTOOL = 1 << 5,
        RPMFC_BOURNE = 1 << 6,
        RPMFC_MODULE = 1 << 7,
        RPMFC_EXECUTABLE = 1 << 8,
        RPMFC_SCRIPT = 1 << 9,
        RPMFC_TEXT = 1 << 10,
        RPMFC_DATA = 1 << 11,
        RPMFC_DOCUMENT = 1 << 12,
        RPMFC_STATIC = 1 << 13,
        RPMFC_NOTSTRIPPED = 1 << 14,
        RPMFC_COMPRESSED = 1 << 15,
        RPMFC_DIRECTORY = 1 << 16,
        RPMFC_SYMLINK = 1 << 17,
        RPMFC_DEVICE = 1 << 18,
        RPMFC_LIBRARY = 1 << 19,
        RPMFC_ARCHIVE = 1 << 20,
        RPMFC_FONT = 1 << 21,
        RPMFC_IMAGE = 1 << 22,
        RPMFC_MANPAGE = 1 << 23,
        RPMFC_PERL = 1 << 24,
        RPMFC_JAVA = 1 << 25,
        RPMFC_PYTHON = 1 << 26,
        RPMFC_PHP = 1 << 27,
        RPMFC_TCL = 1 << 28,
        RPMFC_WHITE = 1 << 29,
        RPMFC_INCLUDE = 1 << 30,
        RPMFC_ERROR = 1 << 31
    }
}
