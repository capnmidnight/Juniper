using System.Diagnostics;

using Juniper;
using Juniper.IO;
using Juniper.Processes;

using static System.Console;

if (args.Length != 1)
{
    Console.Error.WriteLine("Expected parent process ID to be passed as an argument");
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
var shuttingDown = new CancellationTokenSource();
var cmdFactory = new JsonFactory<CommandProxyDescription>() { Formatting = Newtonsoft.Json.Formatting.None };
var commands = new Dictionary<int, ShellCommand>();
var tasks = new Dictionary<int, Task>();

stdInEventer.Line += (_, e) =>
{
    if (cmdFactory.TryParse(e.Value, out var desc))
    {
        if (desc.Command == "shutdown")
        {
            shuttingDown.Cancel();
            return;
        }

        if (desc.Command == "exec")
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
                var cmd = MakeCommand(desc);
                commands.Add(desc.TaskID, cmd);
                Run(desc, cmd);
            }
            catch (ShellCommandNotFoundException exp)
            {
                Error(desc, exp.Unroll());
            }
        }
    }
};

void Message(CommandProxyDescription? desc, string cmdName, params string[] messageParts)
{
    var response = new CommandProxyDescription(desc, cmdName, messageParts);
    var cmd = cmdFactory?.ToString(response) ?? "N/A";
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
        var task = cmd.RunAsync(shuttingDown.Token);
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
    await Task.Delay(100);
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
    if (host.HasExited || shuttingDown.IsCancellationRequested)
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

static ShellCommand MakeCommand(CommandProxyDescription desc)
{
    if (desc.Args[0] == "npm")
    {
        var pkg = desc.WorkingDir.Touch("package.json");
        if (pkg.Exists)
        {
            if (desc.Args.Length >= 2 && desc.Args[1] == "install")
            {
                return new NPMInstallCommand(pkg);
            }
            else if (desc.Args.Length >= 3 && desc.Args[1] == "run")
            {
                return new NPMRunCommand(pkg, desc.Args[2]);
            }
        }
    }

    return new ShellCommand(
        desc.WorkingDir,
        desc.Args.First(),
        desc.Args.Skip(1).ToArray());
}