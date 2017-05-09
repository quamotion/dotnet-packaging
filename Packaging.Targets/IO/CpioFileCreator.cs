using Packaging.Targets.Rpm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Packaging.Targets.IO
{
    public class CpioFileCreator
    {
        private IFileAnalyzer fileAnayzer;

        public CpioFileCreator()
        {
            fileAnayzer = new FileAnalyzer();
        }

        public void FromDirectory(string directory, string prefix, Stream targetStream)
        {
            using (CpioFile cpioFile = new CpioFile(targetStream, leaveOpen: true))
            {
                uint inode = 0;

                AddDirectory(directory, prefix, cpioFile, ref inode);
                cpioFile.WriteTrailer();
            }
        }

        public void AddDirectory(string directory, string prefix, CpioFile cpioFile, ref uint inode)
        {
            byte[] fileHeader = new byte[1024];

            foreach (var file in Directory.GetFiles(directory))
            {

                using (Stream fileStream = File.OpenRead(file))
                {
                    var fileName = Path.GetFileName(file);

                    if (fileName.StartsWith(".") || fileStream.Length == 0)
                    {
                        // Skip hidden and empty files - this would case rmplint errors.
                        continue;
                    }

                    inode++;

                    var read = fileStream.Read(fileHeader, 0, fileHeader.Length);

                    bool isExecutable = this.fileAnayzer.IsExecutable(fileHeader);

                    var mode = LinuxFileMode.S_IROTH | LinuxFileMode.S_IRGRP | LinuxFileMode.S_IRUSR | LinuxFileMode.S_IFREG;

                    if (isExecutable)
                    {
                        mode |= LinuxFileMode.S_IXOTH | LinuxFileMode.S_IXGRP | LinuxFileMode.S_IWUSR | LinuxFileMode.S_IXUSR;
                    }

                    fileStream.Position = 0;
                    string name = prefix + "/" + fileName;

                    CpioHeader cpioHeader = new CpioHeader()
                    {
                        Check = 0,
                        DevMajor = 1,
                        DevMinor = 0,
                        FileSize = (uint)fileStream.Length,
                        Gid = 0, // root
                        Uid = 0, // root
                        Ino = inode,
                        Mode = mode,
                        Mtime = File.GetLastWriteTimeUtc(file),
                        NameSize = (uint)name.Length + 1,
                        Nlink = 1,
                        RDevMajor = 0,
                        RDevMinor = 0,
                        Signature = "070701",
                    };

                    cpioFile.Write(cpioHeader, name, fileStream);
                }
            }

            foreach (var child in Directory.GetDirectories(directory))
            {
                AddDirectory(child, prefix + "/" + Path.GetFileName(child), cpioFile, ref inode);
            }
        }
    }
}
