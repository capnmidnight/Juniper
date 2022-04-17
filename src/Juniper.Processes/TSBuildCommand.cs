namespace Juniper.Processes
{
    public class TSBuildCommand : ShellCommand
    {
        public TSBuildCommand(DirectoryInfo? workingDir)
            : base(workingDir, "npx", "tsc --build")
        {
        }
    }
}
