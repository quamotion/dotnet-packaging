using Packaging.Targets.IO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace Packaging.Targets.Rpm
{
    /// <summary>
    /// Provides access to the metadata stored in the RPM package.
    /// </summary>
    internal class RpmMetadata
    {
        public RpmMetadata(RpmPackage package)
        {
            if (package == null)
            {
                throw new ArgumentNullException(nameof(package));
            }

            this.Package = package;
        }

        public RpmPackage Package
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the length of the immutable region, starting from the position of the <see cref="IndexTag.RPMTAG_HEADERIMMUTABLE"/>
        /// record. This record should be the last record in the signature block, and the value is negative, indicating the previous
        /// blocks are considered immutable.
        /// </summary>
        public int ImmutableRegionSize
        {
            get
            {
                // For more information about immutable regions, see:
                // http://ftp.rpm.org/api/4.4.2.2/hregions.html
                // https://dentrassi.de/2016/04/15/writing-rpm-files-in-plain-java/
                // https://blog.bethselamin.de/posts/argh-pm.html
                // For now, we're always assuming the entire header and the entire signature is immutable
                var immutableSignatureRegion = this.GetByteArray(IndexTag.RPMTAG_HEADERIMMUTABLE);

                using (MemoryStream s = new MemoryStream(immutableSignatureRegion))
                {
                    var h = s.ReadStruct<IndexHeader>();
                    return h.Offset;
                }
            }

            set
            {
                IndexHeader header = default(IndexHeader);
                header.Offset = value;
                header.Count = 0x10;
                header.Type = IndexType.RPM_BIN_TYPE;
                header.Tag = (uint)(IndexTag.RPMTAG_HEADERIMMUTABLE);

                byte[] data;

                using (MemoryStream s = new MemoryStream())
                {
                    s.WriteStruct(header);
                    data = s.ToArray();
                }

                this.SetByteArray(IndexTag.RPMTAG_HEADERIMMUTABLE, data);
            }
        }

        /// <summary>
        /// Gets a list of all locales supported by this package. The default, invariant locale is marked as the <c>C</c> locale.
        /// </summary>
        public Collection<string> Locales
        {
            get { return this.GetStringArray(IndexTag.RPMTAG_HEADERI18NTABLE); }
            set { this.SetStringArray(IndexTag.RPMTAG_HEADERI18NTABLE, value.ToArray()); }
        }

        /// <summary>
        /// Gets the name of the package.
        /// </summary>
        public string Name
        {
            get { return this.GetString(IndexTag.RPMTAG_NAME); }
            set { this.SetString(IndexTag.RPMTAG_NAME, value); }
        }

        /// <summary>
        /// Gets the package version.
        /// </summary>
        public string Version
        {
            get { return this.GetString(IndexTag.RPMTAG_VERSION); }
            set { this.SetString(IndexTag.RPMTAG_VERSION, value); }
        }

        /// <summary>
        /// Gets the release number of the package.
        /// </summary>
        public string Release
        {
            get { return this.GetString(IndexTag.RPMTAG_RELEASE); }
            set { this.SetString(IndexTag.RPMTAG_RELEASE, value); }
        }

        /// <summary>
        /// Gets the a summary (one line) description of the package.
        /// </summary>
        public string Summary
        {
            get { return this.GetLocalizedString(IndexTag.RPMTAG_SUMMARY); }
            set { this.SetLocalizedString(IndexTag.RPMTAG_SUMMARY, value); }
        }

        /// <summary>
        /// Gets the full description of the package.
        /// </summary>
        public string Description
        {
            get { return this.GetLocalizedString(IndexTag.RPMTAG_DESCRIPTION); }
            set { this.SetLocalizedString(IndexTag.RPMTAG_DESCRIPTION, value); }
        }

        /// <summary>
        /// Gets the date and time at which the package was built.
        /// </summary>
        public DateTimeOffset BuildTime
        {
            get { return DateTimeOffset.FromUnixTimeSeconds(this.GetInt(IndexTag.RPMTAG_BUILDTIME)); }
            set { this.SetInt(IndexTag.RPMTAG_BUILDTIME, (int)value.ToUnixTimeSeconds()); }
        }

        /// <summary>
        /// Gets the name of the host on which the package was built.
        /// </summary>
        public string BuildHost
        {
            get { return this.GetString(IndexTag.RPMTAG_BUILDHOST); }
            set { this.SetString(IndexTag.RPMTAG_BUILDHOST, value); }
        }

        /// <summary>
        /// Gets the sum of the sizes of the regular files in the package.
        /// </summary>
        public int Size
        {
            get { return this.GetInt(IndexTag.RPMTAG_SIZE); }
            set { this.SetInt(IndexTag.RPMTAG_SIZE, value); }
        }

        /// <summary>
        /// Gets the name of the distribution for which the package was built.
        /// </summary>
        public string Distribution
        {
            get { return this.GetString(IndexTag.RPMTAG_DISTRIBUTION); }
            set { this.SetString(IndexTag.RPMTAG_DISTRIBUTION, value); }
        }

        /// <summary>
        /// Gets the name of the organization which produced the package.
        /// </summary>
        public string Vendor
        {
            get { return this.GetString(IndexTag.RPMTAG_VENDOR); }
            set { this.SetString(IndexTag.RPMTAG_VENDOR, value); }
        }

        /// <summary>
        /// Gets the license which applies to this package.
        /// </summary>
        public string License
        {
            get { return this.GetString(IndexTag.RPMTAG_LICENSE); }
            set { this.SetString(IndexTag.RPMTAG_LICENSE, value); }
        }

        /// <summary>
        /// Gets the administrative group to which this package belongs.
        /// </summary>
        public string Group
        {
            get { return this.GetLocalizedString(IndexTag.RPMTAG_GROUP); }
            set { this.SetLocalizedString(IndexTag.RPMTAG_GROUP, value); }
        }

        /// <summary>
        /// Gets the generic package information URL.
        /// </summary>
        public string Url
        {
            get { return this.GetString(IndexTag.RPMTAG_URL); }
            set { this.SetString(IndexTag.RPMTAG_URL, value); }
        }

        /// <summary>
        /// Gets the generic name of the OS for which this package was built. Should be <c>linux</c>.
        /// </summary>
        public string Os
        {
            get { return this.GetString(IndexTag.RPMTAG_OS); }
            set { this.SetString(IndexTag.RPMTAG_OS, value); }
        }

        /// <summary>
        /// Gets the name of the architecture for which the package was built, as defined in the architecture-specific
        /// LSB specifications.
        /// </summary>
        public string Arch
        {
            get { return this.GetString(IndexTag.RPMTAG_ARCH); }
            set { this.SetString(IndexTag.RPMTAG_ARCH, value); }
        }

        /// <summary>
        /// Gets the name of the source RPM.
        /// </summary>
        public string SourceRpm
        {
            get { return this.GetString(IndexTag.RPMTAG_SOURCERPM); }
            set { this.SetString(IndexTag.RPMTAG_SOURCERPM, value); }
        }

        /// <summary>
        /// Gets the version of the RPM tool used to build this package.
        /// </summary>
        public string RpmVersion
        {
            get { return this.GetString(IndexTag.RPMTAG_RPMVERSION); }
            set { this.SetString(IndexTag.RPMTAG_RPMVERSION, value); }
        }

        /// <summary>
        /// Gets the name of the program to run after installation of this package.
        /// </summary>
        public string PostInProg
        {
            get { return this.GetString(IndexTag.RPMTAG_POSTINPROG); }
            set { this.SetString(IndexTag.RPMTAG_POSTINPROG, value); }
        }

        /// <summary>
        /// Gets the name of the program to run after removal of this package.
        /// </summary>
        public string PostUnProg
        {
            get { return this.GetString(IndexTag.RPMTAG_POSTUNPROG); }
            set { this.SetString(IndexTag.RPMTAG_POSTUNPROG, value); }
        }

        /// <summary>
        /// Gets an opaque string whose contents are undefined.
        /// </summary>
        public string Cookie
        {
            get { return this.GetString(IndexTag.RPMTAG_COOKIE); }
            set { this.SetString(IndexTag.RPMTAG_COOKIE, value); }
        }

        /// <summary>
        /// Gets additional flags which may have been passed to the compiler when building this package.
        /// </summary>
        public string OptFlags
        {
            get { return this.GetString(IndexTag.RPMTAG_OPTFLAGS); }
            set { this.SetString(IndexTag.RPMTAG_OPTFLAGS, value); }
        }

        /// <summary>
        /// Gets the URL for the package.
        /// </summary>
        public string DistUrl
        {
            get { return this.GetString(IndexTag.RPMTAG_DISTURL); }
            set { this.SetString(IndexTag.RPMTAG_DISTURL, value); }
        }

        /// <summary>
        /// Gets the format of the payload. Should be <c>cpio.</c>
        /// </summary>
        public string PayloadFormat
        {
            get { return this.GetString(IndexTag.RPMTAG_PAYLOADFORMAT); }
            set { this.SetString(IndexTag.RPMTAG_PAYLOADFORMAT, value); }
        }

        /// <summary>
        /// Gets the name of the compressor used to compress the payload.
        /// </summary>
        public string PayloadCompressor
        {
            get { return this.GetString(IndexTag.RPMTAG_PAYLOADCOMPRESSOR); }
            set { this.SetString(IndexTag.RPMTAG_PAYLOADCOMPRESSOR, value); }
        }

        /// <summary>
        /// Gets the compression level used for the payload.
        /// </summary>
        public string PayloadFlags
        {
            get { return this.GetString(IndexTag.RPMTAG_PAYLOADFLAGS); }
            set { this.SetString(IndexTag.RPMTAG_PAYLOADFLAGS, value); }
        }

        /// <summary>
        /// Gets an opaque string whose value is undefined.
        /// </summary>
        public string Platform
        {
            get { return this.GetString(IndexTag.RPMTAG_PLATFORM); }
            set { this.SetString(IndexTag.RPMTAG_PLATFORM, value); }
        }

        public byte[] SourcePkgId
        {
            get { return this.GetByteArray(IndexTag.RPMTAG_SOURCEPKGID); }
            set { this.SetByteArray(IndexTag.RPMTAG_SOURCEPKGID, value); }
        }

        /// <summary>
        /// Gets the hashing algorithm used to calculate the hash of files embedded
        /// in this RPM archive.
        /// </summary>
        public PgpHashAlgo FileDigetsAlgo
        {
            get { return (PgpHashAlgo)this.GetInt(IndexTag.RPMTAG_FILEDIGESTALGO); }
            set { this.SetInt(IndexTag.RPMTAG_FILEDIGESTALGO, (int)value); }
        }

        /// <summary>
        /// Gets all change log entries.
        /// </summary>
        public IEnumerable<ChangelogEntry> ChangelogEntries
        {
            get
            {
                var times = this.GetIntArray(IndexTag.RPMTAG_CHANGELOGTIME);
                var names = this.GetStringArray(IndexTag.RPMTAG_CHANGELOGNAME);
                var text = this.GetStringArray(IndexTag.RPMTAG_CHANGELOGTEXT);

                int count = Math.Min(times.Count, Math.Min(names.Count, text.Count));

                for (int i = 0; i < count; i++)
                {
                    yield return new ChangelogEntry()
                    {
                        Date = DateTimeOffset.FromUnixTimeSeconds(times[i]),
                        Name = names[i],
                        Text = text[i]
                    };
                }
            }

            set
            {
                var entries = value.ToArray();

                var times = new int[entries.Length];
                var names = new string[entries.Length];
                var text = new string[entries.Length];

                for (int i = 0; i < entries.Length; i++)
                {
                    times[i] = (int)entries[i].Date.ToUnixTimeSeconds();
                    names[i] = entries[i].Name;
                    text[i] = entries[i].Text;
                }

                this.SetIntArray(IndexTag.RPMTAG_CHANGELOGTIME, times);
                this.SetStringArray(IndexTag.RPMTAG_CHANGELOGNAME, names);
                this.SetStringArray(IndexTag.RPMTAG_CHANGELOGTEXT, text);
            }
        }

        /// <summary>
        /// Gets a list of all files embedded in this package.
        /// </summary>
        public IEnumerable<RpmFile> Files
        {
            get
            {
                var sizes = this.GetIntArray(IndexTag.RPMTAG_FILESIZES);
                var modes = this.GetShortArray(IndexTag.RPMTAG_FILEMODES);
                var rdevs = this.GetShortArray(IndexTag.RPMTAG_FILERDEVS);
                var mtimes = this.GetIntArray(IndexTag.RPMTAG_FILEMTIMES);
                var md5s = this.GetStringArray(IndexTag.RPMTAG_FILEDIGESTS);
                var linkTos = this.GetStringArray(IndexTag.RPMTAG_FILELINKTOS);
                var flags = this.GetIntArray(IndexTag.RPMTAG_FILEFLAGS);
                var usernames = this.GetStringArray(IndexTag.RPMTAG_FILEUSERNAME);
                var groupnames = this.GetStringArray(IndexTag.RPMTAG_FILEGROUPNAME);
                var verifyFlags = this.GetIntArray(IndexTag.RPMTAG_FILEVERIFYFLAGS);
                var devices = this.GetIntArray(IndexTag.RPMTAG_FILEDEVICES);
                var inodes = this.GetIntArray(IndexTag.RPMTAG_FILEINODES);
                var langs = this.GetStringArray(IndexTag.RPMTAG_FILELANGS);
                var colors = this.GetIntArray(IndexTag.RPMTAG_FILECOLORS);
                var classes = this.GetIntArray(IndexTag.RPMTAG_FILECLASS);
                var dependsX = this.GetIntArray(IndexTag.RPMTAG_FILEDEPENDSX);
                var dependsN = this.GetIntArray(IndexTag.RPMTAG_FILEDEPENDSN);

                var baseNames = this.GetStringArray(IndexTag.RPMTAG_BASENAMES);
                var dirIndexes = this.GetIntArray(IndexTag.RPMTAG_DIRINDEXES);
                var dirNames = this.GetStringArray(IndexTag.RPMTAG_DIRNAMES);

                var classDict = this.GetStringArray(IndexTag.RPMTAG_CLASSDICT);
                var dependsDict = this.GetIntArray(IndexTag.RPMTAG_DEPENDSDICT);

                for (int i = 0; i < sizes.Count; i++)
                {
                    Collection<PackageDependency> requires = new Collection<PackageDependency>();
                    Collection<PackageDependency> provides = new Collection<PackageDependency>();

                    for (int j = dependsX[i]; j < dependsX[i] + dependsN[i]; j++)
                    {
                        // https://github.com/rpm-software-management/rpm/blob/8f509d669b9ae79c86dd510c5a4bc5109f60d733/build/rpmfc.c#L734
                        var value = dependsDict[j];

                        var dependencyType = GetDependencyTag((char)((value >> 24) & 0xFF));
                        var index = value & 0x00ffffff;

                        var values = this.GetStringArray(dependencyType);
                        Collection<string> versions;
                        RpmSense[] types;

                        switch (dependencyType)
                        {
                            case IndexTag.RPMTAG_REQUIRENAME:
                                versions = this.GetStringArray(IndexTag.RPMTAG_REQUIREVERSION);
                                types = this.GetIntArray(IndexTag.RPMTAG_REQUIREFLAGS).Select(e => (RpmSense)e).ToArray();
                                break;

                            case IndexTag.RPMTAG_PROVIDENAME:
                                versions = this.GetStringArray(IndexTag.RPMTAG_PROVIDEVERSION);
                                types = this.GetIntArray(IndexTag.RPMTAG_PROVIDEFLAGS).Select(e => (RpmSense)e).ToArray();
                                break;

                            default:
                                throw new ArgumentOutOfRangeException(nameof(dependencyType));
                        }

                        var dependencyName = values[index];
                        var dependencyVersion = versions[index];
                        var dependencyFlags = types[index];

                        var dependency = new PackageDependency()
                        {
                            Flags = dependencyFlags,
                            Name = dependencyName,
                            Version = dependencyVersion
                        };

                        switch (dependencyType)
                        {
                            case IndexTag.RPMTAG_REQUIRENAME:
                                requires.Add(dependency);
                                break;

                            case IndexTag.RPMTAG_PROVIDENAME:
                                provides.Add(dependency);
                                break;

                            default:
                                throw new ArgumentOutOfRangeException(nameof(dependencyType));
                        }
                    }

                    var directoryName = dirNames[dirIndexes[i]];
                    var name = $"{directoryName}{baseNames[i]}";

                    yield return new RpmFile()
                    {
                        Size = sizes[i],
                        Mode = (LinuxFileMode)modes[i],
                        Rdev = rdevs[i],
                        ModifiedTime = DateTimeOffset.FromUnixTimeSeconds(mtimes[i]),
                        MD5Hash = RpmSignature.StringToByteArray(md5s[i]),
                        LinkTo = linkTos[i],
                        Flags = (RpmFileFlags)flags[i],
                        UserName = usernames[i],
                        GroupName = groupnames[i],
                        VerifyFlags = (RpmVerifyFlags)verifyFlags[i],
                        Device = devices[i],
                        Inode = inodes[i],
                        Lang = langs[i],
                        Color = (RpmFileColor)colors[i],
                        Class = classDict[classes[i]],
                        Requires = requires,
                        Provides = provides,
                        Name = name
                    };
                }
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                var files = value.ToArray();

                var sizes = new int[files.Length];
                var modes = new short[files.Length];
                var rdevs = new short[files.Length];
                var mtimes = new int[files.Length];
                var md5s = new string[files.Length];
                var linkTos = new string[files.Length];
                var flags = new int[files.Length];
                var usernames = new string[files.Length];
                var groupnames = new string[files.Length];
                var verifyFlags = new int[files.Length];
                var devices = new int[files.Length];
                var inodes = new int[files.Length];
                var langs = new string[files.Length];
                var colors = new int[files.Length];
                var classes = new int[files.Length];

                var baseNames = new string[files.Length];
                var dirIndexes = new int[files.Length];
                var dirNames = new Collection<string>();

                var dependsX = new int[files.Length];
                var dependsN = new int[files.Length];
                var classDict = new Collection<string>();
                var dependsDict = new Collection<int>();

                var currentDependencies = this.Dependencies.ToList();
                var currentProvides = this.Provides.ToList();

                // Prepopulate the new dependencies in sorted order
                var allDependencies = files
                    .SelectMany(f => f.Requires)
                    .Distinct()
                    .Where(d => !currentDependencies.Contains(d))
                    .OrderBy(f => f.Name)
                    .ToList();

                var allProvides = files
                    .SelectMany(f => f.Provides)
                    .Distinct()
                    .Where(d => !currentProvides.Contains(d))
                    .ToList();

                currentDependencies.AddRange(allDependencies);
                this.Dependencies = currentDependencies;

                currentProvides.AddRange(allProvides);
                this.Provides = currentProvides;

                var requireNames = this.GetStringArray(IndexTag.RPMTAG_REQUIRENAME);
                var provideNames = this.GetStringArray(IndexTag.RPMTAG_PROVIDENAME);

                for (int i = 0; i < files.Length; i++)
                {
                    var file = files[i];
                    sizes[i] = file.Size;
                    modes[i] = (short)file.Mode;
                    rdevs[i] = file.Rdev;
                    mtimes[i] = (int)file.ModifiedTime.ToUnixTimeSeconds();
                    md5s[i] = BitConverter.ToString(file.MD5Hash).Replace("-", string.Empty).ToLowerInvariant();
                    linkTos[i] = file.LinkTo;
                    usernames[i] = file.UserName;
                    groupnames[i] = file.GroupName;
                    verifyFlags[i] = (int)file.VerifyFlags;
                    devices[i] = file.Device;
                    inodes[i] = file.Inode;
                    langs[i] = file.Lang;
                    colors[i] = (int)file.Color;
                    flags[i] = (int)file.Flags;

                    if (!classDict.Contains(file.Class))
                    {
                        classDict.Add(file.Class);
                    }
                    classes[i] = classDict.IndexOf(file.Class);

                    var dirName = Path.GetDirectoryName(file.Name).Replace('\\', '/') + '/';
                    var fileName = Path.GetFileName(file.Name);

                    if (!dirNames.Contains(dirName))
                    {
                        dirNames.Add(dirName);
                    }

                    dirIndexes[i] = dirNames.IndexOf(dirName);
                    baseNames[i] = fileName;

                    int dependX = dependsDict.Count;
                    dependsN[i] = file.Requires.Count + file.Provides.Count;
                    dependsX[i] = dependsN[i] == 0 ? 0 : dependX;

                    foreach (var provide in file.Provides)
                    {
                        byte type = (byte)GetDependencyType(IndexTag.RPMTAG_PROVIDENAME);

                        // Incomplete - we should check for not only name but also flags & version
                        var index = provideNames.IndexOf(provide.Name);
                        index |= (type << 24);

                        dependsDict.Add(index);
                    }

                    foreach (var dependency in file.Requires)
                    {
                        byte type = (byte)GetDependencyType(IndexTag.RPMTAG_REQUIRENAME);

                        // Incomplete - we should check for not only name but also flags & version
                        var index = requireNames.IndexOf(dependency.Name);
                        index |= (type << 24);

                        dependsDict.Add(index);
                    }
                }

                this.SetIntArray(IndexTag.RPMTAG_FILESIZES, sizes);
                this.SetShortArray(IndexTag.RPMTAG_FILEMODES, modes);
                this.SetShortArray(IndexTag.RPMTAG_FILERDEVS, rdevs);
                this.SetIntArray(IndexTag.RPMTAG_FILEMTIMES, mtimes);
                this.SetStringArray(IndexTag.RPMTAG_FILEDIGESTS, md5s);
                this.SetStringArray(IndexTag.RPMTAG_FILELINKTOS, linkTos);
                this.SetIntArray(IndexTag.RPMTAG_FILEFLAGS, flags);
                this.SetStringArray(IndexTag.RPMTAG_FILEUSERNAME, usernames);
                this.SetStringArray(IndexTag.RPMTAG_FILEGROUPNAME, groupnames);
                this.SetIntArray(IndexTag.RPMTAG_FILEVERIFYFLAGS, verifyFlags);
                this.SetIntArray(IndexTag.RPMTAG_FILEDEVICES, devices);
                this.SetIntArray(IndexTag.RPMTAG_FILEINODES, inodes);
                this.SetStringArray(IndexTag.RPMTAG_FILELANGS, langs);
                this.SetIntArray(IndexTag.RPMTAG_FILECOLORS, colors);
                this.SetIntArray(IndexTag.RPMTAG_FILECLASS, classes);
                this.SetIntArray(IndexTag.RPMTAG_FILEDEPENDSX, dependsX);
                this.SetIntArray(IndexTag.RPMTAG_FILEDEPENDSN, dependsN);

                this.SetStringArray(IndexTag.RPMTAG_BASENAMES, baseNames);
                this.SetIntArray(IndexTag.RPMTAG_DIRINDEXES, dirIndexes);
                this.SetStringArray(IndexTag.RPMTAG_DIRNAMES, dirNames.ToArray());

                this.SetStringArray(IndexTag.RPMTAG_CLASSDICT, classDict.ToArray());
                this.SetIntArray(IndexTag.RPMTAG_DEPENDSDICT, dependsDict.ToArray());

                this.Size = files.Where(f => f.Mode.HasFlag(LinuxFileMode.S_IFREG)).Sum(f => f.Size) - 0x24;
            }
        }


        public IEnumerable<PackageDependency> Dependencies
        {
            get
            {
                var flags = GetIntArray(IndexTag.RPMTAG_REQUIREFLAGS);
                var names = GetStringArray(IndexTag.RPMTAG_REQUIRENAME);
                var vers = GetStringArray(IndexTag.RPMTAG_REQUIREVERSION);

                for (int i = 0; i < flags.Count; i++)
                {
                    yield return new PackageDependency()
                    {
                        Flags = (RpmSense)flags[i],
                        Name = names[i],
                        Version = vers[i]
                    };
                }
            }

            set
            {
                var dependencies = value.ToArray();

                var flags = new int[dependencies.Length];
                var names = new string[dependencies.Length];
                var vers = new string[dependencies.Length];

                for (int i = 0; i < dependencies.Length; i++)
                {
                    flags[i] = (int)dependencies[i].Flags;
                    names[i] = dependencies[i].Name;
                    vers[i] = dependencies[i].Version;
                }

                this.SetIntArray(IndexTag.RPMTAG_REQUIREFLAGS, flags);
                this.SetStringArray(IndexTag.RPMTAG_REQUIRENAME, names);
                this.SetStringArray(IndexTag.RPMTAG_REQUIREVERSION, vers);
            }
        }

        public IEnumerable<PackageDependency> Provides
        {
            get
            {
                var flags = GetIntArray(IndexTag.RPMTAG_PROVIDEFLAGS);
                var names = GetStringArray(IndexTag.RPMTAG_PROVIDENAME);
                var vers = GetStringArray(IndexTag.RPMTAG_PROVIDEVERSION);

                for (int i = 0; i < flags.Count; i++)
                {
                    yield return new PackageDependency()
                    {
                        Flags = (RpmSense)flags[i],
                        Name = names[i],
                        Version = vers[i]
                    };
                }
            }

            set
            {
                var provides = value.ToArray();

                var flags = new int[provides.Length];
                var names = new string[provides.Length];
                var vers = new string[provides.Length];

                for (int i = 0; i < provides.Length; i++)
                {
                    flags[i] = (int)provides[i].Flags;
                    names[i] = provides[i].Name;
                    vers[i] = provides[i].Version;
                }

                this.SetIntArray(IndexTag.RPMTAG_PROVIDEFLAGS, flags);
                this.SetStringArray(IndexTag.RPMTAG_PROVIDENAME, names);
                this.SetStringArray(IndexTag.RPMTAG_PROVIDEVERSION, vers);
            }
        }

        protected char GetDependencyType(IndexTag tag)
        {
            switch (tag)
            {
                case IndexTag.RPMTAG_PROVIDENAME:
                    return 'P';
                case IndexTag.RPMTAG_REQUIRENAME:
                    return 'R';
                case IndexTag.RPMTAG_RECOMMENDNAME:
                    return 'r';
                case IndexTag.RPMTAG_SUGGESTNAME:
                    return 'S';
                case IndexTag.RPMTAG_SUPPLEMENTNAME:
                    return 'S';
                case IndexTag.RPMTAG_ENHANCENAME:
                    return 'e';
                case IndexTag.RPMTAG_CONFLICTNAME:
                    return 'C';
                case IndexTag.RPMTAG_OBSOLETENAME:
                    return 'O';
                default:
                    throw new ArgumentOutOfRangeException(nameof(tag));
            }
        }

        protected IndexTag GetDependencyTag(char deptype)
        {
            switch (deptype)
            {
                case 'P':
                    return IndexTag.RPMTAG_PROVIDENAME;
                case 'R':
                    return IndexTag.RPMTAG_REQUIRENAME;
                case 'r':
                    return IndexTag.RPMTAG_RECOMMENDNAME;
                case 's':
                    return IndexTag.RPMTAG_SUGGESTNAME;
                case 'S':
                    return IndexTag.RPMTAG_SUPPLEMENTNAME;
                case 'e':
                    return IndexTag.RPMTAG_ENHANCENAME;
                case 'C':
                    return IndexTag.RPMTAG_CONFLICTNAME;
                case 'O':
                    return IndexTag.RPMTAG_OBSOLETENAME;
                default:
                    return IndexTag.RPMTAG_NOT_FOUND;
            }
        }

        protected Collection<string> GetStringArray(IndexTag tag)
        {
            var value = this.GetValue<Collection<string>>(tag, IndexType.RPM_STRING_ARRAY_TYPE);

            if (value == null)
            {
                return new Collection<string>();
            }
            else
            {
                return value;
            }
        }

        protected void SetStringArray(IndexTag tag, string[] value)
        {
            this.SetValue(tag, IndexType.RPM_STRING_ARRAY_TYPE, value);
        }

        protected string GetLocalizedString(IndexTag tag)
        {
            var localizedValues = this.GetValue<Collection<string>>(tag, IndexType.RPM_I18NSTRING_TYPE);

            return localizedValues.First();
        }

        protected void SetLocalizedString(IndexTag tag, string value)
        {
            if (this.Locales.Count == 0)
            {
                throw new InvalidOperationException();
            }

            var values = new string[this.Locales.Count];

            for (int i = 0; i < values.Length; i++)
            {
                values[i] = value;
            }

            this.SetValue<string>(tag, IndexType.RPM_I18NSTRING_TYPE, values);
        }

        protected string GetString(IndexTag tag)
        {
            return this.GetValue<string>(tag, IndexType.RPM_STRING_TYPE);
        }

        protected void SetString(IndexTag tag, string value)
        {
            this.SetSingleValue(tag, IndexType.RPM_STRING_TYPE, value);
        }

        protected int GetInt(IndexTag tag)
        {
            return this.GetValue<int>(tag, IndexType.RPM_INT32_TYPE);
        }

        protected void SetInt(IndexTag tag, int value)
        {
            this.SetSingleValue(tag, IndexType.RPM_INT32_TYPE, value);
        }

        protected Collection<int> GetIntArray(IndexTag tag)
        {
            var value = this.GetValue<Collection<int>>(tag, IndexType.RPM_INT32_TYPE);

            if (value == null)
            {
                return new Collection<int>();
            }
            else
            {
                return value;
            }
        }

        protected void SetIntArray(IndexTag tag, int[] value)
        {
            this.SetValue(tag, IndexType.RPM_INT32_TYPE, value);
        }

        protected Collection<short> GetShortArray(IndexTag tag)
        {
            return this.GetValue<Collection<short>>(tag, IndexType.RPM_INT16_TYPE);
        }

        protected void SetShortArray(IndexTag tag, short[] value)
        {
            this.SetValue(tag, IndexType.RPM_INT16_TYPE, value);
        }

        protected byte[] GetByteArray(IndexTag tag)
        {
            return this.GetValue<byte[]>(tag, IndexType.RPM_BIN_TYPE);
        }

        protected void SetByteArray(IndexTag tag, byte[] value)
        {
            this.SetValue(tag, IndexType.RPM_BIN_TYPE, value);
        }

        protected T GetValue<T>(IndexTag tag, IndexType type)
        {
            if (!this.Package.Header.Records.ContainsKey(tag))
            {
                this.Package.Header.Records.Add(tag,
                    new IndexRecord()
                    {
                        Header = new IndexHeader()
                        {
                            Count = 0,
                            Offset = -1,
                            Tag = (uint)tag,
                            Type = type
                        },
                        Value = default(T)
                    });
            }

            var record = this.Package.Header.Records[tag];

            if (record.Header.Type != type)
            {
                throw new ArgumentOutOfRangeException(nameof(tag));
            }

            return (T)record.Value;
        }

        protected void SetValue<T>(IndexTag tag, IndexType type, T[] value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var collection = new Collection<T>(value);

            IndexRecord record = new IndexRecord()
            {
                Header = new IndexHeader()
                {
                    Count = collection.Count,
                    Offset = 0,
                    Tag = (uint)tag,
                    Type = type
                },
                Value = collection
            };

            if (!this.Package.Header.Records.ContainsKey(tag))
            {
                this.Package.Header.Records.Add(tag, record);
            }
            else
            {
                this.Package.Header.Records[tag] = record;
            }
        }

        protected void SetSingleValue<T>(IndexTag tag, IndexType type, T value)
        {
            IndexRecord record = new IndexRecord()
            {
                Header = new IndexHeader()
                {
                    Count = 1,
                    Offset = 0,
                    Tag = (uint)tag,
                    Type = type
                },
                Value = value
            };

            if (!this.Package.Header.Records.ContainsKey(tag))
            {
                this.Package.Header.Records.Add(tag, record);
            }
            else
            {
                this.Package.Header.Records[tag] = record;
            }
        }
    }
}
