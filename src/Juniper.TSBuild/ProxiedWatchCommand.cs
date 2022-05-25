using Juniper.Processes;

namespace Juniper.TSBuild
{
    public class ProxiedWatchCommand : AbstractShellCommand
    {
        private readonly CommandProxier proxy;
        private readonly string[] pathParts;
        private readonly TaskCompletionSource completer = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public ProxiedWatchCommand(CommandProxier proxy, params string[] pathParts)
            : base("watch " + pathParts.Join(Path.DirectorySeparatorChar))
        {
            this.proxy = proxy;
            this.pathParts = pathParts;
        }

        public override async Task RunAsync()
        {
            proxy.Exec(this, proxy.Root.CD(pathParts), "npm", "run", "watch");
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
