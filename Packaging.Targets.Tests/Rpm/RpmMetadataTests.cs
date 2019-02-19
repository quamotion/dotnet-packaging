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
            using (Stream stream = File.OpenRead(@"Rpm/libplist-2.0.1.151-1.1.x86_64.rpm"))
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
                Assert.Equal(string.Empty, files[0].Requires[0].Version);
                Assert.Equal(1, files[0].Device);
                Assert.Equal(RpmFileFlags.None, files[0].Flags);
                Assert.Equal("root", files[0].GroupName);
                Assert.Equal(1, files[0].Inode);
                Assert.Equal(string.Empty, files[0].Lang);
                Assert.Equal(string.Empty, files[0].LinkTo);
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
                Assert.Equal(string.Empty, dependencies[0].Version);
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
            using (Stream stream = File.OpenRead(@"Rpm/libplist-2.0.1.151-1.1.x86_64.rpm"))
            {
                var originalPackage = RpmPackageReader.Read(stream);
                var package = new RpmPackage();

                using (var payloadStream = RpmPayloadReader.GetDecompressedPayloadStream(originalPackage))
                using (var cpio = new CpioFile(payloadStream, false))
                {
                    ArchiveBuilder builder = new ArchiveBuilder(new PlistFileAnalyzer());
                    RpmPackageCreator creator = new RpmPackageCreator(new PlistFileAnalyzer());
                    var entries = builder.FromCpio(cpio);
                    var files = creator.CreateFiles(entries);

                    var metadata = new PublicRpmMetadata(package);
                    metadata.Name = "libplist";
                    metadata.Version = "2.0.1.151";
                    metadata.Arch = "x86_64";
                    metadata.Release = "1.1";

                    creator.AddPackageProvides(metadata);
                    creator.AddLdDependencies(metadata);

                    metadata.Files = files;
                    creator.AddRpmDependencies(metadata, null);

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
            using (Stream stream = File.OpenRead(@"Rpm/libplist-2.0.1.151-1.1.x86_64.rpm"))
            {
                var originalPackage = RpmPackageReader.Read(stream);

                using (var payloadStream = RpmPayloadReader.GetDecompressedPayloadStream(originalPackage))
                using (var cpio = new CpioFile(payloadStream, false))
                {
                    ArchiveBuilder builder = new ArchiveBuilder(new PlistFileAnalyzer());
                    RpmPackageCreator creator = new RpmPackageCreator(new PlistFileAnalyzer());
                    var entries = builder.FromCpio(cpio);
                    var files = creator.CreateFiles(entries);

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
                    creator.AddRpmDependencies(metadata, null);

                    PlistMetadata.ApplyDefaultMetadata(metadata);

                    metadata.Vendor = "obs://build.opensuse.org/home:qmfrederik";
                    metadata.Description = "libplist is a library for manipulating Apple Binary and XML Property Lists";
                    metadata.Url = "http://www.libimobiledevice.org/";
                    metadata.Size = 0x26e6d;
                    metadata.ImmutableRegionSize = -976;

                    foreach (var record in originalPackage.Header.Records)
                    {
                        this.AssertTagEqual(record.Key, originalPackage, package);
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
