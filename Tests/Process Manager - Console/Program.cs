using Juniper.Processes;

using static System.Console;

var juniperDir = new DirectoryInfo(Environment.CurrentDirectory);
while (juniperDir is not null
    && juniperDir.Name != "Juniper")
{
    juniperDir = juniperDir.Parent;
}

if (juniperDir is null)
{
    Error.WriteLine("Can't find start directory");
    return;
}

var typeScriptDir = juniperDir.CD("test", "Juniper Web Examples");
var commandTree = new CommandTree();
commandTree.AddCommands(new ShellCommand(typeScriptDir, "npm", "install"));
commandTree.AddCommands(new ShellCommand(typeScriptDir, "npm", "run", "watch"));

commandTree.Info += (_, e) => WriteLine("Command Tree Info: " + e.Value);
commandTree.Warning += (_, e) => WriteLine("Command Tree Warning: " + e.Value);
commandTree.Err += (_, e) => Error.WriteLine("Command Tree Error: " + e.Value.Unroll());

await commandTree.ExecuteAsync(CancellationToken.None);