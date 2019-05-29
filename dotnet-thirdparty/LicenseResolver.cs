using Newtonsoft.Json;
using System;
using System.IO;
using System.IO.Abstractions;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DotNet.ThirdParty
{
    class LicenseResolver : ILicenseResolver
    {
        private readonly IFileSystem fileSystem;
        private readonly JsonSerializer serializer = new JsonSerializer();
        private readonly SHA256 sha = SHA256.Create();

        public LicenseResolver(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        }

        public async Task<License> GetSpdxLicense(string identifier, string licenseCacheFolder)
        {
            var licensePath = await this.DownloadFile($"http://spdx.org/licenses/{identifier}.json", $"{identifier}.json", licenseCacheFolder).ConfigureAwait(false);

            using (var stream = this.fileSystem.File.OpenRead(licensePath))
            using (var streamReader = new StreamReader(stream))
            using (var jsonTextReader = new JsonTextReader(streamReader))
            {
                var licenseEntry = serializer.Deserialize<SpdxLicenseEntry>(jsonTextReader);

                return new License()
                {
                    Name = licenseEntry.Name,
                    LicenseId = licenseEntry.LicenseId,
                    Text = licenseEntry.LicenseText
                };
            }
        }

        public async Task<License> GetLicenseFromUrl(string url, string licenseCacheFolder)
        {
            var licensePath = await this.DownloadFile(url, licenseCacheFolder).ConfigureAwait(false);

            // Something went wrong downloading the license file. Bail out.
            if (!this.fileSystem.File.Exists(licensePath))
            {
                return null;
            }

            return new License()
            {
                Text = this.fileSystem.File.ReadAllText(licensePath)
            };
        }

        public async Task<License> GetLicenseFromGitHubUrl(string url, string licenseCacheFolder)
        {
            // Get the owner and repo
            string[] parts = url.Substring(8).Split(new char[] { '/' });
            var owner = parts[1];
            var repo = parts[2];

            var licenseUrl = $"https://api.github.com/repos/{owner}/{repo}/license";
            var localName = $"github_{owner}_{repo}.json";
            
            var licensePath = await this.DownloadFile(licenseUrl, localName, licenseCacheFolder).ConfigureAwait(false);

            // Something went wrong downloading the license file. Bail out.
            if (!this.fileSystem.File.Exists(licensePath))
            {
                return null;
            }

            using (var stream = this.fileSystem.File.OpenRead(licensePath))
            using (var streamReader = new StreamReader(stream))
            using (var jsonTextReader = new JsonTextReader(streamReader))
            {
                var licenseEntry = serializer.Deserialize<GitHubProjectLicense>(jsonTextReader);

                if (licenseEntry.License.Spdxid != "NOASSERTION")
                {
                    return await this.GetSpdxLicense(licenseEntry.License.Spdxid, licenseCacheFolder);
                }
                else
                {
                    var licenseText = await this.DownloadFile(licenseEntry.DownloadUrl, licenseCacheFolder);

                    return new License()
                    {
                        Text = this.fileSystem.File.ReadAllText(licenseText)
                    };
                };
            }
        }

        public Task<string> DownloadFile(string url, string licenseCacheFolder)
        {
            var hash = BitConverter.ToString(this.sha.ComputeHash(Encoding.UTF8.GetBytes(url))).Replace("-", string.Empty);
            return DownloadFile(url, hash, licenseCacheFolder);
        }

        public async Task<string> DownloadFile(string url, string localName, string licenseCacheFolder)
        {
            var localPath = this.fileSystem.Path.Combine(licenseCacheFolder, localName);

            if (!this.fileSystem.File.Exists(localPath))
            {
                try
                {
                    HttpClient client = new HttpClient();
                    client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("dotnet-thirdparty", "1.0.0"));

                    using (var response = await client.GetAsync(url).ConfigureAwait(false))
                    using (var remoteStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                    using (var localStream = File.Open(localPath, FileMode.Create, FileAccess.Write))
                    {
                        await remoteStream.CopyToAsync(localStream).ConfigureAwait(false);

                        response.EnsureSuccessStatusCode();
                    }
                }
                catch (HttpRequestException)
                {
                    this.fileSystem.File.Delete(localPath);
                    return null;
                }
            }

            return localPath;
        }
    }
}
