using NuGet.Common;
using NuGet.Packaging.Core;
using NuGet.Packaging.Licenses;
using NuGet.ProjectModel;
using NuGet.Protocol;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;

namespace DotNet.ThirdParty
{
    internal class LicenseAnalysisService : ILicenseAnalysisService
    {
        private readonly IFileSystem fileSystem;
        private readonly ILicenseResolver licenseResolver;

        public LicenseAnalysisService(IFileSystem fileSystem, ILicenseResolver licenseResolver)
        {
            this.fileSystem = fileSystem;
            this.licenseResolver = licenseResolver;
        }

        public async Task<List<ThirdPartyNotice>> AnalyzeProject(string projectAssetsPath, string nugetPackageRoot, string licenseCacheDirectory)
        {
            List<ThirdPartyNotice> notices = new List<ThirdPartyNotice>();

            // Load the lock file
            string lockFilePath = this.fileSystem.Path.Combine(projectAssetsPath);
            var lockFile = LockFileUtilities.GetLockFile(lockFilePath, NullLogger.Instance);

            // TODO: Make this selection dynamic
            var target = lockFile.Targets[0];

            foreach (var library in target.Libraries)
            {
                bool isDeployed =
                    library.RuntimeAssemblies.Any()
                    || library.NativeLibraries.Any()
                    || library.ContentFiles.Any(c => c.CopyToOutput);

                if (!isDeployed)
                {
                    Debug.WriteLine($"Ignoring library {library.Name} because it has no runtime assemblies, native libraries or content files");
                    continue;
                }

                var packageIdentity = new PackageIdentity(library.Name, library.Version);
                var packageInfo = LocalFolderUtility.GetPackageV3(nugetPackageRoot, packageIdentity, NullLogger.Instance);

                if (packageInfo == null)
                {
                    Debug.WriteLine($"Ignoring library {library.Name} because its NuGet package could not be found");
                    continue;
                }

                var copyright = !string.IsNullOrWhiteSpace(packageInfo.Nuspec.GetCopyright()) ? packageInfo.Nuspec.GetCopyright() : $"Copyright (c) {packageInfo.Nuspec.GetAuthors()}";
                var licenseMetadata = packageInfo.Nuspec.GetLicenseMetadata();
                var licenseUrl = packageInfo.Nuspec.GetLicenseUrl();
                var projectUrl = packageInfo.Nuspec.GetProjectUrl();
                var packageName = !string.IsNullOrWhiteSpace(packageInfo.Nuspec.GetTitle()) ? packageInfo.Nuspec.GetTitle() : packageInfo.Nuspec.GetId();
                var packageUrl = packageInfo.Nuspec.GetProjectUrl();

                if (copyright == null && licenseMetadata == null && licenseUrl == null)
                {
                    Debug.WriteLine($"Ignoring library {library.Name} because no license information could be found");
                    continue;
                }

                License license = null;

                if (license == null
                    && licenseMetadata != null
                    && licenseMetadata.LicenseExpression.Type == LicenseExpressionType.License)
                {
                    var nugetLicense = (NuGetLicense)licenseMetadata.LicenseExpression;
                    license = await this.licenseResolver.GetSpdxLicense(nugetLicense.Identifier, licenseCacheDirectory).ConfigureAwait(false);
                }
                else if (license == null
                    && licenseUrl != null
                    && licenseUrl.Contains("github.com/"))
                {
                    license = await this.licenseResolver.GetLicenseFromGitHubUrl(licenseUrl, licenseCacheDirectory).ConfigureAwait(false);
                }
                else if (license == null
                    && projectUrl.Contains("github.com/"))
                {
                    license = await this.licenseResolver.GetLicenseFromGitHubUrl(projectUrl, licenseCacheDirectory).ConfigureAwait(false);
                }
                else if (license == null
                    && licenseUrl != null)
                {
                    license = await this.licenseResolver.GetLicenseFromUrl(licenseUrl, licenseCacheDirectory).ConfigureAwait(false);
                }

                if (license != null)
                {
                    notices.Add(new ThirdPartyNotice()
                        {
                            Copyright = copyright,
                            PackageName = packageName,
                            PackageUrl = packageUrl,
                            License = license
                        });
                }
                else
                {
                    Debug.WriteLine($"Ignoring library {library.Name} because no license information could be found");
                }

            }

            return notices;

        }
    }
}
