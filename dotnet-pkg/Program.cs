using System.Diagnostics;

namespace Dotnet.Packaging
{
    class Program
    {
        static int Main(string[] args)
        {
            PackagingRunner runner = new PackagingRunner("macOS installer package", "CreatePkg");
            return runner.Run(args);
        }
    }
}
