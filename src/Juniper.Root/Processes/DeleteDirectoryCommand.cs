namespace Juniper.Processes
{
    public class DeleteDirectoryCommand : AbstractCommand
    {
        private readonly DirectoryInfo dir;

        public DeleteDirectoryCommand(DirectoryInfo dir)
            : base("Delete")
        {
            this.dir = dir;
        }

        public override Task RunAsync(CancellationToken cancellationToken)
        {
            if (dir.Exists)
            {
                dir.Delete(true);
                OnInfo($"Deleted! {dir.FullName}");
            }
            else
            {
                OnInfo($"No directory to delete: {dir.FullName}");
            }

            return Task.CompletedTask;
        }
    }
}
