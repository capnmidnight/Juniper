using Juniper.TSBuild;

namespace Juniper.Examples
{
    public class BuildConfig : IBuildConfig
    {
        private const string ProjectName = "Juniper Web Examples";

        public BuildConfig()
        {
            var workingDir = new DirectoryInfo(".");
            var here = workingDir;
            while (here is not null && !here.CD(ProjectName, "src").Exists)
            {
                here = here.Parent;
            }

            if (here is null)
            {
                throw new DirectoryNotFoundException("Could not find project root from " + workingDir.FullName);
            }

            var projectOutDir = here.CD(ProjectName);
            var wwwRoot = projectOutDir.CD("wwwroot");
            var jsOutput = wwwRoot.CD("js");

            Options = new BuildSystemOptions
            {
                CleanDirs = new[]
                {
                    jsOutput
                },
                Project = projectOutDir
            };
        }

        public BuildSystemOptions Options { get; }
    }
}