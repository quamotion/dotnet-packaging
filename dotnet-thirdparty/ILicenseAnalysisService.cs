using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotNet.ThirdParty
{
    interface ILicenseAnalysisService
    {
        Task<List<ThirdPartyNotice>> AnalyzeProject(string projectAssetsPath, string nugetPackageRoot, string licenseCacheDirectory);
    }
}
