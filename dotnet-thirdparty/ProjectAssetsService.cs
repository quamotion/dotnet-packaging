using Microsoft.Build.Locator;
using MSBuildProject = Microsoft.Build.Evaluation.Project;

namespace DotNet.ThirdParty
{
    class ProjectAssetsService : IProjectAssetsService
    {
        static ProjectAssetsService()
        {
            MSBuildLocator.RegisterDefaults();
        }

        public string ProjectAssetsPath { get; private set; }
        public string NuGetPackageRoot { get; private set; }
        public string IntermediateOutputPath { get; private set; }

        public bool AnalyzeProjectAsset(string projectFilePath, bool restore)
        {
            var project = new MSBuildProject(projectFilePath);

            if (restore)
            {
                if (!project.Build("Restore"))
                {
                    return false;
                }
            }

            this.ProjectAssetsPath = project.GetPropertyValue("ProjectAssetsFile");
            this.NuGetPackageRoot = project.GetPropertyValue("NugetPackageRoot");
            this.IntermediateOutputPath = project.GetPropertyValue("IntermediateOutputPath");
            return true;
        }
    }
}
