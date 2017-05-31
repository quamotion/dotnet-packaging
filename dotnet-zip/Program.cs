using System.Diagnostics;

namespace Dotnet.Packaging
{
    class Program
    {
        static int Main(string[] args)
        {
            PackagingRunner runner = new PackagingRunner("zip archive", "CreateZip");
            return runner.Run(args);
        }
    }
}
