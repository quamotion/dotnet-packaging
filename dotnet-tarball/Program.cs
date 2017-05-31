using System.Diagnostics;

namespace Dotnet.Packaging
{
    class Program
    {
        static int Main(string[] args)
        {
            PackagingRunner runner = new PackagingRunner("tarball", "CreateTarball");
            return runner.Run(args);
        }
    }
}
