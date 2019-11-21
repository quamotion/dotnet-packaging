namespace Dotnet.Packaging
{
    class Program
    {
        static int Main(string[] args)
        {
            PackagingRunner runner = new PackagingRunner("tarball", "CreateTarball", "tarball");
            return runner.Run(args);
        }
    }
}
