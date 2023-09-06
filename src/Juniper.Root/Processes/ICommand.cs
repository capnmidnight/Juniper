using Juniper.Logging;

namespace Juniper.Processes
{
    public interface ICommand : ILoggingSource
    {
        string CommandName { get; }
        Task RunAsync(CancellationToken cancellationToken);
        Task RunSafeAsync(CancellationToken cancellationToken);
    }
}
