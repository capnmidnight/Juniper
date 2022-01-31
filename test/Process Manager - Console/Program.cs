using Juniper.IO;
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

var processManagerDir = juniperDir.CD("src", "Juniper.ProcessManager");
var typeScriptDir = juniperDir.CD("src", "Juniper.TypeScript");
var fetcherWorkerDir = typeScriptDir.CD("fetcher-worker");
var environmentDir = typeScriptDir.CD("environment");
var cmdFactory = new JsonFactory<CommandProxyDescription>()
{
    Formatting = Newtonsoft.Json.Formatting.None
};

var processManager = new ShellCommand(processManagerDir, "dotnet", "run", "--no-build");

var runningTasks = new Dictionary<int, TaskCompletionSource>();
var resolvesOn = new Dictionary<int, string>();
var taskCounter = 0;

Task Message(DirectoryInfo? workingDir, string resolveOn, string command, params string[] args)
{
    var taskID = ++taskCounter;
    var completer = new TaskCompletionSource();
    runningTasks?.Add(taskID, completer);
    resolvesOn?.Add(taskID, resolveOn);
    var cmd = cmdFactory.ToString(new CommandProxyDescription(taskID, workingDir, command, args));
    WriteLine("Sending: " + cmd);
    processManager?.Send(cmd);
    return completer.Task;
}

Task Send(string resolveOn, string command, params string[] args)
{
    return Message(null, resolveOn, command, args);
}

async Task Start()
{
    await Send("started", "start", Environment.ProcessId.ToString());
    await Message(fetcherWorkerDir, "started", "exec", "npm", "run", "watch");
    await Message(environmentDir, "started", "exec", "npm", "run", "watch");
}

void DeleteTask(int taskID)
{
    if (runningTasks is not null
        && runningTasks.ContainsKey(taskID))
    {
        runningTasks.Remove(taskID);
        resolvesOn.Remove(taskID);
    }
}

processManager.Info += (_, e) =>
{
    WriteLine("Info: " + e.Value);
    if (cmdFactory.TryParse(e.Value, out var cmd))
    {
        if (cmd.TaskID > 0
            && runningTasks is not null
            && runningTasks.ContainsKey(cmd.TaskID))
        {
            var completer = runningTasks[cmd.TaskID];
            var resolveOn = resolvesOn[cmd.TaskID];
            if (cmd.Command == "error")
            {
                completer.SetException(new Exception(cmd.Args.Join(Environment.NewLine))); DeleteTask(cmd.TaskID);
            }
            else if (cmd.Command == resolveOn)
            {
                completer.SetResult();
                DeleteTask(cmd.TaskID);
            }
        }
        else
        {
            switch (cmd.Command)
            {
                case "ready": _ = Start(); break;
            }
        }
    }
};

processManager.Warning += (_, e) => WriteLine("Warning: " + e.Value);
processManager.Err += (_, e) => Error.WriteLine("Error: " + e.Value.Unroll());
processManager.Started += (_, e) => WriteLine("Process manager started");

var task = processManager.RunAsync();
await task;