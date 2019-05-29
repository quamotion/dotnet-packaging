using System.Threading.Tasks;

namespace DotNet.ThirdParty
{
    interface ILicenseResolver
    {
        Task<License> GetSpdxLicense(string identifier, string licenseCacheFolder);
        Task<License> GetLicenseFromUrl(string url, string licenseCacheFolder);
        Task<License> GetLicenseFromGitHubUrl(string url, string licenseCacheFolder);
    }
}
