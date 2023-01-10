using Juniper.Processes;

namespace Juniper.TSBuild
{
    public class ProxiedCommand : AbstractShellCommand
    {
        private readonly CommandProxier proxy;
        private readonly DirectoryInfo workingDir;
        private readonly string[] args;
        private readonly TaskCompletionSource completer = new(TaskCreationOptions.RunContinuationsAsynchronously);

        private static string MakeCommandName(DirectoryInfo? workingDir, ref string command, ref string[] args)
        {
            if (workingDir is null)
            {
                throw new ShellCommandNotFoundException("No working directory provided.");
            }

            if (string.IsNullOrEmpty(command))
            {
                throw new ShellCommandNotFoundException("No command provided.");
            }

            args ??= Array.Empty<string>();

            var commandName = args.Prepend(command).ToArray().Join(' ');
            if (workingDir is not null)
            {
                commandName = $"({workingDir.Name}) {commandName}";
            }

            return commandName;
        }

        public ProxiedCommand(CommandProxier proxy, DirectoryInfo workingDir, string command, params string[] args)
            : base(MakeCommandName(workingDir, ref command, ref args))
        {
            this.proxy = proxy;
            this.workingDir = workingDir;
            this.args = args.Prepend(command).ToArray();
        }

        public override async Task RunAsync()
        {
            proxy.Exec(this, workingDir, args);
            await completer.Task;
        }

        internal void ProxyInfo(string message)
        {
            OnInfo(message);
        }

        internal void ProxyWarning(string message)
        {
            OnWarning(message);
        }

        internal void ProxyError(Exception exp)
        {
            OnError(exp);
        }

        internal void ProxyEnd()
        {
            completer.SetResult();
        }
    }
}
