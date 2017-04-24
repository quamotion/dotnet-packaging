using System;
using System.IO;

namespace Packaging.Targets.Rpm
{
    class RpmPackageReader
    {
        public static RpmPackage Read(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            RpmPackage package = new RpmPackage();

            package.Lead = stream.ReadStruct<RpmLead>();

            package.Signature = ReadSection(stream);
            package.Header = ReadSection(stream);

            return package;
        }

        private static Section ReadSection(Stream stream)
        {
            Section section = new Section();

            section.Header = stream.ReadStruct<RpmHeader>();

            for (int i = 0; i < section.Header.IndexCount; i++)
            {
                var record = stream.ReadStruct<IndexRecord>();
                section.Records.Add(record);
            }

            section.Data = new byte[section.Header.HeaderSize];
            stream.Read(section.Data, 0, (int)section.Header.HeaderSize);

            stream.Position += stream.Position % 8;

            return section;
        }
    }
}
