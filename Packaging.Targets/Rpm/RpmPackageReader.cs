using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Packaging.Targets.Rpm
{
    internal class RpmPackageReader
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

            package.Signature = ReadSection<SignatureTag>(stream, h => (SignatureTag)h.Tag);

            if (stream.Position % 8 != 0)
            {
                stream.Position += 8 - (stream.Position % 8);
            }

            package.HeaderOffset = stream.Position;
            package.Header = ReadSection<IndexTag>(stream, h => (IndexTag)h.Tag);

            package.PayloadOffset = stream.Position;

            return package;
        }

        public static Section<K> ReadSection<K>(Stream stream, Func<IndexHeader, K> getTag)
        {
            Section<K> section = new Section<K>();

            section.Header = stream.ReadStruct<RpmHeader>();

            for (int i = 0; i < section.Header.IndexCount; i++)
            {
                var header = stream.ReadStruct<IndexHeader>();
                section.Records.Add(
                    getTag(header),
                    new IndexRecord()
                    {
                        Header = header
                    });
            }

            var data = new byte[section.Header.HeaderSize];
            stream.Read(data, 0, (int)section.Header.HeaderSize);

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
                        record.Value = (char)data[offset];
                        break;

                    case IndexType.RPM_INT8_TYPE:
                        record.Value = (byte)data[offset];
                        break;

                    case IndexType.RPM_INT16_TYPE:
                        Collection<short> shortValues = new Collection<short>();

                        for (int i = 0; i < record.Header.Count; i++)
                        {
                            Array.Copy(data, offset + i * sizeof(short), int16Buffer, 0, sizeof(short));
                            Array.Reverse(int16Buffer);
                            shortValues.Add(BitConverter.ToInt16(int16Buffer, 0));
                        }

                        if (shortValues.Count == 1)
                        {
                            record.Value = shortValues[0];
                        }
                        else
                        {
                            record.Value = shortValues;
                        }

                        break;

                    case IndexType.RPM_INT32_TYPE:
                        Collection<int> intValues = new Collection<int>();

                        for (int i = 0; i < record.Header.Count; i++)
                        {
                            Array.Copy(data, offset + i * sizeof(int), int32Buffer, 0, sizeof(int));
                            Array.Reverse(int32Buffer);
                            intValues.Add(BitConverter.ToInt32(int32Buffer, 0));
                        }

                        if (intValues.Count == 1)
                        {
                            record.Value = intValues[0];
                        }
                        else
                        {
                            record.Value = intValues;
                        }

                        break;

                    case IndexType.RPM_INT64_TYPE:
                        Collection<long> longValues = new Collection<long>();

                        for (int i = 0; i < record.Header.Count; i++)
                        {
                            Array.Copy(data, offset + i * sizeof(long), int64Buffer, 0, sizeof(long));
                            Array.Reverse(int64Buffer);
                            longValues.Add(BitConverter.ToInt64(int64Buffer, 0));
                        }

                        if (longValues.Count == 1)
                        {
                            record.Value = longValues[0];
                        }
                        else
                        {
                            record.Value = longValues;
                        }

                        break;

                    case IndexType.RPM_STRING_TYPE:
                        record.Value = ReadNullTerminatedString(ref offset, data);
                        break;

                    case IndexType.RPM_I18NSTRING_TYPE:
                    case IndexType.RPM_STRING_ARRAY_TYPE:
                        Collection<string> strings = new Collection<string>();

                        for (int i = 0; i < record.Header.Count; i++)
                        {
                            strings.Add(ReadNullTerminatedString(ref offset, data));
                        }

                        record.Value = strings;
                        break;

                    case IndexType.RPM_BIN_TYPE:
                        byte[] value = new byte[record.Header.Count];
                        Array.Copy(data, offset, value, 0, (int)record.Header.Count);
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
            offset += 1;
            return value;
        }
    }
}
