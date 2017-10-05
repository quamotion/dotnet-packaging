using System;
using System.IO;

namespace Packaging.Targets.IO
{
    public static class ArFileCreator
    {
        public static void WriteMagic(Stream output)
        {
            var wr = new StreamWriter(output);
            wr.Write("!<arch>\n");
            wr.Flush();
        }

        public static void WriteEntry(Stream output, string name, LinuxFileMode mode, Stream data)
        {
            var hdr = new ArHeader
            {
                EndChar = "`\n",
                FileMode = mode,
                FileName = name,
                FileSize = (uint)data.Length,
                GroupId = 0,
                OwnerId = 0,
                LastModified = DateTimeOffset.UtcNow
            };
            WriteEntry(output, hdr, data);
        }
        
        public static void WriteEntry(Stream output, ArHeader header, Stream data)
        {
            output.WriteStruct(header);
            data.CopyTo(output);
            if (output.Position % 2 != 0)
                output.WriteByte(0);
        }
    }
}