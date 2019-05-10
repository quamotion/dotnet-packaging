using McMaster.Extensions.CommandLineUtils;
using System;
using System.Diagnostics;
using System.Text;

namespace Dotnet.Packaging
{
    public class PackagingRunner
    {
        private readonly string outputName;
        private readonly string msbuildTarget;

        public PackagingRunner(string outputName, string msbuildTarget)
        {
            this.outputName = outputName;
            this.msbuildTarget = msbuildTarget;
        }

        public int Run(string[] args)
        {
            CommandLineApplication commandLineApplication = new CommandLineApplication(throwOnUnexpectedArg: true);

            CommandOption runtime = commandLineApplication.Option(
              "-r |--runtime <runtime>",
              $"Target runtime of the {outputName}. The target runtime has to be specified in the project file.",
              CommandOptionType.SingleValue);

            CommandOption framework = commandLineApplication.Option(
              "-f | --framework <framework>",
              $"Required. Target framework of the {outputName}. The target framework has to be specified in the project file.",
              CommandOptionType.SingleValue);

            CommandOption configuration = commandLineApplication.Option(
              "-c | --configuration <configuration>",
              $"Required. Target configuration of the {outputName}. The default for most projects is 'Debug'.",
              CommandOptionType.SingleValue);

            CommandOption version = commandLineApplication.Option(
              "-v | --version <version>",
              "Defines the version of the package. The default is 1.0.0.",
              CommandOptionType.SingleValue);

            commandLineApplication.HelpOption("-? | -h | --help");
            commandLineApplication.FullName = $"dotnet {this.outputName}";
            commandLineApplication.LongVersionGetter = () => ThisAssembly.AssemblyInformationalVersion;

            commandLineApplication.ExtendedHelpText = $"{Environment.NewLine}See https://github.com/qmfrederik/dotnet-packaging for more information";

            commandLineApplication.OnExecute(() =>
            {
                Console.WriteLine($"dotnet {this.outputName} ({ThisAssembly.AssemblyInformationalVersion})");

                if (!framework.HasValue())
                {
                    Console.WriteLine("You must specify a target framework.");
                    commandLineApplication.ShowHint();

                    return -1;
                }

                if (!runtime.HasValue())
                {
                    Console.WriteLine("You must specify a target runtime.");
                    commandLineApplication.ShowHint();

                    return -1;
                }

                StringBuilder msbuildArguments = new StringBuilder();
                msbuildArguments.Append($"msbuild /t:{msbuildTarget} ");
                msbuildArguments.Append($"/p:RuntimeIdentifier={runtime.Value()} ");
                msbuildArguments.Append($"/p:TargetFramework={framework.Value()} ");

                if (configuration.HasValue())
                {
                    msbuildArguments.Append($"/p:Configuration={configuration.Value()} ");
                }

                if (version.HasValue())
                {
                    msbuildArguments.Append($"/p:Version={version.Value()} ");
                }

                var psi = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = msbuildArguments.ToString()
                };

                var process = new Process
                {
                    StartInfo = psi,

                };

                process.Start();
                process.WaitForExit();

                return process.ExitCode;
            });

            return commandLineApplication.Execute(args);
        }
    }
}
