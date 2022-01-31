using Juniper;
using Juniper.IO;
using Juniper.Processes;

using System.Diagnostics;

using static System.Console;

using var stdIn = OpenStandardInput();
using var stdOut = OpenStandardOutput();
using var stdErr = OpenStandardError();
using var stdInEventer = new StreamReaderEventer(stdIn);

var running = true;
var hostPID = -1;
Process? host = null;
var cmdFactory = new JsonFactory<CommandProxyDescription>() { Formatting = Newtonsoft.Json.Formatting.None };
var commands = new Dictionary<int, ShellCommand>();
var tasks = new Dictionary<int, Task>();

stdInEventer.Line += (_, e) =>
{
    if (cmdFactory.TryParse(e.Value, out var desc))
    {
        switch (desc.Command)
        {
            case "start": Startup(desc); break;
            case "exec": StartCommand(desc); break;
            case "stop": hostPID = -1; break;
        }
    }
};

void Message(CommandProxyDescription? desc, string cmdName, params string[] messageParts)
{
    var cmd = cmdFactory.ToString(new CommandProxyDescription(desc, cmdName, messageParts));
    WriteLine(cmd);
}

void Send(string cmdName, params string[] messageParts)
{
    Message(null, cmdName, messageParts);
}

void Info(CommandProxyDescription? desc, string message)
{
    Message(desc, "info", message);
}

void Warn(CommandProxyDescription desc, string message)
{
    Message(desc, "warn", message);
}

void Error(CommandProxyDescription desc, string message)
{
    Message(desc, "error", message);
}

void Complete(CommandProxyDescription desc)
{
    Message(desc, "started");
}

void Startup(CommandProxyDescription desc)
{
    if (hostPID >= 0)
    {
        Error(desc, "Startup: hostPID already set");
        return;
    }

    if (desc.Args.Length != 1)
    {
        Error(desc, "Startup: Expected only 1 agument");
        return;
    }

    if (!int.TryParse(desc.Args[0], out var pid))
    {
        Error(desc, $"Startup: Argument not a valid integer ({desc.Args[0]})");
        return;
    }

    hostPID = pid;
    host = System.Diagnostics.Process.GetProcessById(hostPID);
    Complete(desc);
}

void StartCommand(CommandProxyDescription desc)
{
    if (hostPID < 0)
    {
        Error(desc, "StartCommand: No hostPID set.");
        return;
    }

    if (commands.ContainsKey(desc.TaskID))
    {
        Error(desc, $"StartCommand: Task {desc.TaskID} has already been started.");
        return;
    }

    if (desc.Args.Length == 0)
    {
        Error(desc, "StartCommand: No command name provided");
        return;
    }

    var cmd = new ShellCommand(
        desc.WorkingDir,
        desc.Args.First(),
        desc.Args.Skip(1).ToArray());

    commands.Add(desc.TaskID, cmd);

    Run(desc, cmd);
}

async void Run(CommandProxyDescription desc, ShellCommand cmd)
{
    void Cmd_Info(object? sender, StringEventArgs e)
    {
        Info(desc, e.Value);
    }

    void Cmd_Warning(object? sender, StringEventArgs e)
    {
        Warn(desc, e.Value);
    }

    void Cmd_Err(object? sender, Juniper.ErrorEventArgs e)
    {
        Error(desc, e.Value.Unroll());
    }

    void Cmd_Started(object? sender, EventArgs e)
    {
        Complete(desc);
    }

    cmd.Info += Cmd_Info;
    cmd.Warning += Cmd_Warning;
    cmd.Err += Cmd_Err;
    cmd.Started += Cmd_Started;
    try
    {
        var task = cmd.RunAsync();
        tasks.Add(desc.TaskID, task);
        await task;
        Message(desc, "ended");
    }
    catch (Exception ex)
    {
        Error(desc, ex.Unroll());
    }
    finally
    {
        tasks.Remove(desc.TaskID);
    }
    cmd.Info -= Cmd_Info;
    cmd.Warning -= Cmd_Warning;
    cmd.Err -= Cmd_Err;
    cmd.Started -= Cmd_Started;
}

async Task Shutdown()
{
    Send("Shutting down");
    stdInEventer.Stop();
    foreach(var cmd in commands.Values)
    {
        cmd.Kill();
    }
    var task = Task.WhenAll(tasks.Values);
    running = false;
    await task;
}

stdInEventer.Start();

Send("ready");

while (running)
{
    await Task.Delay(100);
    if (host?.HasExited == true)
    {
        await Shutdown();
    }
}

host?.Dispose();

var procs = Process.GetProcessesByName("esbuild");
foreach(var proc in procs)
{
    try
    {
        proc?.Kill(true);
    }
    catch(Exception ex)
    {
        WriteLine(ex.Unroll());
    }
}