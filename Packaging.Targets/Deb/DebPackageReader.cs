using Packaging.Targets.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Formats.Tar;
using System.IO;
using System.IO.Compression;
using System.Text;

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
                while (archive.Read())
                {
                    if (archive.FileName == "debian-binary")
                    {
                        ReadDebianBinary(archive, package);
                    }
                    else if (archive.FileName == "control.tar.gz")
                    {
                        ReadControlArchive(archive, package);
                    }
                }
            }

            return package;
        }

        internal static Stream GetPayloadStream(Stream stream)
        {
            using (ArFile archive = new ArFile(stream, leaveOpen: true))
            {
                while (archive.Read())
                {
                    if (archive.FileName.StartsWith("data.tar."))
                    {
                        var ext = Path.GetExtension(archive.FileName);
                        if (ext == ".gz")
                        {
                            return new GZipStream(archive.Open(), CompressionMode.Decompress, leaveOpen: false);
                        }

                        if (ext == ".xz")
                        {
                            // For some reason it complains about corrupted data when we try to read using smaller chunks
                            var payload = new MemoryStream();
                            using (var xz = new XZInputStream(archive.Open()))
                            {
                                xz.CopyTo(payload);
                                payload.Seek(0, SeekOrigin.Begin);
                                return payload;
                            }
                        }

                        throw new InvalidDataException("Don't know how to decompress " + archive.FileName);
                    }
                }

                throw new InvalidDataException("data.tar.?? not found");
            }
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
            using (Stream stream = archive.Open())
            using (StreamReader reader = new StreamReader(stream))
            {
                var content = reader.ReadToEnd();
                package.PackageFormatVersion = new Version(content);
            }
        }

        private static void ReadControlArchive(ArFile archive, DebPackage package)
        {
            package.ControlExtras = new Dictionary<string, DebPackageControlFileData>();
            package.Md5Sums = new Dictionary<string, string>();
            using (Stream stream = archive.Open())
            using (GZipStream decompressedStream = new GZipStream(stream, CompressionMode.Decompress, leaveOpen: true))
            using (TarReader tarReader = new TarReader(decompressedStream, leaveOpen: true))
            {
                TarEntry entry;
                while ((entry = tarReader.GetNextEntry()) != null)
                {
                    switch (entry.Name)
                    {
                        case "./control":
                            package.ControlFile = ControlFileParser.Read(entry.DataStream);
                            break;
                        case "./md5sums":
                            using (var sums = new StreamReader(entry.DataStream))
                            {
                                string line;
                                while ((line = sums.ReadLine()) != null)
                                {
                                    var s = line.Split(new[] { "  " }, 2, StringSplitOptions.None);
                                    package.Md5Sums[s[1]] = s[0];
                                }
                            }

                            break;
                        case "./preinst":
                            package.PreInstallScript = ReadAsUtf8String(entry);
                            break;
                        case "./postinst":
                            package.PostInstallScript = ReadAsUtf8String(entry);
                            break;
                        case "./prerm":
                            package.PreRemoveScript = ReadAsUtf8String(entry);
                            break;
                        case "./postrm":
                            package.PostRemoveScript = ReadAsUtf8String(entry);
                            break;

                        case "./":
                            break;
                        default:
                            package.ControlExtras[entry.Name] = new DebPackageControlFileData
                            {
                                Mode = (LinuxFileMode)(int)entry.Mode,
                                Contents = ReadAsUtf8String(entry)
                            };
                            break;
                    }
                }
            }
        }

        private static string ReadAsUtf8String(TarEntry entry)
        {
            if (entry.DataStream == null)
            {
                return string.Empty;
            }

            using (var data = new MemoryStream())
            {
                entry.DataStream.CopyTo(data);
                return Encoding.UTF8.GetString(data.GetBuffer(), 0, (int)data.Length);
            }
        }
    }
}
