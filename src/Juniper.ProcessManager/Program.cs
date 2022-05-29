using Juniper;
using Juniper.IO;
using Juniper.Processes;

using System.Diagnostics;

using static System.Console;

if (args.Length != 1)
{
    Console.Error.WriteLine("Excepted parent process ID to be passed as an argument");
    return;
}

var hostPID = -1;
if (!int.TryParse(args[0], out hostPID))
{
    Console.Error.WriteLine("Parent process ID argument was not an integer.");
    return;
}

using var host = Process.GetProcessById(hostPID);
using var stdIn = OpenStandardInput();
using var stdOut = OpenStandardOutput();
using var stdErr = OpenStandardError();
using var stdInEventer = new StreamReaderEventer(stdIn);

var running = true;
var cmdFactory = new JsonFactory<CommandProxyDescription>() { Formatting = Newtonsoft.Json.Formatting.None };
var commands = new Dictionary<int, ShellCommand>();
var tasks = new Dictionary<int, Task>();

stdInEventer.Line += (_, e) =>
{
    if (cmdFactory.TryParse(e.Value, out var desc)
        && desc.Command == "exec")
    {
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

        try
        {
            var cmd = new ShellCommand(
                desc.WorkingDir,
                desc.Args.First(),
                desc.Args.Skip(1).ToArray());

            commands.Add(desc.TaskID, cmd);

            Run(desc, cmd);
        }
        catch(ShellCommandNotFoundException exp)
        {
            Error(desc, exp.Unroll());
        }
    }
};

void Message(CommandProxyDescription? desc, string cmdName, params string[] messageParts)
{
    var response = new CommandProxyDescription(desc, cmdName, messageParts);
    var cmd = cmdFactory.ToString(response);
    WriteLine(cmd);
}

void Send(string cmdName, params string[] messageParts) => 
    Message(null, cmdName, messageParts);

void Info(CommandProxyDescription desc, string message) => 
    Message(desc, "info", message);

void Warn(CommandProxyDescription desc, string message) => 
    Message(desc, "warn", message);

void Error(CommandProxyDescription desc, string message) => 
    Message(desc, "error", message);

async void Run(CommandProxyDescription desc, ShellCommand cmd)
{
    void Cmd_Info(object? sender, StringEventArgs e) => 
        Info(desc, e.Value);

    void Cmd_Warning(object? sender, StringEventArgs e) => 
        Warn(desc, e.Value);

    void Cmd_Err(object? sender, Juniper.ErrorEventArgs e) => 
        Error(desc, e.Value.Unroll());

    cmd.Info += Cmd_Info;
    cmd.Warning += Cmd_Warning;
    cmd.Err += Cmd_Err;
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
}

async Task Shutdown()
{
    Send("Shutting down");
    stdInEventer.Stop();
    foreach (var cmd in commands.Values)
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
    if (host.HasExited)
    {
        await Shutdown();
    }
}

var procs = Process.GetProcessesByName("esbuild");
foreach (var proc in procs)
{
    try
    {
        proc?.Kill(true);
    }
    catch (Exception ex)
    {
        WriteLine(ex.Unroll());
    }
}