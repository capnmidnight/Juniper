namespace Juniper.TSBuild
{
    public interface IBuildConfig
    {
        BuildSystemOptions Options { get; }
    }

    public class BuildSystemOptions
    {
        public DirectoryInfo[] CleanDirs;
        public DirectoryInfo InProject;
        public DirectoryInfo OutProject;
        public bool SkipNPMInstall;
        public bool SkipPreBuild;
        public DeploymentOptions Deployment;
        public DirectoryInfo[] AdditionalNPMProjects;
        public IEnumerable<BuildSystemDependency> Dependencies;
        public IEnumerable<BuildSystemDependency> OptionalDependencies;
        public (string Name, string Version, string Reason)[] BannedDependencies;
    }
}
