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
        public DeploymentOptions Deployment;
        public Dictionary<string, (FileInfo From, FileInfo To)> Dependencies;
        public Dictionary<string, (FileInfo From, FileInfo To)> OptionalDependencies = new();
        public (string Name, string Version, string Reason)[] BannedDependencies;
    }
}
