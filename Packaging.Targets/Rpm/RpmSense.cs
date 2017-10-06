using System;
using System.Collections.Generic;
using System.Text;

namespace Packaging.Targets.Rpm
{
    [Flags]
    internal enum RpmSense
    {
        RPMSENSE_ANY = 0,
        RPMSENSE_LESS = 1 << 1,
        RPMSENSE_GREATER = 1 << 2,
        RPMSENSE_EQUAL = 1 << 3,
        RPMSENSE_POSTTRANS = 1 << 5, /*!< %posttrans dependency */
        RPMSENSE_PREREQ = 1 << 6,     /* legacy prereq dependency */
        RPMSENSE_PRETRANS = 1 << 7, /*!< Pre-transaction dependency. */
        RPMSENSE_INTERP = 1 << 8, /*!< Interpreter used by scriptlet. */
        RPMSENSE_SCRIPT_PRE = 1 << 9, /*!< %pre dependency. */
        RPMSENSE_SCRIPT_POST = 1 << 10,   /*!< %post dependency. */
        RPMSENSE_SCRIPT_PREUN = 1 << 11,  /*!< %preun dependency. */
        RPMSENSE_SCRIPT_POSTUN = 1 << 12, /*!< %postun dependency. */
        RPMSENSE_SCRIPT_VERIFY = 1 << 13, /*!< %verify dependency. */
        RPMSENSE_FIND_REQUIRES = 1 << 14, /*!< find-requires generated dependency. */
        RPMSENSE_FIND_PROVIDES = 1 << 15, /*!< find-provides generated dependency. */

        RPMSENSE_TRIGGERIN = 1 << 16,    /*!< %triggerin dependency. */
        RPMSENSE_TRIGGERUN = 1 << 17,    /*!< %triggerun dependency. */
        RPMSENSE_TRIGGERPOSTUN = 1 << 18, /*!< %triggerpostun dependency. */
        RPMSENSE_MISSINGOK = 1 << 19,    /*!< suggests/enhances hint. */
        RPMSENSE_RPMLIB = 1 << 24,    /*!< rpmlib(feature) dependency. */
        RPMSENSE_TRIGGERPREIN = 1 << 25,  /*!< %triggerprein dependency. */
        RPMSENSE_KEYRING = 1 << 26,
        RPMSENSE_CONFIG = 1 << 28
    }
}
