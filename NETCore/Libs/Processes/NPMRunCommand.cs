namespace Juniper.Processes;

public class NPMRunCommand : ShellCommand
{
    public NPMRunCommand(FileInfo packageJson, string command)
        : base(packageJson.Directory, "npm", "run", command)
    {
    }
}
