using Packaging.Targets.Rpm;
using System;
using System.Collections.ObjectModel;

namespace Packaging.Targets.Tests.Rpm
{
    /// <summary>
    /// Provides default metadata for the Plist package. Used by multiple tests, centralized here so we don't need to type
    /// the same values every time.
    /// </summary>
    internal class PlistMetadata
    {
        public static void ApplyDefaultMetadata(RpmMetadata metadata)
        {
            // All other metadata.
            metadata.Locales = new Collection<string> { "C" }; // Should come before any localizable data.
            metadata.BuildHost = "lamb11";
            metadata.BuildTime = new DateTimeOffset(2017, 04, 21, 20, 56, 28, TimeSpan.Zero);
            metadata.Cookie = "lamb11 1492808188";
            metadata.Description = "libplist is a library for manipulating Apple Binary and XML Property Lists";
            metadata.Distribution = "home:qmfrederik / CentOS_7";
            metadata.DistUrl = "obs://build.opensuse.org/home:qmfrederik/CentOS_7/adfeea138cd469466e6fa13a3c88fb8f-libplist";
            metadata.FileDigetsAlgo = PgpHashAlgo.PGPHASHALGO_SHA256;
            metadata.Group = "System Environment/Libraries";
            metadata.License = "LGPLv2+";
            metadata.OptFlags = "-O2 -g -pipe -Wall -Wp,-D_FORTIFY_SOURCE=2 -fexceptions -fstack-protector-strong --param=ssp-buffer-size=4 -grecord-gcc-switches   -m64 -mtune=generic";
            metadata.Os = "linux";
            metadata.PayloadCompressor = "xz";
            metadata.PayloadFlags = "2";
            metadata.PayloadFormat = "cpio";
            metadata.Platform = "x86_64-redhat-linux-gnu";
            metadata.PostInProg = "/sbin/ldconfig";
            metadata.PostUnProg = "/sbin/ldconfig";
            metadata.RpmVersion = "4.11.3";
            metadata.SourcePkgId = new byte[] { 0x45, 0xc0, 0x86, 0x80, 0x77, 0x4e, 0xf4, 0xc0, 0x37, 0xf1, 0x1e, 0xb1, 0xd3, 0x47, 0xf0, 0xbf };
            metadata.SourceRpm = "libplist-2.0.1.151-1.1.src.rpm";
            metadata.Summary = "Library for manipulating Apple Binary and XML Property Lists";
            metadata.Url = "http://www.libimobiledevice.org/";
            metadata.Vendor = "obs://build.opensuse.org/home:qmfrederik";

            Collection<ChangelogEntry> changeLogEntries = new Collection<ChangelogEntry>()
                    {
                        new ChangelogEntry(DateTimeOffset.Parse("1/24/2014 12:00:00 PM +00:00"), "Daniel Mach <dmach@redhat.com> - 1.10-4", "- Mass rebuild 2014-01-24"),
                        new ChangelogEntry(DateTimeOffset.Parse("12/27/2013 12:00:00 PM +00:00"), "Daniel Mach <dmach@redhat.com> - 1.10-3", "- Mass rebuild 2013-12-27"),
                        new ChangelogEntry(DateTimeOffset.Parse("10/8/2013 12:00:00 PM +00:00"), "Matthias Clasen <mclasen@redhat.com> - 1.10-2", "- Disable strict aliasing (related: #884099)"),
                        new ChangelogEntry(DateTimeOffset.Parse("3/19/2013 12:00:00 PM +00:00"), "Peter Robinson <pbrobinson@fedoraproject.org> 1.10-1", "- New upstream 1.10 release"),
                        new ChangelogEntry(DateTimeOffset.Parse("3/18/2013 12:00:00 PM +00:00"), "Peter Robinson <pbrobinson@fedoraproject.org> 1.9-1", "- New upstream 1.9 release"),
                        new ChangelogEntry(DateTimeOffset.Parse("2/14/2013 12:00:00 PM +00:00"), "Fedora Release Engineering <rel-eng@lists.fedoraproject.org> - 1.8-6", "- Rebuilt for https://fedoraproject.org/wiki/Fedora_19_Mass_Rebuild"),
                        new ChangelogEntry(DateTimeOffset.Parse("7/19/2012 12:00:00 PM +00:00"), "Fedora Release Engineering <rel-eng@lists.fedoraproject.org> - 1.8-5", "- Rebuilt for https://fedoraproject.org/wiki/Fedora_18_Mass_Rebuild"),
                        new ChangelogEntry(DateTimeOffset.Parse("4/11/2012 12:00:00 PM +00:00"), "Peter Robinson <pbrobinson@fedoraproject.org> - 1.8-4", "- Fix python bindings"),
                        new ChangelogEntry(DateTimeOffset.Parse("4/11/2012 12:00:00 PM +00:00"), "Rex Dieter <rdieter@fedoraproject.org> 1.8-3", "- fix ftbfs, work harder to ensure CMAKE_INSTALL_LIBDIR macro is correct"),
                        new ChangelogEntry(DateTimeOffset.Parse("3/23/2012 12:00:00 PM +00:00"), "Peter Robinson <pbrobinson@fedoraproject.org> - 1.8-2", "- Fix RPATH issue with cmake, disable parallel build as it causes other problems"),
                        new ChangelogEntry(DateTimeOffset.Parse("1/12/2012 12:00:00 PM +00:00"), "Peter Robinson <pbrobinson@fedoraproject.org> - 1.8-1", "- 1.8 release"),
                        new ChangelogEntry(DateTimeOffset.Parse("9/26/2011 12:00:00 PM +00:00"), "Peter Robinson <pbrobinson@fedoraproject.org> - 1.7-1", "- 1.7 release"),
                        new ChangelogEntry(DateTimeOffset.Parse("6/25/2011 12:00:00 PM +00:00"), "Peter Robinson <pbrobinson@fedoraproject.org> 1.6-1", "- 1.6 release"),
                        new ChangelogEntry(DateTimeOffset.Parse("6/13/2011 12:00:00 PM +00:00"), "Peter Robinson <pbrobinson@fedoraproject.org> 1.5-1", "- 1.5 release"),
                        new ChangelogEntry(DateTimeOffset.Parse("3/22/2011 12:00:00 PM +00:00"), "Peter Robinson <pbrobinson@fedoraproject.org> 1.4-1", "- stable 1.4 release"),
                        new ChangelogEntry(DateTimeOffset.Parse("2/8/2011 12:00:00 PM +00:00"), "Fedora Release Engineering <rel-eng@lists.fedoraproject.org> - 1.3-3", "- Rebuilt for https://fedoraproject.org/wiki/Fedora_15_Mass_Rebuild"),
                        new ChangelogEntry(DateTimeOffset.Parse("7/21/2010 12:00:00 PM +00:00"), "David Malcolm <dmalcolm@redhat.com> - 1.3-2", "- Rebuilt for https://fedoraproject.org/wiki/Features/Python_2.7/MassRebuild"),
                        new ChangelogEntry(DateTimeOffset.Parse("4/20/2010 12:00:00 PM +00:00"), "Peter Robinson <pbrobinson@fedoraproject.org> 1.3-1", "- Upstream stable 1.3 release"),
                        new ChangelogEntry(DateTimeOffset.Parse("1/23/2010 12:00:00 PM +00:00"), "Peter Robinson <pbrobinson@fedoraproject.org> 1.2-1", "- Upstream stable 1.2 release"),
                        new ChangelogEntry(DateTimeOffset.Parse("1/9/2010 12:00:00 PM +00:00"), "Peter Robinson <pbrobinson@fedoraproject.org> 1.0.0-5", "- Updated to the new python sysarch spec file reqs"),
                        new ChangelogEntry(DateTimeOffset.Parse("12/7/2009 12:00:00 PM +00:00"), "Peter Robinson <pbrobinson@fedoraproject.org> 1.0.0-4", "- and once more with feeling"),
                        new ChangelogEntry(DateTimeOffset.Parse("12/7/2009 12:00:00 PM +00:00"), "Peter Robinson <pbrobinson@fedoraproject.org> 1.0.0-3", "- Further updated fixes for the spec file"),
                        new ChangelogEntry(DateTimeOffset.Parse("12/7/2009 12:00:00 PM +00:00"), "Peter Robinson <pbrobinson@fedoraproject.org> 1.0.0-2", "- Drop upstreamed patch"),
                        new ChangelogEntry(DateTimeOffset.Parse("12/7/2009 12:00:00 PM +00:00"), "Peter Robinson <pbrobinson@fedoraproject.org> 1.0.0-1", "- Upstream stable 1.0.0 release"),
                        new ChangelogEntry(DateTimeOffset.Parse("10/29/2009 12:00:00 PM +00:00"), "Peter Robinson <pbrobinson@fedoraproject.org> 0.16-3", "- Actually add patch for python"),
                        new ChangelogEntry(DateTimeOffset.Parse("10/29/2009 12:00:00 PM +00:00"), "Peter Robinson <pbrobinson@fedoraproject.org> 0.16-2", "- Add python patch and c++ bindings"),
                        new ChangelogEntry(DateTimeOffset.Parse("10/29/2009 12:00:00 PM +00:00"), "Peter Robinson <pbrobinson@fedoraproject.org> 0.16-1", "- New upstream 0.16 release"),
                        new ChangelogEntry(DateTimeOffset.Parse("10/20/2009 12:00:00 PM +00:00"), "Peter Robinson <pbrobinson@fedoraproject.org> 0.15-1", "- New upstream 0.15 release"),
                        new ChangelogEntry(DateTimeOffset.Parse("7/24/2009 12:00:00 PM +00:00"), "Fedora Release Engineering <rel-eng@lists.fedoraproject.org> - 0.13-2", "- Rebuilt for https://fedoraproject.org/wiki/Fedora_12_Mass_Rebuild"),
                        new ChangelogEntry(DateTimeOffset.Parse("5/11/2009 12:00:00 PM +00:00"), "Peter Robinson <pbrobinson@fedoraproject.org> 0.13-1", "- New upstream 0.13 release"),
                        new ChangelogEntry(DateTimeOffset.Parse("5/11/2009 12:00:00 PM +00:00"), "Peter Robinson <pbrobinson@fedoraproject.org> 0.12-2", "- Further review updates"),
                        new ChangelogEntry(DateTimeOffset.Parse("5/10/2009 12:00:00 PM +00:00"), "Peter Robinson <pbrobinson@fedoraproject.org> 0.12-1", "- Update to official tarball release, some review fixes"),
                        new ChangelogEntry(DateTimeOffset.Parse("5/10/2009 12:00:00 PM +00:00"), "Peter Robinson <pbrobinson@fedoraproject.org> 0.12.0-0.1", "- Initial package"),
                    };
            metadata.ChangelogEntries = changeLogEntries;
        }
    }
}
