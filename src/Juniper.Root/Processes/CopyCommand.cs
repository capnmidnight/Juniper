namespace Juniper.Processes
{
    public class CopyCommand : AbstractCommand
    {
        private readonly FileInfo from;
        private readonly FileInfo to;
        private readonly bool overwrite;
        private DateTime? lastWriteTime;

        public CopyCommand(string name, FileInfo from, FileInfo to, bool overwrite = true)
            : base("Copy " + name)
        {
            this.from = from;
            this.to = to;
            this.overwrite = overwrite;
        }

        public override Task RunAsync()
        {
            Check();
            return Task.CompletedTask;
        }

        private void Check()
        {
            var fromRel = PathExt.Abs2Rel(from.FullName, Environment.CurrentDirectory);
            if (!from.Exists)
            {
                OnWarning($"File does not exist! {fromRel}");
            }
            else
            {
                to.Directory?.Create();
                File.Copy(from.FullName, to.FullName, overwrite);
                lastWriteTime = from.LastWriteTime;
                var toRel = PathExt.Abs2Rel(to.FullName, Environment.CurrentDirectory);
                OnInfo($"Copied! {fromRel} -> {toRel}");
            }
        }

        public bool Recheck()
        {
            from.Refresh();
            if (from.Exists
                && (lastWriteTime is null
                    || from.LastWriteTime > lastWriteTime))
            {
                Check();
                return true;
            }

            return false;
        }
    }
}
