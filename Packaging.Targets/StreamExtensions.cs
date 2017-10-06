using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Packaging.Targets
{
    /// <summary>
    /// Provides extension methods for the <see cref="Stream"/> class.
    /// </summary>
    internal static class StreamExtensions
    {
        /// <summary>
        /// Reads a <see cref="T"/> struct from the stream.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the struct to read.
        /// </typeparam>
        /// <param name="stream">
        /// The <see cref="Stream"/> to read the struct from.
        /// </param>
        /// <returns>
        /// A new <typeparamref name="T"/> struct, with the data read
        /// from the stream.
        /// </returns>
        public static T ReadStruct<T>(this Stream stream)
            where T : struct
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

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
            RespectEndianness<T>(data);

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

        /// <summary>
        /// Writes a struct to a stream.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the struct to write.
        /// </typeparam>
        /// <param name="stream">
        /// The stream to which to write the struct.
        /// </param>
        /// <param name="data">
        /// The struct to write to the stram.
        /// </param>
        public static void WriteStruct<T>(this Stream stream, T data)
            where T : struct
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            byte[] bytes = new byte[Marshal.SizeOf<T>()];

            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);

            try
            {
                Marshal.StructureToPtr(data, handle.AddrOfPinnedObject(), true);
            }
            finally
            {
                handle.Free();
            }

            RespectEndianness<T>(bytes);

            stream.Write(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Writes a <see cref="short"/> to a <see cref="Stream"/> using big-endian encoding.
        /// </summary>
        /// <param name="stream">
        /// The <see cref="Stream"/> to which to write the value.
        /// </param>
        /// <param name="value">
        /// The value to write to the stream.
        /// </param>
        public static void WriteBE(this Stream stream, short value)
        {
            var data = BitConverter.GetBytes(value);
            Array.Reverse(data);
            stream.Write(data, 0, data.Length);
        }

        /// <summary>
        /// Writes a <see cref="short"/> to a <see cref="Stream"/> using big-endian encoding.
        /// </summary>
        /// <param name="stream">
        /// The <see cref="Stream"/> to which to write the value.
        /// </param>
        /// <param name="value">
        /// The value to write to the stream.
        /// </param>
        public static void WriteBE(this Stream stream, int value)
        {
            var data = BitConverter.GetBytes(value);
            Array.Reverse(data);
            stream.Write(data, 0, data.Length);
        }

        /// <summary>
        /// Writes a <see cref="short"/> to a <see cref="Stream"/> using big-endian encoding.
        /// </summary>
        /// <param name="stream">
        /// The <see cref="Stream"/> to which to write the value.
        /// </param>
        /// <param name="value">
        /// The value to write to the stream.
        /// </param>
        public static void WriteBE(this Stream stream, long value)
        {
            var data = BitConverter.GetBytes(value);
            Array.Reverse(data);
            stream.Write(data, 0, data.Length);
        }

        private static void RespectEndianness<T>(byte[] data)
        {
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
        }
    }
}
