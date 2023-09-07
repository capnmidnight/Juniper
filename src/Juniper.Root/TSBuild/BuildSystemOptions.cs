namespace Juniper.TSBuild
{
    public interface IBuildConfig
    {
        BuildSystemOptions Options { get; }
    }

    public class BuildSystemOptions
    {
        public DirectoryInfo[] CleanDirs;
        public string InProjectName;
        public string OutProjectName;
        public bool SkipNPMInstall;
        public DeploymentOptions Deployment;
        public DirectoryInfo[] AdditionalNPMProjects;
        public IEnumerable<BuildSystemDependency> Dependencies;
        public IEnumerable<BuildSystemDependency> OptionalDependencies;
        public (string Name, string Version, string Reason)[] BannedDependencies;
    }
}
