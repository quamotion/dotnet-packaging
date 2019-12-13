using McMaster.Extensions.CommandLineUtils;
using Microsoft.Build.Locator;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using ConsoleLogger = Microsoft.Build.Logging.ConsoleLogger;
using IMSBuildLogger = Microsoft.Build.Framework.ILogger;
using LoggerVerbosity = Microsoft.Build.Framework.LoggerVerbosity;
using MSBuild = Microsoft.Build.Evaluation;

namespace Dotnet.Packaging
{
    public class PackagingRunner
    {
        private readonly string outputName;
        private readonly string msbuildTarget;
        private readonly string commandName;

        public PackagingRunner(string outputName, string msbuildTarget, string commandName)
        {
            MSBuildLocator.RegisterDefaults();

            this.outputName = outputName;
            this.msbuildTarget = msbuildTarget;
            this.commandName = commandName;
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
              $"Target framework of the {outputName}. The target framework has to be specified in the project file.",
              CommandOptionType.SingleValue);

            CommandOption configuration = commandLineApplication.Option(
              "-c | --configuration <configuration>",
              $"Target configuration of the {outputName}. The default for most projects is 'Debug'.",
              CommandOptionType.SingleValue);

            CommandOption output = commandLineApplication.Option(
              "-o | --output <output-dir>",
              $"The output directory to place built packages in. The default is the output directory of your project.",
              CommandOptionType.SingleValue);

            CommandOption versionSuffix = commandLineApplication.Option(
              "--version-suffix <version-suffix>",
              "Defines the value for the $(VersionSuffix) property in the project.",
              CommandOptionType.SingleValue);

            CommandOption noRestore = commandLineApplication.Option(
                "--no-restore",
                "Do not restore the project before building.",
                CommandOptionType.NoValue);

            CommandArgument projectArgument = commandLineApplication.Argument(
                "project",
                "The project file to operate on. If a file is not specified, the command will search the current directory for one.");

            commandLineApplication.VersionOption("-v | --version", new Version(ThisAssembly.AssemblyFileVersion).ToString(3), ThisAssembly.AssemblyInformationalVersion);

            commandLineApplication.HelpOption("-? | -h | --help");
            commandLineApplication.Name = $"dotnet {this.commandName}";
            commandLineApplication.FullName = $"dotnet {this.commandName}";
            commandLineApplication.ShortVersionGetter = () => new Version(ThisAssembly.AssemblyFileVersion).ToString(3);
            commandLineApplication.LongVersionGetter = () => ThisAssembly.AssemblyInformationalVersion;

            commandLineApplication.ExtendedHelpText = $"{Environment.NewLine}See https://github.com/qmfrederik/dotnet-packaging for more information";

            var installCommand = commandLineApplication.Command(
                "install",
                command => command.OnExecute(() =>
                {
                    Console.WriteLine($"dotnet {this.commandName} ({ThisAssembly.AssemblyInformationalVersion})");

                    // Create/update the Directory.Build.props file in the directory of the version.json file to add the Packaging.Targets package.
                    string directoryBuildPropsPath = Path.Combine(Environment.CurrentDirectory, "Directory.Build.props");
                    MSBuild.Project propsFile;
                    if (File.Exists(directoryBuildPropsPath))
                    {
                        propsFile = new MSBuild.Project(directoryBuildPropsPath);
                    }
                    else
                    {
                        propsFile = new MSBuild.Project();
                    }

                    const string PackageReferenceItemType = "PackageReference";
                    const string PackageId = "Packaging.Targets";
                    if (!propsFile.GetItemsByEvaluatedInclude(PackageId).Any(i => i.ItemType == PackageReferenceItemType && i.EvaluatedInclude == PackageId))
                    {
                        // Using the -* suffix will allow us to match both released and prereleased versions, in the absence
                        // of https://github.com/AArnott/Nerdbank.GitVersioning/issues/409
                        string packageVersion = $"{new Version(ThisAssembly.AssemblyFileVersion).ToString(3)}-*";
                        propsFile.AddItem(
                            PackageReferenceItemType,
                            PackageId,
                            new Dictionary<string, string>
                            {
                                { "Version", packageVersion },
                                { "PrivateAssets", "all" },
                            });

                        propsFile.Save(directoryBuildPropsPath);
                    }

                    Console.WriteLine($"Successfully installed dotnet {this.commandName}. Now run 'dotnet {this.commandName}' to package your");
                    Console.WriteLine($"application as a {this.outputName}");
                }));

            commandLineApplication.OnExecute(() =>
            {
                Console.WriteLine($"dotnet {this.commandName} ({ThisAssembly.AssemblyInformationalVersion})");

                if (!TryGetProjectFilePath(projectArgument, out string projectFilePath))
                {
                    return -1;
                }

                if (!noRestore.HasValue())
                {
                    if (!this.IsPackagingTargetsInstalled(projectFilePath))
                    {
                        return -1;
                    }
                }

                StringBuilder msbuildArguments = new StringBuilder();
                msbuildArguments.Append($"msbuild /t:{msbuildTarget} ");

                if (runtime.HasValue())
                {
                    msbuildArguments.Append($"/p:RuntimeIdentifier={runtime.Value()} ");
                }

                if (framework.HasValue())
                {
                    msbuildArguments.Append($"/p:TargetFramework={framework.Value()} ");
                }

                if (configuration.HasValue())
                {
                    msbuildArguments.Append($"/p:Configuration={configuration.Value()} ");
                }

                if (versionSuffix.HasValue())
                {
                    msbuildArguments.Append($"/p:VersionSuffix={versionSuffix.Value()} ");
                }

                if (output.HasValue())
                {
                    msbuildArguments.Append($"/p:PackageDir={output.Value()} ");
                }

                msbuildArguments.Append($"{projectFilePath} ");

                return RunDotnet(msbuildArguments);
            });

            return commandLineApplication.Execute(args);
        }

        public int RunDotnet(StringBuilder msbuildArguments)
        {
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
        }

        public bool IsPackagingTargetsInstalled(string projectFilePath)
        {
            var loggers = new IMSBuildLogger[] { new ConsoleLogger(LoggerVerbosity.Quiet) };
            var project = new MSBuild.Project(projectFilePath);

            if (!project.Build("Restore", loggers))
            {
                Console.Error.WriteLine($"Failed to restore '{Path.GetFileName(projectFilePath)}'. Please run dotnet restore, and try again.");
                return false;
            }

            var projectAssetsPath = project.GetPropertyValue("ProjectAssetsFile");

            // NuGet has a LockFileUtilities.GetLockFile API which provides direct access to this file format,
            // but loading NuGet in the same process as MSBuild creates dependency conflicts.
            LockFile lockFile = null;
            using (StreamReader reader = File.OpenText(projectAssetsPath))
            using (JsonReader jsonReader = new JsonTextReader(reader))
            {
                JsonSerializer serializer = new JsonSerializer();
                lockFile = serializer.Deserialize<LockFile>(jsonReader);
            }

            if (!lockFile.Libraries.Any(l => l.Key.StartsWith("Packaging.Targets/")))
            {
                Console.Error.WriteLine($"The project '{Path.GetFileName(projectFilePath)}' doesn't have a PackageReference to Packaging.Targets.");
                Console.Error.WriteLine($"Please run 'dotnet {this.commandName} install', and try again.");
                return false;
            }

            return true;
        }

        private bool TryGetProjectFilePath(CommandArgument projectArgument, out string projectFilePath)
        {
            if (projectArgument == null)
            {
                throw new ArgumentNullException(nameof(projectArgument));
            }

            if (!string.IsNullOrWhiteSpace(projectArgument.Value))
            {
                projectFilePath = projectArgument.Value;

                if (!File.Exists(projectFilePath))
                {
                    Console.Error.WriteLine($"Could not find the project file '{projectFilePath}'.");
                    return false;
                }

                return true;
            }

            projectFilePath = Directory.GetFiles(Environment.CurrentDirectory, "*.*proj").SingleOrDefault();

            if (projectFilePath == null)
            {
                Console.Error.WriteLine($"Failed to find a .*proj file in '{Environment.CurrentDirectory}'. dotnet {this.commandName} only works if");
                Console.Error.WriteLine($"you have exactly one .*proj file in your directory. For advanced scenarios, please use 'dotnet msbuild /t:{this.msbuildTarget}'");
                return false;
            }

            return true;
        }

        class LockFile
        {
            public Dictionary<string, object> Libraries;
        }
    }
}
