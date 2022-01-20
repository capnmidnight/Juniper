using Juniper.Logging;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Juniper.Processes
{
    public interface ICommand : ILoggingSource, IDisposable
    {
        string CommandName { get; }
        Task RunAsync(CancellationToken? token = null);
        Task RunSafeAsync(CancellationToken? token = null);
    }
}
