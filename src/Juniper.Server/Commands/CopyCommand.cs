using Juniper.Processes;

namespace Juniper.Services
{
    public class CopyCommand : AbstractTasker, ICommand
    {
        private readonly string from;
        private readonly string to;
        private readonly bool overwrite;

        public CopyCommand(string from, string to, bool overwrite = true)
        {
            this.from = from;
            this.to = to;
            this.overwrite = overwrite;
            CommandName = $"copy {from} to {to}";
        }

        public ITasker CreateTask()
        {
            return this;
        }

        public override Task RunAsync(CancellationToken? token = null)
        {
            var outFile = new FileInfo(to);
            outFile.Directory.Create();
            File.Copy(from, to, overwrite);
            OnInfo("Copied!");
            return Task.CompletedTask;
        }
    }
}
