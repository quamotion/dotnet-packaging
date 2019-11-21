namespace Dotnet.Packaging
{
    public class Program
    {
        public static int Main(string[] args)
        {
            PackagingRunner runner = new PackagingRunner("RPM package", "CreateRpm", "rpm");
            return runner.Run(args);
        }
    }
}
