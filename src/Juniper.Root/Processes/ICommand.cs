using Juniper.Logging;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Juniper.Processes
{
    public interface ICommand : ILoggingSource
    {
        string CommandName { get; }
        Task RunAsync();
        Task RunSafeAsync();
    }
}
