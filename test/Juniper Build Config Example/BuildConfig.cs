using Juniper.TSBuild;

namespace Juniper.Examples
{
    public static class BuildConfig
    {
        private const string ProjectName = "Juniper Web Examples";

        public static BuildSystemOptions GetBuildConfig()
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

            var options = new BuildSystemOptions()
            {
                CleanDirs = new[] 
                {
                    jsOutput
                },
                InProjectName = ProjectName,
                OutProjectName = ProjectName
            };

            options.OptionalDependencies = new();

            return options;
        }
    }
}