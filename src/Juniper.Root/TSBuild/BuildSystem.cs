namespace Juniper.TSBuild
{
    public struct BuildSystemDependency
    {
        public string Name { get; set; }
        public string[] From { get; set; }
        public string[] To { get; set; }
    }

    public class BuildSystemOptions
    {
        public DirectoryInfo[] CleanDirs;
        public string InProjectName;
        public string OutProjectName;
        public Dictionary<string, (FileInfo From, FileInfo To)> Dependencies;
        public Dictionary<string, (FileInfo From, FileInfo To)> OptionalDependencies;
        public (string Name, string Version, string Reason)[] BannedDependencies;
    }
}
