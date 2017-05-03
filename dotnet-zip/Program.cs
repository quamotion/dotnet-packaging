using System.Diagnostics;

class Program
{
    static void Main(string[] args)
    {
        var msbArguments = $"msbuild /t:CreateZip";

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
