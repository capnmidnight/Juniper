using System;
using System.Collections.Generic;

namespace Juniper.Processes
{
    public interface ICommandTree : IDisposable
    {
        IEnumerable<IEnumerable<ICommand>> Tree { get; }
        ICommandTree AddCommands(params ICommand[] commands);
        ICommandTree AddCommands(IEnumerable<ICommand> commands);
    }
}
