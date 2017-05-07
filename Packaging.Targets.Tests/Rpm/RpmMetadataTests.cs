using Packaging.Targets.IO;
using Packaging.Targets.Rpm;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Xunit;

namespace Packaging.Targets.Tests.Rpm
{
    /// <summary>
    /// Tests the <see cref="RpmMetadata"/> class.
    /// </summary>
    public class RpmMetadataTests
    {
        /// <summary>
        /// Tests the various properties of the <see cref="RpmMetadata"/> class.
        /// </summary>
        [Fact]
        public void ReadMetadataPropertiesTest()
        {
            using (Stream stream = File.OpenRead(@"Rpm\libplist-2.0.1.151-1.1.x86_64.rpm"))
            {
                var package = RpmPackageReader.Read(stream);
                var metadata = new RpmMetadata(package);
                var headerRecords = package.Header.Records;

                Assert.Equal(package, metadata.Package);

                Assert.Equal("x86_64", metadata.Arch);
                Assert.Equal("lamb11", metadata.BuildHost);
                Assert.Equal(new DateTimeOffset(2017, 04, 21, 20, 56, 28, TimeSpan.Zero), metadata.BuildTime);
                Assert.Equal("lamb11 1492808188", metadata.Cookie);
                Assert.Equal("libplist is a library for manipulating Apple Binary and XML Property Lists", metadata.Description);
                Assert.Equal("home:qmfrederik / CentOS_7", metadata.Distribution);
                Assert.Equal("obs://build.opensuse.org/home:qmfrederik/CentOS_7/adfeea138cd469466e6fa13a3c88fb8f-libplist", metadata.DistUrl);
                Assert.Equal(PgpHashAlgo.PGPHASHALGO_SHA256, metadata.FileDigetsAlgo);
                Assert.Equal("System Environment/Libraries", metadata.Group);
                Assert.Equal(-976, metadata.ImmutableRegionSize);
                Assert.Equal("LGPLv2+", metadata.License);
                Assert.Equal(new Collection<string>() { "C" }, metadata.Locales);
                Assert.Equal("libplist", metadata.Name);
                Assert.Equal("-O2 -g -pipe -Wall -Wp,-D_FORTIFY_SOURCE=2 -fexceptions -fstack-protector-strong --param=ssp-buffer-size=4 -grecord-gcc-switches   -m64 -mtune=generic", metadata.OptFlags);
                Assert.Equal("linux", metadata.Os);
                Assert.Equal("xz", metadata.PayloadCompressor);
                Assert.Equal("2", metadata.PayloadFlags);
                Assert.Equal("cpio", metadata.PayloadFormat);
                Assert.Equal("x86_64-redhat-linux-gnu", metadata.Platform);
                Assert.Equal("/sbin/ldconfig", metadata.PostInProg);
                Assert.Equal("/sbin/ldconfig", metadata.PostUnProg);
                Assert.Equal("1.1", metadata.Release);
                Assert.Equal("4.11.3", metadata.RpmVersion);
                Assert.Equal(0x26e6d, metadata.Size);
                Assert.Equal(new byte[] { 0x45, 0xc0, 0x86, 0x80, 0x77, 0x4e, 0xf4, 0xc0, 0x37, 0xf1, 0x1e, 0xb1, 0xd3, 0x47, 0xf0, 0xbf }, metadata.SourcePkgId);
                Assert.Equal("libplist-2.0.1.151-1.1.src.rpm", metadata.SourceRpm);
                Assert.Equal("Library for manipulating Apple Binary and XML Property Lists", metadata.Summary);
                Assert.Equal("http://www.libimobiledevice.org/", metadata.Url);
                Assert.Equal("obs://build.opensuse.org/home:qmfrederik", metadata.Vendor);
                Assert.Equal("2.0.1.151", metadata.Version);

                var entries = metadata.ChangelogEntries.ToArray();
                Assert.Equal(0x21, entries.Length);
                Assert.Equal(new DateTimeOffset(2014, 1, 24, 12, 0, 0, TimeSpan.Zero), entries[0].Date);
                Assert.Equal("Daniel Mach <dmach@redhat.com> - 1.10-4", entries[0].Name);
                Assert.Equal("- Mass rebuild 2014-01-24", entries[0].Text);

                var files = metadata.Files.ToArray();
                Assert.Equal(9, files.Length);
                Assert.Equal("ELF 64-bit LSB executable, x86-64, version 1 (SYSV), dynamically linked (uses shared libs), for GNU/Linux 2.6.32, BuildID[sha1]=44864a4aec49ec94f3dc1486068ff0d308e3ae37, stripped", files[0].Class);
                Assert.Equal(RpmFileColor.RPMFC_ELF64, files[0].Color);
                Assert.Equal(6, files[0].Requires.Count);
                Assert.Equal("libpthread.so.0(GLIBC_2.2.5)(64bit)", files[0].Requires[0].Name);
                Assert.Equal(RpmSense.RPMSENSE_FIND_REQUIRES, files[0].Requires[0].Flags);
                Assert.Equal("", files[0].Requires[0].Version);
                Assert.Equal(1, files[0].Device);
                Assert.Equal(RpmFileFlags.None, files[0].Flags);
                Assert.Equal("root", files[0].GroupName);
                Assert.Equal(1, files[0].Inode);
                Assert.Equal("", files[0].Lang);
                Assert.Equal("", files[0].LinkTo);
                Assert.Equal(new byte[] { 0xf5, 0x17, 0x06, 0x2e, 0xe2, 0x60, 0xd9, 0x30, 0x4e, 0x74, 0xef, 0xed, 0xbb, 0x51, 0xf2, 0x53, 0x21, 0xde, 0xd8, 0x71, 0xcb, 0xb7, 0xcc, 0x68, 0x0c, 0xa2, 0x9b, 0x48, 0x0a, 0x11, 0x03, 0xbd }, files[0].MD5Hash);
                Assert.Equal(LinuxFileMode.S_IXOTH | LinuxFileMode.S_IROTH | LinuxFileMode.S_IXGRP | LinuxFileMode.S_IRGRP | LinuxFileMode.S_IXUSR | LinuxFileMode.S_IWUSR | LinuxFileMode.S_IRUSR | LinuxFileMode.S_IFREG, files[0].Mode);
                Assert.Equal(new DateTimeOffset(2017, 4, 21, 20, 56, 27, TimeSpan.Zero), files[0].ModifiedTime);
                Assert.Equal(0, files[0].Rdev);
                Assert.Equal(0x2ca0, files[0].Size);
                Assert.Equal("root", files[0].UserName);
                Assert.Equal(RpmVerifyFlags.RPMVERIFY_ALL, files[0].VerifyFlags);
                Assert.Equal("/usr/bin/plistutil", files[0].Name);

                var dependencies = metadata.Dependencies.ToArray();
                Assert.Equal(0x13, dependencies.Length);
                Assert.Equal(RpmSense.RPMSENSE_INTERP | RpmSense.RPMSENSE_SCRIPT_POST, dependencies[0].Flags);
                Assert.Equal("", dependencies[0].Version);
                Assert.Equal("/sbin/ldconfig", dependencies[0].Name);

                var provides = metadata.Provides.ToArray();
                Assert.Equal(4, provides.Length);
                Assert.Equal(RpmSense.RPMSENSE_EQUAL, provides[0].Flags);
                Assert.Equal("2.0.1.151-1.1", provides[0].Version);
                Assert.Equal("libplist", provides[0].Name);
            }
        }

        /// <summary>
        /// Tests the <see cref="RpmMetadata.Files"/> setter.
        /// </summary>
        [Fact]
        public void SetFilesTest()
        {
            using (Stream stream = File.OpenRead(@"Rpm\libplist-2.0.1.151-1.1.x86_64.rpm"))
            {
                var originalPackage = RpmPackageReader.Read(stream);
                var package = new RpmPackage();

                using (var payloadStream = RpmPayloadReader.GetDecompressedPayloadStream(originalPackage))
                using (var cpio = new CpioFile(payloadStream, false))
                {
                    RpmPackageCreator creator = new RpmPackageCreator(new PlistFileAnalyzer());
                    var files = creator.CreateFiles(cpio);

                    var metadata = new PublicRpmMetadata(package);
                    metadata.Name = "libplist";
                    metadata.Version = "2.0.1.151";
                    metadata.Arch = "x86_64";
                    metadata.Release = "1.1";

                    creator.AddPackageProvides(metadata);
                    creator.AddLdDependencies(metadata);

                    metadata.Files = files;
                    creator.AddRpmDependencies(metadata);

                    this.AssertTagEqual(IndexTag.RPMTAG_FILESIZES, originalPackage, package);
                    this.AssertTagEqual(IndexTag.RPMTAG_FILEMODES, originalPackage, package);
                    this.AssertTagEqual(IndexTag.RPMTAG_FILERDEVS, originalPackage, package);
                    this.AssertTagEqual(IndexTag.RPMTAG_FILEMTIMES, originalPackage, package);
                    this.AssertTagEqual(IndexTag.RPMTAG_FILEDIGESTS, originalPackage, package);
                    this.AssertTagEqual(IndexTag.RPMTAG_FILELINKTOS, originalPackage, package);
                    this.AssertTagEqual(IndexTag.RPMTAG_FILEFLAGS, originalPackage, package);
                    this.AssertTagEqual(IndexTag.RPMTAG_FILEUSERNAME, originalPackage, package);
                    this.AssertTagEqual(IndexTag.RPMTAG_FILEGROUPNAME, originalPackage, package);
                    this.AssertTagEqual(IndexTag.RPMTAG_FILEVERIFYFLAGS, originalPackage, package);
                    this.AssertTagEqual(IndexTag.RPMTAG_FILEDEVICES, originalPackage, package);
                    this.AssertTagEqual(IndexTag.RPMTAG_FILEINODES, originalPackage, package);
                    this.AssertTagEqual(IndexTag.RPMTAG_FILELANGS, originalPackage, package);
                    this.AssertTagEqual(IndexTag.RPMTAG_FILECOLORS, originalPackage, package);

                    this.AssertTagEqual(IndexTag.RPMTAG_FILECLASS, originalPackage, package);
                    this.AssertTagEqual(IndexTag.RPMTAG_CLASSDICT, originalPackage, package);

                    this.AssertTagEqual(IndexTag.RPMTAG_BASENAMES, originalPackage, package);
                    this.AssertTagEqual(IndexTag.RPMTAG_DIRINDEXES, originalPackage, package);
                    this.AssertTagEqual(IndexTag.RPMTAG_DIRNAMES, originalPackage, package);

                    // The require and provides records contain file dependencies, as well as package dependencies.
                    // That's why there's a call to AddLdDependencies and AddRpmDependencies, to make sure
                    // these dependencies are written out in order.
                    this.AssertTagEqual(IndexTag.RPMTAG_REQUIRENAME, originalPackage, package);
                    this.AssertTagEqual(IndexTag.RPMTAG_REQUIREFLAGS, originalPackage, package);
                    this.AssertTagEqual(IndexTag.RPMTAG_REQUIREVERSION, originalPackage, package);
                    this.AssertTagEqual(IndexTag.RPMTAG_PROVIDENAME, originalPackage, package);
                    this.AssertTagEqual(IndexTag.RPMTAG_PROVIDEFLAGS, originalPackage, package);
                    this.AssertTagEqual(IndexTag.RPMTAG_REQUIREVERSION, originalPackage, package);
                    this.AssertTagEqual(IndexTag.RPMTAG_FILEDEPENDSN, originalPackage, package);
                    this.AssertTagEqual(IndexTag.RPMTAG_FILEDEPENDSX, originalPackage, package);
                    this.AssertTagEqual(IndexTag.RPMTAG_DEPENDSDICT, originalPackage, package);
                }
            }
        }

        /// <summary>
        /// This test attempts to re-create all the metadata for the libplist package, and verifies that all data is set correctly. This test mainly
        /// excercises all the setters of the <see cref="RpmMetadata"/> object.
        /// </summary>
        [Fact]
        public void CreatePackageMetadata()
        {
            using (Stream stream = File.OpenRead(@"Rpm\libplist-2.0.1.151-1.1.x86_64.rpm"))
            {
                var originalPackage = RpmPackageReader.Read(stream);

                using (var payloadStream = RpmPayloadReader.GetDecompressedPayloadStream(originalPackage))
                using (var cpio = new CpioFile(payloadStream, false))
                {
                    RpmPackageCreator creator = new RpmPackageCreator(new PlistFileAnalyzer());
                    var files = creator.CreateFiles(cpio);

                    // Core routine to populate files and dependencies
                    RpmPackage package = new RpmPackage();
                    var metadata = new PublicRpmMetadata(package);
                    metadata.Name = "libplist";
                    metadata.Version = "2.0.1.151";
                    metadata.Arch = "x86_64";
                    metadata.Release = "1.1";

                    creator.AddPackageProvides(metadata);
                    creator.AddLdDependencies(metadata);

                    metadata.Files = files;
                    creator.AddRpmDependencies(metadata);

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

                    metadata.Size = 0x26e6d;
                    metadata.ImmutableRegionSize = -976;

                    foreach (var record in originalPackage.Header.Records)
                    {
                        AssertTagEqual(record.Key, originalPackage, package);
                    }
                }
            }
        }

        private void AssertTagEqual(IndexTag tag, RpmPackage originalPackage, RpmPackage package)
        {
            var originalRecord = originalPackage.Header.Records[tag];
            var record = package.Header.Records[tag];

            Assert.Equal(originalRecord.Value, record.Value);
            Assert.Equal(originalRecord.Header.Count, record.Header.Count);
            Assert.Equal(originalRecord.Header.Tag, record.Header.Tag);
            Assert.Equal(originalRecord.Header.Type, record.Header.Type);
        }
    }
}
