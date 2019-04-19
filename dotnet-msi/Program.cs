namespace Dotnet.Packaging
{
    class Program
    {
        static int Main(string[] args)
        {
            PackagingRunner runner = new PackagingRunner("Windows Installer package", "CreateMsi");
            return runner.Run(args);
        }
    }
}
