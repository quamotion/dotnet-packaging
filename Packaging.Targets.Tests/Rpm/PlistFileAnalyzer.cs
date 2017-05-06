using Packaging.Targets.IO;
using Packaging.Targets.Rpm;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

namespace Packaging.Targets.Tests.Rpm
{
    /// <summary>
    /// Because the basic <see cref="FileAnalyzer"/> does not support all the details required to fully populate the plist package,
    /// this class is overriden here and certain values are hard-coded.
    /// </summary>
    internal class PlistFileAnalyzer : FileAnalyzer
    {
        /// <inheritdoc/>
        public override Collection<string> DetermineDependencies(string filename, CpioHeader fileHeader, byte[] header)
        {
            switch (filename)
            {
                case "/usr/bin/plistutil":
                    return new Collection<string>()
                    {
                        "libpthread.so.0(GLIBC_2.2.5)(64bit)",
                        "libc.so.6(GLIBC_2.2.5)(64bit)",
                        "libplist.so.3()(64bit)",
                        "libpthread.so.0()(64bit)",
                        "libc.so.6()(64bit)",
                        "rtld(GNU_HASH)"
                    };
                case "/usr/lib64/libplist++.so.3.1.0":
                    return new Collection<string>()
                    {
                        "libplist++.so.3()(64bit)",
                        "libgcc_s.so.1(GCC_3.0)(64bit)",
                        "libpthread.so.0(GLIBC_2.2.5)(64bit)",
                        "libc.so.6(GLIBC_2.14)(64bit)",
                        "libc.so.6(GLIBC_2.2.5)(64bit)",
                        "libstdc++.so.6(CXXABI_1.3)(64bit)",
                        "libstdc++.so.6(GLIBCXX_3.4)(64bit)",
                        "libplist.so.3()(64bit)",
                        "libpthread.so.0()(64bit)",
                        "libstdc++.so.6()(64bit)",
                        "libm.so.6()(64bit)",
                        "libc.so.6()(64bit)",
                        "libgcc_s.so.1()(64bit)",
                        "rtld(GNU_HASH)"
                    };
                case "/usr/lib64/libplist.so.3.1.0":
                    return new Collection<string>()
                    {
                        "libplist.so.3()(64bit)",
                        "libpthread.so.0(GLIBC_2.2.5)(64bit)",
                        "libc.so.6(GLIBC_2.14)(64bit)",
                        "libc.so.6(GLIBC_2.2.5)(64bit)",
                        "libpthread.so.0()(64bit)",
                        "libc.so.6()(64bit)",
                        "rtld(GNU_HASH)"
                    };
            }

            return base.DetermineDependencies(filename, fileHeader, header);
        }

        /// <inheritdoc/>
        public override RpmFileColor DetermineColor(string filename, CpioHeader fileHeader, byte[] header)
        {
            switch (filename)
            {
                case "/usr/share/doc/libplist-2.0.1.151/AUTHORS":
                    return RpmFileColor.RPMFC_BLACK;
            }

            return base.DetermineColor(filename, fileHeader, header);
        }

        /// <inheritdoc/>
        public override string DetermineClass(string filename, CpioHeader fileHeader, byte[] header)
        {
            switch (filename)
            {
                case "/usr/bin/plistutil":
                    return "ELF 64-bit LSB executable, x86-64, version 1 (SYSV), dynamically linked (uses shared libs), for GNU/Linux 2.6.32, BuildID[sha1]=44864a4aec49ec94f3dc1486068ff0d308e3ae37, stripped";

                case "/usr/lib64/libplist++.so.3":
                    return string.Empty;

                case "/usr/lib64/libplist++.so.3.1.0":
                    return "ELF 64-bit LSB shared object, x86-64, version 1 (SYSV), dynamically linked, BuildID[sha1]=dae9c182875b5303acffdff2ea8bd3e4bbc8ebf0, stripped";

                case "/usr/lib64/libplist.so.3":
                    return string.Empty;

                case "/usr/lib64/libplist.so.3.1.0":
                    return "ELF 64-bit LSB shared object, x86-64, version 1 (SYSV), dynamically linked, BuildID[sha1]=cc5c79145ee69889a9fe95abaa70cf91b0d64d50, stripped";
            }

            return base.DetermineClass(filename, fileHeader, header);
        }
    }
}
