using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Diagnostics;
using System.Text;

namespace Dotnet.Packaging
{
    public class Program
    {
        public static int Main(string[] args)
        {
            PackagingRunner runner = new PackagingRunner("RPM package", "CreateRpm");
            return runner.Run(args);
        }
    }
}
