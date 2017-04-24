using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Packaging.Targets
{
    internal static class StreamExtensions
    {
        public static T ReadStruct<T>(this Stream stream) where T : struct
        {
            var size = Marshal.SizeOf<T>();

            var data = new byte[size];
            var totalRead = 0;

            while (totalRead < size)
            {
                var read = stream.Read(data, totalRead, size);

                if (read == 0)
                {
                    break;
                }

                totalRead += read;
            }

            if (totalRead < size)
            {
                throw new InvalidOperationException("Not enough data");
            }

            // Convert from network byte order (big endian) to little endian.
            foreach (var field in typeof(T).GetTypeInfo().DeclaredFields)
            {
                int length = 0;

                var type = field.FieldType;

                if (type.GetTypeInfo().IsEnum)
                {
                    type = Enum.GetUnderlyingType(type);
                }

                if (type == typeof(short) || type == typeof(ushort))
                {
                    length = 2;
                }
                else if (type == typeof(int) || type == typeof(uint))
                {
                    length = 4;
                }

                if (length > 0)
                {
                    var offset = Marshal.OffsetOf<T>(field.Name).ToInt32();
                    Array.Reverse(data, offset, length);
                }
            }

            var pinnedData = GCHandle.Alloc(data, GCHandleType.Pinned);

            try
            {
                var ptr = pinnedData.AddrOfPinnedObject();
                return Marshal.PtrToStructure<T>(ptr);
            }
            finally
            {
                pinnedData.Free();
            }
        }
    }
}
