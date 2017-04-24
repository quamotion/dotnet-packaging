using System;
using System.Collections.Generic;
using System.Text;

namespace Packaging.Targets.Rpm
{
    internal enum DependencyAttribute
    {
        RPMSENSE_LESS = 0x02,
        RPMSENSE_GREATER = 0x04,
        RPMSENSE_EQUAL = 0x08,
        RPMSENSE_PREREQ = 0x40,
        RPMSENSE_INTERP = 0x100,
        RPMSENSE_SCRIPT_PRE = 0x200,
        RPMSENSE_SCRIPT_POST = 0x400,
        RPMSENSE_SCRIPT_PREUN = 0x800,
        RPMSENSE_SCRIPT_POSTUN = 0x1000,
        RPMSENSE_RPMLIB = 0x1000000
    }
}
