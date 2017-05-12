using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Diagnostics;
using System.Text;

public class Program
{
    public static int Main(string[] args)
    {
        CommandLineApplication commandLineApplication = new CommandLineApplication(throwOnUnexpectedArg: true);

        CommandOption runtime = commandLineApplication.Option(
          "-r |--runtime <runtime>",
          "Target runtime of the RPM package. The target runtime has to be specified in the project file.",
          CommandOptionType.SingleValue);

        CommandOption framework = commandLineApplication.Option(
          "-f | --framework <framework>",
          "Required. Target framework of the RPM package. The target framework has to be specified in the project file.",
          CommandOptionType.SingleValue);

        CommandOption configuration = commandLineApplication.Option(
          "-c | --configuration <configuration>",
          "Required. Target configuration of the RPM package. The default for most projects is 'Debug'.",
          CommandOptionType.SingleValue);

        CommandOption versionSuffix = commandLineApplication.Option(
          "---version-suffix <version-suffix>",
          "Defines the value for the $(VersionSuffix) property in the project.",
          CommandOptionType.SingleValue);

        commandLineApplication.HelpOption("-? | -h | --help");

        commandLineApplication.OnExecute(() =>
        {
            if (!framework.HasValue())
            {
                Console.WriteLine("You must specify a target framework.");
                return -1;
            }

            if (!runtime.HasValue())
            {
                Console.WriteLine("You must specify a target runtime.");
                return -1;
            }

            StringBuilder msbuildArguments = new StringBuilder();
            msbuildArguments.Append("msbuild /t:CreateRpm ");
            msbuildArguments.Append($"/p:RuntimeIdentifier={runtime.Value()} ");
            msbuildArguments.Append($"/p:TargetFramework={framework.Value()} ");

            if (configuration.HasValue())
            {
                msbuildArguments.Append($"/p:Configuration={configuration.Value()} ");
            }

            if (versionSuffix.HasValue())
            {
                msbuildArguments.Append($"/p:VersionSuffix={versionSuffix.Value()} ");
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
