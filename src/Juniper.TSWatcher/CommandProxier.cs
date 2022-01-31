using Juniper.IO;
using Juniper.Logging;
using Juniper.Processes;

namespace Juniper.TSWatcher
{
    public class CommandProxier : ILoggingSource
    {
        public DirectoryInfo Root { get; private set; }
        private readonly ShellCommand processManager;
        private readonly JsonFactory<CommandProxyDescription> cmdFactory = new() { Formatting = Newtonsoft.Json.Formatting.None };
        private readonly Dictionary<int, ProxiedWatchCommand> proxies = new();
        private readonly Task startup;
        private int taskCounter = 0;

        public event EventHandler<StringEventArgs>? Info;
        public event EventHandler<StringEventArgs>? Warning;
        public event EventHandler<ErrorEventArgs>? Err;

        public CommandProxier(DirectoryInfo? rootDir)
        {
            if(rootDir is null)
            {
                throw new ArgumentNullException(nameof(rootDir));
            }

            Root = rootDir;


            var starter = new TaskCompletionSource();
            startup = starter.Task;

            processManager = new ShellCommand("Juniper.ProcessManager", Environment.ProcessId.ToString());
            processManager.Info += (_, e) =>
            {
                if (cmdFactory.TryParse(e.Value, out var cmd))
                {
                    if (proxies.ContainsKey(cmd.TaskID))
                    {
                        var pcmd = proxies[cmd.TaskID];
                        switch (cmd.Command)
                        {
                            case "info": pcmd.ProxyInfo(cmd.Args.Join(" ")); break;
                            case "warning": pcmd.ProxyWarning(cmd.Args.Join(" ")); break;
                            case "error": pcmd.ProxyError(new Exception(cmd.Args.Join(Environment.NewLine))); break;
                            case "ended": EndTask(cmd.TaskID); break;
                        }
                    }
                    else if (cmd.Command == "ready")
                    {
                        starter.SetResult();
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
            };

            processManager.Warning += (_, e) => Warning?.Invoke(this, e);
            processManager.Err += (_, e) => Err?.Invoke(this, e);
            _ = processManager.RunAsync();
        }

        private void EndTask(int taskID)
        {
            if (proxies.ContainsKey(taskID))
            {
                var pcmd = proxies[taskID];
                pcmd.ProxyEnd();
                proxies.Remove(taskID);
            }
        }

        internal async Task Watch(ProxiedWatchCommand pcmd, params string[] pathParts)
        {
            var taskID = ++taskCounter;
            proxies.Add(taskID, pcmd);
            await startup;
            var cmd = cmdFactory.ToString(new CommandProxyDescription(taskID, Root.CD(pathParts), "exec", "npm", "run", "watch"));
            processManager.Send(cmd);
        }
    }
}
