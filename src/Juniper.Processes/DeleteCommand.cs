namespace Juniper.Processes
{
    public class DeleteCommand : AbstractCommand
    {
        private readonly FileInfo file;

        public DeleteCommand(FileInfo file)
            : base("Delete")
        {
            this.file = file;
        }

        public override Task RunAsync()
        {
            if (file.Exists)
            {
                file.Delete();
                OnInfo($"Deleted! {file.FullName}");
            }
            else
            {
                OnInfo($"No file to delete: {file.FullName}");
            }

            return Task.CompletedTask;
        }
    }
}
