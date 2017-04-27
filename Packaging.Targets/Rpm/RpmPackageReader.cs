using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

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

            if (!stream.CanSeek)
            {
                throw new ArgumentOutOfRangeException(nameof(stream), "A stream which backs a RPM package must be seekable");
            }

            RpmPackage package = new RpmPackage();
            package.Stream = stream;

            package.Lead = stream.ReadStruct<RpmLead>();

            package.Signature = ReadSection(stream);
            package.Header = ReadSection(stream);

            package.PayloadOffset = stream.Position;

            return package;
        }

        private static Section ReadSection(Stream stream)
        {
            Section section = new Section();

            stream.Position += stream.Position % 8;

            section.Header = stream.ReadStruct<RpmHeader>();

            for (int i = 0; i < section.Header.IndexCount; i++)
            {
                var header = stream.ReadStruct<IndexHeader>();
                section.Records.Add(
                    header.Tag,
                    new IndexRecord()
                    {
                        Header = header
                    });
            }

            section.Data = new byte[section.Header.HeaderSize];
            stream.Read(section.Data, 0, (int)section.Header.HeaderSize);

            var headerSize = Marshal.SizeOf<IndexHeader>() * section.Records.Count;

            byte[] int16Buffer = new byte[2];
            byte[] int32Buffer = new byte[4];
            byte[] int64Buffer = new byte[8];

            // Read the data for all records
            foreach (var record in section.Records.Values)
            {
                var offset = (int)record.Header.Offset;

                switch (record.Header.Type)
                {
                    case IndexType.RPM_CHAR_TYPE:
                        record.Value = (char)section.Data[offset];
                        break;

                    case IndexType.RPM_INT8_TYPE:
                        record.Value = (byte)section.Data[offset];
                        break;

                    case IndexType.RPM_INT16_TYPE:
                        Array.Copy(section.Data, offset, int16Buffer, 0, 2);
                        Array.Reverse(int16Buffer);
                        record.Value = BitConverter.ToInt16(int16Buffer, 0);
                        break;

                    case IndexType.RPM_INT32_TYPE:
                        Array.Copy(section.Data, offset, int32Buffer, 0, 4);
                        Array.Reverse(int32Buffer);
                        record.Value = BitConverter.ToInt16(int32Buffer, 0);
                        break;

                    case IndexType.RPM_INT64_TYPE:
                        Array.Copy(section.Data, offset, int64Buffer, 0, 8);
                        Array.Reverse(int64Buffer);
                        record.Value = BitConverter.ToInt16(int64Buffer, 0);
                        break;

                    case IndexType.RPM_STRING_TYPE:
                        record.Value = ReadNullTerminatedString(ref offset, section.Data);
                        break;

                    case IndexType.RPM_I18NSTRING_TYPE:
                    case IndexType.RPM_STRING_ARRAY_TYPE:
                        Collection<string> strings = new Collection<string>();

                        for (int i = 0; i < record.Header.Count; i++)
                        {
                            strings.Add(ReadNullTerminatedString(ref offset, section.Data));
                        }

                        record.Value = strings;
                        break;

                    case IndexType.RPM_BIN_TYPE:
                        byte[] value = new byte[record.Header.Count];
                        Array.Copy(section.Data, offset, value, 0, (int)record.Header.Count);
                        record.Value = value;
                        break;

                    case IndexType.RPM_NULL_TYPE:
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return section;
        }

        private static string ReadNullTerminatedString(ref int offset, byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            int stringLength = 0;

            while (offset + stringLength < data.Length && data[offset + stringLength] != 0)
            {
                stringLength++;
            }

            string value;

            if (stringLength > 0)
            {
                value = Encoding.UTF8.GetString(data, offset, stringLength);
            }
            else
            {
                value = string.Empty;
            }

            offset += stringLength;
            return value;
        }
    }
}
