namespace Dotnet.Packaging
{
    class Program
    {
        static int Main(string[] args)
        {
            PackagingRunner runner = new PackagingRunner("zip archive", "CreateZip", "zip");
            return runner.Run(args);
        }
    }
}
