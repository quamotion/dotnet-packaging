using Packaging.Targets.IO;
using Packaging.Targets.Rpm;
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
        public override Collection<PackageDependency> DetermineProvides(ArchiveEntry entry)
        {
            switch (entry.TargetPath)
            {
                case "/usr/bin/plistutil":
                    return new Collection<PackageDependency>()
                    {
                    };
                case "/usr/lib64/libplist++.so.3.1.0":
                    return new Collection<PackageDependency>()
                    {
                        new PackageDependency("libplist++.so.3()(64bit)", RpmSense.RPMSENSE_FIND_PROVIDES, string.Empty)
                    };
                case "/usr/lib64/libplist.so.3.1.0":
                    return new Collection<PackageDependency>()
                    {
                        new PackageDependency("libplist.so.3()(64bit)", RpmSense.RPMSENSE_FIND_PROVIDES, string.Empty)
                    };
            }

            return base.DetermineProvides(entry);
        }

        /// <inheritdoc/>
        public override Collection<PackageDependency> DetermineRequires(ArchiveEntry entry)
        {
            switch (entry.TargetPath)
            {
                case "/usr/bin/plistutil":
                    return new Collection<PackageDependency>()
                    {
                        new PackageDependency("libpthread.so.0(GLIBC_2.2.5)(64bit)", RpmSense.RPMSENSE_FIND_REQUIRES, string.Empty),
                        new PackageDependency("libc.so.6(GLIBC_2.2.5)(64bit)", RpmSense.RPMSENSE_FIND_REQUIRES, string.Empty),
                        new PackageDependency("libplist.so.3()(64bit)", RpmSense.RPMSENSE_FIND_REQUIRES, string.Empty),
                        new PackageDependency("libpthread.so.0()(64bit)", RpmSense.RPMSENSE_FIND_REQUIRES, string.Empty),
                        new PackageDependency("libc.so.6()(64bit)", RpmSense.RPMSENSE_FIND_REQUIRES, string.Empty),
                        new PackageDependency("rtld(GNU_HASH)", RpmSense.RPMSENSE_FIND_REQUIRES, string.Empty)
                    };
                case "/usr/lib64/libplist++.so.3.1.0":
                    return new Collection<PackageDependency>()
                    {
                        new PackageDependency("libgcc_s.so.1(GCC_3.0)(64bit)", RpmSense.RPMSENSE_FIND_REQUIRES, string.Empty),
                        new PackageDependency("libpthread.so.0(GLIBC_2.2.5)(64bit)", RpmSense.RPMSENSE_FIND_REQUIRES, string.Empty),
                        new PackageDependency("libc.so.6(GLIBC_2.14)(64bit)", RpmSense.RPMSENSE_FIND_REQUIRES, string.Empty),
                        new PackageDependency("libc.so.6(GLIBC_2.2.5)(64bit)", RpmSense.RPMSENSE_FIND_REQUIRES, string.Empty),
                        new PackageDependency("libstdc++.so.6(CXXABI_1.3)(64bit)", RpmSense.RPMSENSE_FIND_REQUIRES, string.Empty),
                        new PackageDependency("libstdc++.so.6(GLIBCXX_3.4)(64bit)", RpmSense.RPMSENSE_FIND_REQUIRES, string.Empty),
                        new PackageDependency("libplist.so.3()(64bit)", RpmSense.RPMSENSE_FIND_REQUIRES, string.Empty),
                        new PackageDependency("libpthread.so.0()(64bit)", RpmSense.RPMSENSE_FIND_REQUIRES, string.Empty),
                        new PackageDependency("libstdc++.so.6()(64bit)", RpmSense.RPMSENSE_FIND_REQUIRES, string.Empty),
                        new PackageDependency("libm.so.6()(64bit)", RpmSense.RPMSENSE_FIND_REQUIRES, string.Empty),
                        new PackageDependency("libc.so.6()(64bit)", RpmSense.RPMSENSE_FIND_REQUIRES, string.Empty),
                        new PackageDependency("libgcc_s.so.1()(64bit)", RpmSense.RPMSENSE_FIND_REQUIRES, string.Empty),
                        new PackageDependency("rtld(GNU_HASH)", RpmSense.RPMSENSE_FIND_REQUIRES, string.Empty)
                    };
                case "/usr/lib64/libplist.so.3.1.0":
                    return new Collection<PackageDependency>()
                    {
                        new PackageDependency("libpthread.so.0(GLIBC_2.2.5)(64bit)", RpmSense.RPMSENSE_FIND_REQUIRES, string.Empty),
                        new PackageDependency("libc.so.6(GLIBC_2.14)(64bit)", RpmSense.RPMSENSE_FIND_REQUIRES, string.Empty),
                        new PackageDependency("libc.so.6(GLIBC_2.2.5)(64bit)", RpmSense.RPMSENSE_FIND_REQUIRES, string.Empty),
                        new PackageDependency("libpthread.so.0()(64bit)", RpmSense.RPMSENSE_FIND_REQUIRES, string.Empty),
                        new PackageDependency("libc.so.6()(64bit)", RpmSense.RPMSENSE_FIND_REQUIRES, string.Empty),
                        new PackageDependency("rtld(GNU_HASH)", RpmSense.RPMSENSE_FIND_REQUIRES, string.Empty)
                    };
            }

            return base.DetermineRequires(entry);
        }

        /// <inheritdoc/>
        public override RpmFileColor DetermineColor(ArchiveEntry entry)
        {
            switch (entry.TargetPath)
            {
                case "/usr/share/doc/libplist-2.0.1.151/AUTHORS":
                    return RpmFileColor.RPMFC_BLACK;
            }

            return base.DetermineColor(entry);
        }

        /// <inheritdoc/>
        public override string DetermineClass(ArchiveEntry entry)
        {
            switch (entry.TargetPath)
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

            return base.DetermineClass(entry);
        }
    }
}
