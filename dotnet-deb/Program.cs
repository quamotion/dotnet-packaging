using System.Diagnostics;

namespace Dotnet.Packaging
{
    class Program
    {
        static int Main(string[] args)
        {
            PackagingRunner runner = new PackagingRunner("Debian/Ubuntu installer package", "CreateDeb", "deb");
            return runner.Run(args);
        }
    }
}
