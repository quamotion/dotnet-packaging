using System.IO;

namespace Packaging.Targets.IO
{
    static class Extensions
    {
        public static string ReadAsUtf8String(this TarFile file)
        {
            using (var r = new StreamReader(file.Open()))
                return r.ReadToEnd();
        }
    }
}