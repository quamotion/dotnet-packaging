using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;

namespace Packaging.Targets.Rpm
{
    /// <summary>
    /// Provides methods which support writing a <see cref="RpmPackage"/> to a <see cref="Stream"/>.
    /// </summary>
    internal class RpmPackageWriter
    {
        /// <summary>
        /// Writes a <see cref="RpmPackage"/> to a <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">
        /// The <see cref="Stream"/> to which to write the <see cref="RpmPackage"/>.
        /// </param>
        /// <param name="package">
        /// The <see cref="RpmPackage"/> to write to the stream.
        /// </param>
        public static void Write(Stream stream, RpmPackage package)
        {
            if (package == null)
            {
                throw new ArgumentNullException(nameof(package));
            }

            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            stream.WriteStruct(package.Lead);

            WriteSection<SignatureTag>(stream, package.Signature, DefaultOrder.Signature);
            WriteSection<IndexTag>(stream, package.Header, DefaultOrder.Header);
        }

        public static void WriteSection<K>(Stream stream, Section<K> section, List<K> order)
        {
            while (stream.Position % 8 != 0)
            {
                stream.WriteByte(0);
            }

            stream.WriteStruct(section.Header);

            // First write out all records in the pre-defined order
            foreach (var record in order)
            {
                if (section.Records.ContainsKey(record))
                {
                    stream.WriteStruct(section.Records[record].Header);
                }
            }

            // Then write out any record which may be new.
            foreach (var record in section.Records)
            {
                if (!order.Contains(record.Key))
                {
                    stream.WriteStruct(record.Value.Header);
                }
            }

            // Write the data for all records
            long start = stream.Position;

            var records = section.Records.Values.OrderBy(v => v.Header.Offset).ToArray();
            foreach (var record in records)
            {
                long actualOffset = stream.Position - start;

                if (actualOffset > record.Header.Offset)
                {
                    throw new InvalidOperationException();
                }

                for (int i = 0; i < record.Header.Offset - actualOffset; i++)
                {
                    stream.WriteByte(0);
                }

                switch (record.Header.Type)
                {
                    case IndexType.RPM_CHAR_TYPE:
                        stream.WriteByte((byte)record.Value);
                        break;

                    case IndexType.RPM_INT8_TYPE:
                        stream.WriteByte((byte)record.Value);
                        break;

                    case IndexType.RPM_INT16_TYPE:
                        if (record.Value is short)
                        {
                            stream.WriteBE((short)record.Value);
                        }
                        else
                        {
                            var shortValues = (IEnumerable<short>)record.Value;

                            foreach (var shortValue in shortValues)
                            {
                                stream.WriteBE(shortValue);
                            }
                        }

                        break;

                    case IndexType.RPM_INT32_TYPE:
                        if (record.Value is int)
                        {
                            stream.WriteBE((int)record.Value);
                        }
                        else
                        {
                            var shortValues = (IEnumerable<int>)record.Value;

                            foreach (var shortValue in shortValues)
                            {
                                stream.WriteBE(shortValue);
                            }
                        }

                        break;

                    case IndexType.RPM_INT64_TYPE:
                        if (record.Value is long)
                        {
                            stream.WriteBE((long)record.Value);
                        }
                        else
                        {
                            var longValues = (IEnumerable<long>)record.Value;

                            foreach (var shortValue in longValues)
                            {
                                stream.WriteBE(shortValue);
                            }
                        }

                        break;

                    case IndexType.RPM_STRING_TYPE:
                        WriteNullTerminatedString(stream, (string)record.Value);
                        break;

                    case IndexType.RPM_I18NSTRING_TYPE:
                    case IndexType.RPM_STRING_ARRAY_TYPE:
                        Collection<string> strings = (Collection<string>)record.Value;

                        foreach (var s in strings)
                        {
                            WriteNullTerminatedString(stream, s);
                        }

                        break;

                    case IndexType.RPM_BIN_TYPE:
                        var value = (IEnumerable<byte>)record.Value;
                        var array = value.ToArray();
                        stream.Write(array, 0, array.Length);
                        break;

                    case IndexType.RPM_NULL_TYPE:
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private static void WriteNullTerminatedString(Stream stream, string value)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var length = Encoding.UTF8.GetByteCount(value) + 1;
            byte[] data = new byte[length];
            Encoding.UTF8.GetBytes(value, 0, value.Length, data, 0);

            stream.Write(data, 0, data.Length);
        }
    }
}
