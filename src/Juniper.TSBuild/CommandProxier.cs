using System.Text.RegularExpressions;

using Juniper.IO;
using Juniper.Logging;

namespace Juniper.Processes;

public partial class CommandProxier : ILoggingSource
{
    private static readonly char[] MAYBE_JSON = {
        '{', '[', '"', '\'', 't', 'f', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
    };

    public DirectoryInfo Root { get; private set; }
    private readonly ShellCommand processManager;
    private readonly TaskCompletionSource processQuit = new();
    private readonly TaskCompletionSource startup = new (TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly JsonFactory<CommandProxyDescription> cmdFactory = new() { Formatting = Newtonsoft.Json.Formatting.None };
    private readonly Dictionary<int, ProxiedCommand> proxies = new();
    private int taskCounter = 0;

    public event EventHandler<StringEventArgs>? Info;
    public event EventHandler<StringEventArgs>? Warning;
    public event EventHandler<ErrorEventArgs>? Err;
    private bool ready;


    [GeneratedRegex(@"Non-zero exit value = -?\d+. Invocation = .+Juniper\.ProcessManager", RegexOptions.ExplicitCapture | RegexOptions.Compiled)]
    private static partial Regex GetPMPattern();
    private static readonly Regex PMPattern = GetPMPattern();

    public CommandProxier(DirectoryInfo rootDir)
    {
        Root = rootDir;

        processManager = new ShellCommand("Juniper.ProcessManager", Environment.ProcessId.ToString());
        processManager.Warning += (_, e) =>
        {
            if (!PMPattern.IsMatch(e.Value))
            {
                Warning?.Invoke(this, e);
            }
        };
        processManager.Err += (_, e) => Err?.Invoke(this, e);
    }

    public async Task Stop()
    {
        var cmd = cmdFactory.ToString(new CommandProxyDescription("shutdown"));
        processManager.Send(cmd);
        await processQuit.Task;
    }

    public async Task Start(CancellationToken cancellationToken)
    {
        cancellationToken.Register(() => startup.TrySetCanceled());
        processManager.Info += OnInfo;
        _ = processManager.RunAsync(CancellationToken.None)
            .ContinueWith(task =>
                processQuit.TrySetResult(), CancellationToken.None);
        await startup.Task;
    }

    private void OnInfo(object? _, StringEventArgs e)
    {
        if (e.Value.Length > 0
            && MAYBE_JSON.Contains(e.Value[0])
            && cmdFactory.TryParse(e.Value, out var cmd)
            && cmd is not null)
        {
            if (proxies.ContainsKey(cmd.TaskID))
            {
                var pcmd = proxies[cmd.TaskID];
                switch (cmd.Command)
                {
                    case "info": pcmd.ProxyInfo(cmd.Args.Join(" ")); break;
                    case "warn": pcmd.ProxyWarning(cmd.Args.Join(" ")); break;
                    case "error": pcmd.ProxyError(new Exception(cmd.Args.Join(Environment.NewLine))); break;
                    case "ended": EndTask(cmd.TaskID); break;
                    default: Warning?.Invoke(this, e); break;
                }
            }
            else if (cmd.Command == "ready")
            {
                if (!ready)
                {
                    ready = true;
                    startup.TrySetResult();
                }
            }
            else if (cmd.Command == "Shutting down")
            {
                processQuit.TrySetResult();
            }
            else
            {
                Warning?.Invoke(this, e);
            }
        }
        else
        {
            Info?.Invoke(this, e);
        }
    }

    private void OnWarning(object? _, StringEventArgs e)
    {
        Warning?.Invoke(this, e);
    }

    private void OnError(object? sender, ErrorEventArgs e)
    {
        Err?.Invoke(this, e);
    }

    private void EndTask(int taskID)
    {
        if (proxies.ContainsKey(taskID))
        {
            var pcmd = proxies[taskID];
            proxies.Remove(taskID);
            pcmd.ProxyEnd();
            pcmd.Err -= OnError;
            pcmd.Warning -= OnWarning;
            pcmd.Info -= OnInfo;
        }
    }

    internal void Exec(ProxiedCommand pcmd, DirectoryInfo? workingDir, params string[] args)
    {
        pcmd.Info += OnInfo;
        pcmd.Warning += OnWarning;
        pcmd.Err += OnError;
        var taskID = ++taskCounter;
        proxies.Add(taskID, pcmd);
        var cmd = cmdFactory.ToString(new CommandProxyDescription(taskID, workingDir, "exec", args));
        processManager.Send(cmd);
    }
}
