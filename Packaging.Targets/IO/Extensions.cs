using System.IO;

namespace Packaging.Targets.IO
{
    internal static class Extensions
    {
        public static string ReadAsUtf8String(this TarFile file)
        {
            using (var r = new StreamReader(file.Open()))
            {
                return r.ReadToEnd();
            }
        }
    }
}