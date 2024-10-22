namespace Juniper.Processes;

public class SSHCommand : ShellCommand
{
    public static Func<string, string, SSHCommand> WithAuth(FileInfo keyFile, string userName, string hostName) =>
        (string name, string script) => new SSHCommand(
                name,
                keyFile,
                userName,
                hostName,
                script);

    public static Func<string, string, SSHCommand> WithAuth(DirectoryInfo workingDir, FileInfo keyFile, string userName, string hostName) =>
        (string name, string script) => new SSHCommand(
            name,
            workingDir,
            keyFile,
            userName,
            hostName,
            script);

    private static string FormatScript(string remoteScript) =>
        $"\"{remoteScript.Replace("\"", "\\\"")
            .ReplaceLineEndings("\n")}\"";

    public SSHCommand(string name, DirectoryInfo? workingDir, FileInfo keyFile, string userName, string hostName, string remoteScript) :
        base(workingDir,
            "ssh",
            "-i", keyFile.FullName,
            $"{userName}@{hostName}",
            FormatScript(remoteScript))
    {
        CommandName = name;
    }

    public SSHCommand(string name, FileInfo keyFile, string userName, string hostName, string remoteScript) :
        base("ssh",
            "-i", keyFile.FullName,
            $"{userName}@{hostName}",
            FormatScript(remoteScript))
    {
        CommandName = name;
    }
}
