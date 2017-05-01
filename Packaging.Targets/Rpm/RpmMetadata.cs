using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;

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

                var immutableSignatureRegion = (byte[])this.Package.Header.Records[IndexTag.RPMTAG_HEADERIMMUTABLE].Value;

                using (MemoryStream s = new MemoryStream(immutableSignatureRegion))
                {
                    var h = s.ReadStruct<IndexHeader>();
                    return h.Offset;
                }
            }
        }

        /// <summary>
        /// Gets a list of all locales supported by this package. The default, invariant locale is marked as the <c>C</c> locale.
        /// </summary>
        public Collection<string> Locales
        {
            get
            {
                return this.GetStringArray(IndexTag.RPMTAG_HEADERI18NTABLE);
            }
        }

        /// <summary>
        /// Gets the name of the package.
        /// </summary>
        public string Name
        {
            get { return this.GetString(IndexTag.RPMTAG_NAME); }
        }

        /// <summary>
        /// Gets the package version.
        /// </summary>
        public string Version
        {
            get { return this.GetString(IndexTag.RPMTAG_VERSION); }
        }

        /// <summary>
        /// Gets the release number of the package.
        /// </summary>
        public string Release
        {
            get { return this.GetString(IndexTag.RPMTAG_RELEASE); }
        }

        /// <summary>
        /// Gets the a summary (one line) description of the package.
        /// </summary>
        public string Summary
        {
            get { return this.GetLocalizedString(IndexTag.RPMTAG_SUMMARY); }
        }

        /// <summary>
        /// Gets the full description of the package.
        /// </summary>
        public string Description
        {
            get { return this.GetLocalizedString(IndexTag.RPMTAG_DESCRIPTION); }
        }

        /// <summary>
        /// Gets the date and time at which the package was built.
        /// </summary>
        public int BuildTime
        {
            get { return this.GetInt(IndexTag.RPMTAG_BUILDTIME); }
        }

        /// <summary>
        /// Gets the name of the host on which the package was built.
        /// </summary>
        public string BuildHost
        {
            get { return this.GetString(IndexTag.RPMTAG_BUILDHOST); }
        }

        /// <summary>
        /// Gets the sum of the sizes of the regular files in the package.
        /// </summary>
        public int Size
        {
            get { return this.GetInt(IndexTag.RPMTAG_SIZE); }
        }

        /// <summary>
        /// Gets the name of the distribution for which the package was built.
        /// </summary>
        public string Distribution
        {
            get { return this.GetString(IndexTag.RPMTAG_DISTRIBUTION); }
        }

        /// <summary>
        /// Gets the name of the organization which produced the package.
        /// </summary>
        public string Vendor
        {
            get { return this.GetString(IndexTag.RPMTAG_VENDOR); }
        }

        /// <summary>
        /// Gets the license which applies to this package.
        /// </summary>
        public string License
        {
            get { return this.GetString(IndexTag.RPMTAG_LICENSE); }
        }

        /// <summary>
        /// Gets the administrative group to which this package belongs.
        /// </summary>
        public string Group
        {
            get { return this.GetLocalizedString(IndexTag.RPMTAG_GROUP); }
        }

        /// <summary>
        /// Gets the generic package information URL.
        /// </summary>
        public string Url
        {
            get { return this.GetString(IndexTag.RPMTAG_URL); }
        }

        /// <summary>
        /// Gets the generic name of the OS for which this package was built. Should be <c>linux</c>.
        /// </summary>
        public string Os
        {
            get { return this.GetString(IndexTag.RPMTAG_OS); }
        }

        /// <summary>
        /// Gets the name of the architecture for which the package was built, as defined in the architecture-specific
        /// LSB specifications.
        /// </summary>
        public string Arch
        {
            get { return this.GetString(IndexTag.RPMTAG_ARCH); }
        }

        /// <summary>
        /// Gets the name of the source RPM.
        /// </summary>
        public string SourceRpm
        {
            get { return this.GetString(IndexTag.RPMTAG_SOURCERPM); }
        }

        /// <summary>
        /// Gets the version of the RPM tool used to build this package.
        /// </summary>
        public string RpmVersion
        {
            get { return this.GetString(IndexTag.RPMTAG_RPMVERSION); }
        }

        /// <summary>
        /// Gets the name of the program to run after installation of this package.
        /// </summary>
        public string PostInProg
        {
            get { return this.GetString(IndexTag.RPMTAG_POSTINPROG); }
        }

        /// <summary>
        /// Gets the name of the program to run after removal of this package.
        /// </summary>
        public string PostUnProg
        {
            get { return this.GetString(IndexTag.RPMTAG_POSTUNPROG); }
        }

        /// <summary>
        /// Gets an opaque string whose contents are undefined.
        /// </summary>
        public string Cookie
        {
            get { return this.GetString(IndexTag.RPMTAG_COOKIE); }
        }

        /// <summary>
        /// Gets additional flags which may have been passed to the compiler when building this package.
        /// </summary>
        public string OptFlags
        {
            get { return this.GetString(IndexTag.RPMTAG_OPTFLAGS); }
        }

        /// <summary>
        /// Gets the URL for the package.
        /// </summary>
        public string DistUrl
        {
            get { return this.GetString(IndexTag.RPMTAG_DISTURL); }
        }

        /// <summary>
        /// Gets the format of the payload. Should be <c>cpio.</c>
        /// </summary>
        public string PayloadFormat
        {
            get { return this.GetString(IndexTag.RPMTAG_PAYLOADFORMAT); }
        }

        /// <summary>
        /// Gets the name of the compressor used to compress the payload.
        /// </summary>
        public string PayloadCompressor
        {
            get { return this.GetString(IndexTag.RPMTAG_PAYLOADCOMPRESSOR); }
        }

        /// <summary>
        /// Gets the compression level used for the payload.
        /// </summary>
        public string PayloadFlags
        {
            get { return this.GetString(IndexTag.RPMTAG_PAYLOADFLAGS); }
        }

        /// <summary>
        /// Gets an opaque string whose value is undefined.
        /// </summary>
        public string Platform
        {
            get { return this.GetString(IndexTag.RPMTAG_PLATFORM); }
        }

        public byte[] SourcePkgId
        {
            get { return this.GetByteArray(IndexTag.RPMTAG_SOURCEPKGID); }
        }

        public int FileDigetsAlgo
        {
            get { return this.GetInt(IndexTag.RPMTAG_FILEDIGESTALGO); }
        }

        // Changelog
        // Require_*: Dependencies
        // Provide_*: Dependencies
        // Dirs
        // Files

        protected Collection<string> GetStringArray(IndexTag tag)
        {
            return this.GetValue<Collection<string>>(tag, IndexType.RPM_STRING_ARRAY_TYPE, false);
        }

        protected string GetLocalizedString(IndexTag tag)
        {
            var localizedValues = this.GetValue<Collection<string>>(tag, IndexType.RPM_I18NSTRING_TYPE, false);

            return localizedValues.First();
        }

        protected string GetString(IndexTag tag)
        {
            return this.GetValue<string>(tag, IndexType.RPM_STRING_TYPE, false);
        }

        protected int GetInt(IndexTag tag)
        {
            return this.GetValue<int>(tag, IndexType.RPM_INT32_TYPE, false);
        }

        protected byte[] GetByteArray(IndexTag tag)
        {
            return this.GetValue<byte[]>(tag, IndexType.RPM_BIN_TYPE, true);
        }

        protected T GetValue<T>(IndexTag tag, IndexType type, bool plural)
        {
            if (!this.Package.Header.Records.ContainsKey(tag))
            {
                throw new ArgumentOutOfRangeException(nameof(tag));
            }

            var record = this.Package.Header.Records[tag];

            if (record.Header.Count > 1 != plural)
            {
                throw new ArgumentOutOfRangeException(nameof(tag));
            }

            if (record.Header.Type != type)
            {
                throw new ArgumentOutOfRangeException(nameof(tag));
            }

            return (T)record.Value;
        }
    }
}
