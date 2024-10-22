using Juniper.TSBuild;

namespace Juniper.Examples
{
    public class BuildConfig : IBuildConfig
    {
        private const string ProjectName = "Juniper Web Examples";

        public BuildSystemOptions Options
        {
            get
            {
                var here = BuildSystemOptions.FindSolutionRoot(ProjectName);
                var projectOutDir = here.CD(ProjectName);
                var wwwRoot = projectOutDir.CD("wwwroot");
                var jsOutput = wwwRoot.CD("js");

                return new BuildSystemOptions
                {
                    Project = projectOutDir,
                    CleanDirs = [jsOutput]
                };
            }
        }
    }
}