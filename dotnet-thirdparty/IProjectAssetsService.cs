namespace DotNet.ThirdParty
{
    interface IProjectAssetsService
    {
        string ProjectAssetsPath { get; }
        string NuGetPackageRoot { get; }
        string IntermediateOutputPath { get; }

        bool AnalyzeProjectAsset(string projectFilePath, bool restore);
    }
}
