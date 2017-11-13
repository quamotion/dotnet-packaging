using System.IO;
using System.Text;

namespace Packaging.Targets.IO
{
    internal static class Extensions
    {
        public static string ReadAsUtf8String(this TarFile file)
        {
            // Use Encoding.UTF8 on a byte array instead of a StreamReader to make sure
            // we stop reading when we encounter a null (\0) character.
            using (var stream = file.Open())
            {
                byte[] data = new byte[stream.Length];
                stream.Read(data, 0, data.Length);

                return Encoding.UTF8.GetString(data);
            }
        }
    }
}