using System.IO;
using System.Xml.Linq;

namespace Packaging.Targets.RpmRepo
{
    /// <summary>
    /// Supports reading RPM repository metadata.
    /// </summary>
    public class RepoMetadataReader
    {
        private static readonly XNamespace CommonNS = XNamespace.Get("http://linux.duke.edu/metadata/common");
        private static readonly XNamespace RepoNS = XNamespace.Get("http://linux.duke.edu/metadata/repo");
        private static readonly XNamespace RpmNS = XNamespace.Get("http://linux.duke.edu/metadata/rpm");
        private static readonly XNamespace FilelistsNS = XNamespace.Get("http://linux.duke.edu/metadata/filelists");
        private static readonly XNamespace OtherNS = XNamespace.Get("http://linux.duke.edu/metadata/other");

        /// <summary>
        /// Reads the <c>repomd.xml</c> file.
        /// </summary>
        /// <param name="stream">
        /// A  <see cref="Stream"/> which represents the repository metadata.
        /// </param>
        /// <returns>
        /// The repository metadata.
        /// </returns>
        public RepoMetadata ReadMetadata(Stream stream)
        {
            XDocument document = XDocument.Load(stream);
            return this.ReadMetadata(document);
        }

        /// <summary>
        /// Reads the <c>repomd.xml</c> file.
        /// </summary>
        /// <param name="document">
        /// A  <see cref="XDocument"/> which represents the repository metadata.
        /// </param>
        /// <returns>
        /// The repository metadata.
        /// </returns>
        public RepoMetadata ReadMetadata(XDocument document)
        {
            RepoMetadata metadata = new RepoMetadata();

            var repomd = document.Element(RepoNS + "repomd");
            metadata.Revision = (int)repomd.Element(RepoNS + "revision");

            foreach (var data in repomd.Elements(RepoNS + "data"))
            {
                var checksum = data.Element(RepoNS + "checksum");
                var openChecksum = data.Element(RepoNS + "open-checksum");
                var location = data.Element(RepoNS + "location");

                RepoData repoData = new RepoData()
                {
                    Type = (string)data.Attribute("type"),
                    Checksum = (string)checksum,
                    ChecksumType = (string)checksum.Attribute("type"),
                    OpenChecksum = (string)openChecksum,
                    OpenChecksumType = (string)checksum.Attribute("type"),
                    Location = (string)location.Attribute("href"),
                    Timestamp = (int)data.Element(RepoNS + "timestamp"),
                    Size = (int)data.Element(RepoNS + "size"),
                    OpenSize = (int)data.Element(RepoNS + "open-size")
                };

                metadata.Data.Add(repoData);
            }

            return metadata;
        }

        /// <summary>
        /// Reads the <c>primary.xml</c> file.
        /// </summary>
        /// <param name="stream">
        /// A  <see cref="Stream"/> which represents the repository metadata.
        /// </param>
        /// <returns>
        /// The repository metadata.
        /// </returns>
        public PrimaryMetadata ReadPrimary(Stream stream)
        {
            XDocument document = XDocument.Load(stream);
            return this.ReadPrimary(document);
        }

        /// <summary>
        /// Reads the <c>primary.xml</c> file.
        /// </summary>
        /// <param name="document">
        /// A  <see cref="XDocument"/> which represents the repository metadata.
        /// </param>
        /// <returns>
        /// The repository metadata.
        /// </returns>
        public PrimaryMetadata ReadPrimary(XDocument document)
        {
            PrimaryMetadata primary = new PrimaryMetadata();
            var metadata = document.Element(CommonNS + "metadata");

            foreach (var packageElement in metadata.Elements(CommonNS + "package"))
            {
                PackageMetadata package = new PackageMetadata();
                package.Type = (string)packageElement.Attribute("type");
                package.Name = (string)packageElement.Element(CommonNS + "name");
                package.Arch = (string)packageElement.Element(CommonNS + "arch");
                package.VersionEpoch = (int)packageElement.Element(CommonNS + "version").Attribute("epoch");
                package.Version = (string)packageElement.Element(CommonNS + "version").Attribute("ver");
                package.Release = (string)packageElement.Element(CommonNS + "version").Attribute("rel");
                package.ChecksumType = (string)packageElement.Element(CommonNS + "checksum").Attribute("type");
                package.ChecksumIsPkgId = (string)packageElement.Element(CommonNS + "checksum").Attribute("pkgid") == "YES";
                package.Checksum = (string)packageElement.Element(CommonNS + "checksum");
                package.Summary = (string)packageElement.Element(CommonNS + "summary");
                package.Description = (string)packageElement.Element(CommonNS + "description");
                package.Packager = (string)packageElement.Element(CommonNS + "packager");
                package.Url = (string)packageElement.Element(CommonNS + "url");
                package.FileTime = (int)packageElement.Element(CommonNS + "time").Attribute("file");
                package.BuildTime = (int)packageElement.Element(CommonNS + "time").Attribute("build");
                package.PackageSize = (int)packageElement.Element(CommonNS + "size").Attribute("package");
                package.InstalledSize = (int)packageElement.Element(CommonNS + "size").Attribute("installed");
                package.ArchiveSize = (int)packageElement.Element(CommonNS + "size").Attribute("archive");
                package.Location = (string)packageElement.Element(CommonNS + "location").Attribute("href");

                var formatElement = packageElement.Element(CommonNS + "format");

                package.License = (string)formatElement.Element(RpmNS + "license");
                package.Vendor = (string)formatElement.Element(RpmNS + "vendor");
                package.Group = (string)formatElement.Element(RpmNS + "group");
                package.BuildHost = (string)formatElement.Element(RpmNS + "buildhost");
                package.SourceRpm = (string)formatElement.Element(RpmNS + "sourcerpm");
                package.HeaderStart = (int)formatElement.Element(RpmNS + "header-range").Attribute("start");
                package.HeaderEnd = (int)formatElement.Element(RpmNS + "header-range").Attribute("end");

                var requires = formatElement.Element(RpmNS + "requires");

                if (requires != null)
                {
                    foreach (var entry in requires.Elements(RpmNS + "entry"))
                    {
                        package.Requires.Add(this.ReadEntry(entry));
                    }
                }

                var provides = formatElement.Element(RpmNS + "provides");
                if (provides != null)
                {
                    foreach (var entry in provides.Elements(RpmNS + "entry"))
                    {
                        package.Provides.Add(this.ReadEntry(entry));
                    }
                }

                foreach (var file in formatElement.Elements(CommonNS + "file"))
                {
                    package.Files.Add((string)file);
                }

                primary.Packages.Add(package);
            }

            return primary;
        }

        /// <summary>
        /// Reads the <c>filelists.xml</c> file.
        /// </summary>
        /// <param name="stream">
        /// A  <see cref="Stream"/> which represents the repository metadata.
        /// </param>
        /// <returns>
        /// The repository metadata.
        /// </returns>
        public PrimaryMetadata ReadFileLists(Stream stream)
        {
            XDocument document = XDocument.Load(stream);
            return this.ReadFileLists(document);
        }

        /// <summary>
        /// Reads the <c>filelists.xml</c> file.
        /// </summary>
        /// <param name="document">
        /// A  <see cref="XDocument"/> which represents the repository metadata.
        /// </param>
        /// <returns>
        /// The repository metadata.
        /// </returns>
        public PrimaryMetadata ReadFileLists(XDocument document)
        {
            PrimaryMetadata filelists = new PrimaryMetadata();
            var metadata = document.Element(FilelistsNS + "filelists");

            foreach (var packageElement in metadata.Elements(FilelistsNS + "package"))
            {
                PackageMetadata package = new PackageMetadata();
                package.PkgId = (string)packageElement.Attribute("pkgid");
                package.Name = (string)packageElement.Attribute("name");
                package.Arch = (string)packageElement.Attribute("arch");
                package.VersionEpoch = (int)packageElement.Element(FilelistsNS + "version").Attribute("epoch");
                package.Version = (string)packageElement.Element(FilelistsNS + "version").Attribute("ver");
                package.Release = (string)packageElement.Element(FilelistsNS + "version").Attribute("rel");

                foreach (var file in packageElement.Elements(FilelistsNS + "file"))
                {
                    package.Files.Add((string)file);
                }

                filelists.Packages.Add(package);
            }

            return filelists;
        }

        /// <summary>
        /// Reads the <c>other.xml</c> file.
        /// </summary>
        /// <param name="stream">
        /// A  <see cref="Stream"/> which represents the repository metadata.
        /// </param>
        /// <returns>
        /// The repository metadata.
        /// </returns>
        public PrimaryMetadata ReadOther(Stream stream)
        {
            XDocument document = XDocument.Load(stream);
            return this.ReadOther(document);
        }

        /// <summary>
        /// Reads the <c>other.xml</c> file.
        /// </summary>
        /// <param name="document">
        /// A  <see cref="XDocument"/> which represents the repository metadata.
        /// </param>
        /// <returns>
        /// The repository metadata.
        /// </returns>
        public PrimaryMetadata ReadOther(XDocument document)
        {
            PrimaryMetadata other = new PrimaryMetadata();
            var metadata = document.Element(OtherNS + "otherdata");

            foreach (var packageElement in metadata.Elements(OtherNS + "package"))
            {
                PackageMetadata package = new PackageMetadata();
                package.PkgId = (string)packageElement.Attribute("pkgid");
                package.Name = (string)packageElement.Attribute("name");
                package.Arch = (string)packageElement.Attribute("arch");
                package.VersionEpoch = (int)packageElement.Element(OtherNS + "version").Attribute("epoch");
                package.Version = (string)packageElement.Element(OtherNS + "version").Attribute("ver");
                package.Release = (string)packageElement.Element(OtherNS + "version").Attribute("rel");

                foreach (var changelogEntryElement in packageElement.Elements(OtherNS + "changelog"))
                {
                    var changelogEntry = new ChangeLogEntry()
                    {
                        Author = (string)changelogEntryElement.Attribute("author"),
                        Date = (int)changelogEntryElement.Attribute("date"),
                        Text = (string)changelogEntryElement
                    };

                    package.ChangeLog.Add(changelogEntry);
                }

                other.Packages.Add(package);
            }

            return other;
        }

        private RpmEntry ReadEntry(XElement element)
        {
            return new RpmEntry
            {
                Name = (string)element.Attribute("name"),
                Flags = (string)element.Attribute("flags"),
                Epoch = (int?)element.Attribute("epoch"),
                Version = (string)element.Attribute("ver"),
                Release = (string)element.Attribute("rel"),
            };
        }
    }
}
