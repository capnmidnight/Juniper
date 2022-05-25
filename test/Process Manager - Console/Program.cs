using Juniper.Processes;
using Juniper.TSBuild;

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

var typeScriptDir = juniperDir.CD("src", "Juniper.TypeScript");
var proxier = new CommandProxier(typeScriptDir);
var commandTree = new CommandTree();
commandTree.AddCommands(
    new ProxiedWatchCommand(proxier, "environment"),
    new ProxiedWatchCommand(proxier, "fetcher-worker")
);

proxier.Info += (_, e) => WriteLine("Proxy Info: " + e.Value);
proxier.Warning += (_, e) => WriteLine("Proxy Warning: " + e.Value);
proxier.Err += (_, e) => Error.WriteLine("Proxy Error: " + e.Value.Unroll());

commandTree.Info += (_, e) => WriteLine("Command Tree Info: " + e.Value);
commandTree.Warning += (_, e) => WriteLine("Command Tree Warning: " + e.Value);
commandTree.Err += (_, e) => Error.WriteLine("Command Tree Error: " + e.Value.Unroll());

await commandTree.ExecuteAsync();