namespace Juniper.TSBuild
{
    public class BuildSystemProjectRootNotFoundException : DirectoryNotFoundException
    {
        public BuildSystemProjectRootNotFoundException(string message)
            : base(message)
        { }
    }
}
