using Juniper.IO;
using Juniper.Logging;
using Juniper.Processes;

namespace Juniper.TSBuild
{
    public class CommandProxier : ILoggingSource
    {
        public DirectoryInfo Root { get; private set; }
        private readonly ShellCommand processManager;
        private readonly JsonFactory<CommandProxyDescription> cmdFactory = new() { Formatting = Newtonsoft.Json.Formatting.None };
        private readonly Dictionary<int, ProxiedCommand> proxies = new();
        private int taskCounter = 0;

        public event EventHandler<StringEventArgs>? Info;
        public event EventHandler<StringEventArgs>? Warning;
        public event EventHandler<ErrorEventArgs>? Err;

        private bool ready;

        public CommandProxier(DirectoryInfo rootDir)
        {
            Root = rootDir;

            processManager = new ShellCommand("Juniper.ProcessManager", Environment.ProcessId.ToString());
            processManager.Warning += (_, e) => Warning?.Invoke(this, e);
            processManager.Err += (_, e) => Err?.Invoke(this, e);
        }

        public async Task Start()
        {
            var startup = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            void onInfo(object? _, StringEventArgs e)
            {
                if (cmdFactory.TryParse(e.Value, out var cmd))
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
                            startup.SetResult();
                        }
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
            processManager.Info += onInfo;
            _ = processManager.RunAsync();
            await startup.Task;
        }

        private void EndTask(int taskID)
        {
            if (proxies.ContainsKey(taskID))
            {
                var pcmd = proxies[taskID];
                proxies.Remove(taskID);
                pcmd.ProxyEnd();
            }
        }

        internal void Exec(ProxiedCommand pcmd, DirectoryInfo? workingDir, params string[] args)
        {
            var taskID = ++taskCounter;
            proxies.Add(taskID, pcmd);
            var cmd = cmdFactory.ToString(new CommandProxyDescription(taskID, workingDir, "exec", args));
            processManager.Send(cmd);
        }
    }
}
