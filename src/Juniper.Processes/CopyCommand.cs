namespace Juniper.Processes
{
    public class CopyCommand : AbstractCommand
    {
        private readonly FileInfo from;
        private readonly FileInfo to;
        private readonly bool overwrite;

        public CopyCommand(FileInfo from, FileInfo to, bool overwrite = true)
            : base("Copy")
        {
            this.from = from;
            this.to = to;
            this.overwrite = overwrite;
        }

        public override Task RunAsync()
        {
            to.Directory?.Create();
            File.Copy(from.FullName, to.FullName, overwrite);
            var fromRel = PathExt.Abs2Rel(from.FullName, Environment.CurrentDirectory);
            var toRel = PathExt.Abs2Rel(to.FullName, Environment.CurrentDirectory);
            OnInfo($"Copied! {fromRel} -> {toRel}");
            return Task.CompletedTask;
        }
    }
}
