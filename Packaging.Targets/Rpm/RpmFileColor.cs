using System;

namespace Packaging.Targets.Rpm
{
    /*
     * from https://github.com/rpm-software-management/rpm/blob/master/build/rpmfc.h
     */

    /// <summary>
    /// Determines the type of the file.
    /// </summary>
    [Flags]
    internal enum RpmFileColor : int
    {
        RPMFC_BLACK = 0,
        RPMFC_ELF32 = 1 << 0,
        RPMFC_ELF64 = 1 << 1,
        RPMFC_ELFMIPSN32 = 1 << 2,
        RPMFC_ELF = RPMFC_ELF32 | RPMFC_ELF64 | RPMFC_ELFMIPSN32,
        RPMFC_WHITE = 1 << 29,
        RPMFC_INCLUDE = 1 << 30,
        RPMFC_ERROR = 1 << 31
    }
}
