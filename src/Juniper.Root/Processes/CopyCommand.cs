namespace Juniper.Processes
{
    public class CopyCommand : AbstractCommand
    {
        private readonly FileInfo from;
        private readonly FileInfo to;
        private readonly bool warnIfNotExists;
        private readonly (string, string)? replacement;
        private DateTime? lastWriteTime;

        public CopyCommand(string name, FileInfo from, FileInfo to, bool warnIfNotExists)
            : base("Copy " + name)
        {
            this.from = from;
            this.to = to;
            this.warnIfNotExists = warnIfNotExists;
        }

        public CopyCommand(string name, FileInfo from, FileInfo to, bool warnIfNotExists, (string, string) replacement)
            : this(name, from, to, warnIfNotExists)
        {
            this.replacement = replacement;
        }

        public override Task RunAsync(CancellationToken cancellationToken)
        {
            Check();
            return Task.CompletedTask;
        }

        private void Check()
        {
            var fromRel = PathExt.Abs2Rel(from.FullName, Environment.CurrentDirectory);
            var toRel = PathExt.Abs2Rel(to.FullName, Environment.CurrentDirectory);
            if (!from.Exists)
            {
                if (warnIfNotExists)
                {
                    OnWarning($"File does not exist! {fromRel}");
                }
            }
            else if (to.Exists && to.LastWriteTime >= from.LastWriteTime)
            {
                lastWriteTime = from.LastWriteTime;
                //OnInfo($"Up to date: {toRel}");
            }
            else
            {
                to.Directory?.Create();
                if (replacement is null)
                {
                    File.Copy(from.FullName, to.FullName, true);
                }
                else
                {
                    var (oldValue, newValue) = replacement.Value;
                    var lines = File.ReadAllLines(from.FullName);
                    if (lines.Length > 0)
                    {
                        var index = lines.Length - 1;
                        while (index > 0 && lines[index].Length == 0)
                        {
                            --index;
                        }
                        if (index > 0)
                        {
                            OnInfo($"Replacing {oldValue} with {newValue} in '{lines[index]}'");
                            lines[index] = lines[index].Replace(oldValue, newValue);
                        }
                    }
                    File.WriteAllLines(to.FullName, lines);
                }
                lastWriteTime = from.LastWriteTime;
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
