namespace Juniper.Processes
{
    public interface IScopedShellCommand
    {
        Task RunAsync(ICommand command, CancellationToken? token);
    }
}
