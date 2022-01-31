namespace Juniper.Processes
{
    public class CopyCommand : AbstractCommand
    {
        private readonly string from;
        private readonly string to;
        private readonly bool overwrite;

        public CopyCommand(string from, string to, bool overwrite = true)
            : base("Copy")
        {
            this.from = from;
            this.to = to;
            this.overwrite = overwrite;
        }

        public override Task RunAsync()
        {
            var outFile = new FileInfo(to);
            outFile.Directory?.Create();
            File.Copy(from, to, overwrite);
            var fromRel = PathExt.Abs2Rel(from, Environment.CurrentDirectory);
            var toRel = PathExt.Abs2Rel(to, Environment.CurrentDirectory);
            OnInfo($"Copied! {fromRel} -> {toRel}");
            return Task.CompletedTask;
        }
    }
}
