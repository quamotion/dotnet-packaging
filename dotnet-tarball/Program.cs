using System.Diagnostics;

class Program
{
    static void Main(string[] args)
    {
        var msbArguments = $"msbuild /t:CreateTarball";

        var psi = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = msbArguments
        };

        var process = new Process
        {
            StartInfo = psi,

        };

        process.Start();
        process.WaitForExit();
    }
}
