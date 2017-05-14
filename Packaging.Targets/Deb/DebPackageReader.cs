using Packaging.Targets.IO;
using System;
using System.IO;
using System.IO.Compression;

namespace Packaging.Targets.Deb
{
    /// <summary>
    /// Reads <see cref="DebPackage"/> objects from a <see cref="Stream"/>.
    /// </summary>
    internal static class DebPackageReader
    {
        /// <summary>
        /// Reads a <see cref="DebPackage"/> from a <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">
        /// The <see cref="Stream"/> from which to read the package.
        /// </param>
        /// <returns>
        /// A <see cref="DebPackage"/> which represents the package.
        /// </returns>
        internal static DebPackage Read(Stream stream)
        {
            DebPackage package = new DebPackage();

            using (ArFile archive = new ArFile(stream, leaveOpen: true))
            {
                ReadDebianBinary(archive, package);
                ReadControlArchive(archive, package);
            }

            return package;
        }

        /// <summary>
        /// Reads and parses the <c>debian-binary</c> file in the Debian archive.
        /// </summary>
        /// <param name="archive">
        /// The archive to update with the data read from the <c>debian-binary</c> file.
        /// </param>
        /// <param name="package">
        /// The package to update.
        /// </param>
        private static void ReadDebianBinary(ArFile archive, DebPackage package)
        {
            if (!archive.Read())
            {
                throw new InvalidDataException();
            }

            if (archive.FileName != "debian-binary")
            {
                throw new InvalidDataException();
            }

            using (Stream stream = archive.Open())
            using (StreamReader reader = new StreamReader(stream))
            {
                var content = reader.ReadToEnd();
                package.PackageFormatVersion = new Version(content);
            }
        }

        private static void ReadControlArchive(ArFile archive, DebPackage package)
        {
            if (!archive.Read())
            {
                throw new InvalidDataException();
            }

            // gzip and xz compression are supported, but for now, we only read gz
            if (archive.FileName != "control.tar.gz")
            {
                throw new InvalidDataException();
            }

            using (Stream stream = archive.Open())
            using (GZipStream decompressedStream = new GZipStream(stream, CompressionMode.Decompress))
            {
                byte[] buffer = new byte[1024];
                decompressedStream.Read(buffer, 0, 1024);
            }
        }
    }
}
