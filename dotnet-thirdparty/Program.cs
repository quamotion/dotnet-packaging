using DotNetOutdated.Exceptions;
using DotNetOutdated.Services;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Nustache.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Threading.Tasks;

namespace DotNet.ThirdParty
{
    class Program
    {
        static int Main(string[] args)
        {
            using (var services = new ServiceCollection()
                    .AddSingleton<IFileSystem, FileSystem>()
                    .AddSingleton<IProjectDiscoveryService, ProjectDiscoveryService>()
                    .AddSingleton<ILicenseAnalysisService, LicenseAnalysisService>()
                    .AddSingleton<IProjectAssetsService, ProjectAssetsService>()
                    .AddSingleton<ILicenseResolver, LicenseResolver>()
                    .BuildServiceProvider())
            {
                var app = new CommandLineApplication<Program>
                {
                    ThrowOnUnexpectedArgument = true,
                    FullName = $"dotnet thirdparty",
                    LongVersionGetter = () => ThisAssembly.AssemblyInformationalVersion,
                    ExtendedHelpText = $"{Environment.NewLine}See https://github.com/qmfrederik/dotnet-packaging for more information"
                };
                app.Conventions
                    .UseDefaultConventions()
                    .UseConstructorInjection(services);

                return app.Execute(args);
            }
        }

        private readonly IFileSystem fileSystem;
        private readonly IProjectDiscoveryService projectDiscoveryService;

        [Argument(0, Description = "The path to a .csproj or .fsproj file, or to a directory containing a .NET Core project. " +
                                   "If none is specified, the current directory will be used.")]
        public string Path { get; set; }

        [Argument(1, "--no-restore", Description = "Do not restore the project before analyzing the third party license notices.")]
        public bool SkipRestore { get; set; } = true;

        private readonly ILicenseAnalysisService licenseAnalysisService;
        private readonly IProjectAssetsService projectAssetsService;

        public Program(IFileSystem fileSystem, IProjectDiscoveryService projectDiscoveryService, IProjectAssetsService projectAssetsService, ILicenseAnalysisService licenseAnalysisService)
        {
            this.fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
            this.projectDiscoveryService = projectDiscoveryService ?? throw new ArgumentNullException(nameof(projectDiscoveryService));
            this.projectAssetsService = projectAssetsService ?? throw new ArgumentNullException(nameof(projectAssetsService));
            this.licenseAnalysisService = licenseAnalysisService ?? throw new ArgumentNullException(nameof(licenseAnalysisService));
        }

        public async Task<int> OnExecute(CommandLineApplication app, IConsole console)
        {
            try
            {
                Console.WriteLine($"dotnet thirdparty ({ThisAssembly.AssemblyInformationalVersion})");

                // If no path is set, use the current directory
                if (string.IsNullOrEmpty(Path))
                    Path = this.fileSystem.Directory.GetCurrentDirectory();

                // Get all the projects
                console.Write("Discovering project...");
                string projectFilePath = this.projectDiscoveryService.DiscoverProject(Path);
                console.WriteLine();

                console.Write($"Analyzing '{projectFilePath}' dependencies");
                if (!this.projectAssetsService.AnalyzeProjectAsset(projectFilePath, restore: !this.SkipRestore))
                {
                    return 1;
                }

                var licenseCachePath = this.fileSystem.Path.Combine(this.projectAssetsService.IntermediateOutputPath, "ThirdPartyNotices");
                if (!this.fileSystem.Directory.Exists(licenseCachePath))
                {
                    this.fileSystem.Directory.CreateDirectory(licenseCachePath);
                }

                var licenses = await this.licenseAnalysisService.AnalyzeProject(this.projectAssetsService.ProjectAssetsPath, this.projectAssetsService.NuGetPackageRoot, licenseCachePath).ConfigureAwait(false);

                // Cache the license information in a JSON file
                using (var stream = this.fileSystem.File.Open(this.fileSystem.Path.Combine(this.projectAssetsService.IntermediateOutputPath, "ThirdPartyNotices", "licenses.json"), FileMode.Create, FileAccess.Write))
                using (var streamWriter = new StreamWriter(stream))
                using (var jsonTextWriter = new JsonTextWriter(streamWriter))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(jsonTextWriter, licenses);
                }

                Helpers.Register(nameof(TitleLine), TitleLine);

                // Inspiration: https://www.nexb.com/blog/oss_attribution_obligations.html
                this.fileSystem.File.WriteAllText(
                    this.fileSystem.Path.Combine(this.projectAssetsService.IntermediateOutputPath, "ThirdPartyNotices", "THIRD-PARTY-NOTICES.TXT"),
                    Render.FileToString("THIRD-PARTY-NOTICES.TXT.template", licenses));

                console.WriteLine();
                return 0;
            }
            catch (CommandValidationException e)
            {
                console.Error.WriteLine(e.Message);

                return 1;
            }
        }

        private static void TitleLine(RenderContext context, IList<object> arguments, IDictionary<string, object> options, RenderBlock fn, RenderBlock inverse)
        {
            if (arguments != null && arguments.Count > 0 && arguments[0] != null && arguments[0] is string)
            {
                context.Write(TitleLine(arguments[0] as string));
            }
        }

        private static string TitleLine(string title)
        {
            return new string('-', title.Length);
        }
    }
}
