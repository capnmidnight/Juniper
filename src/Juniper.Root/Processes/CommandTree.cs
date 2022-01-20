using Juniper.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Juniper.Processes
{
    public class CommandTree : ICommandTree, ILoggingSource
    {
        private readonly List<ICommand[]> commandTree = new();
        private bool disposedValue;

        public event EventHandler<StringEventArgs> Info;
        public event EventHandler<StringEventArgs> Warning;
        public event EventHandler<ErrorEventArgs> Err;

        public IEnumerable<IEnumerable<ICommand>> Tree => commandTree;

        public ICommandTree AddCommands(params ICommand[] commands)
        {
            commandTree.Add(commands);
            return this;
        }

        public ICommandTree AddCommands(IEnumerable<ICommand> commands)
        {
            commandTree.Add(commands.ToArray());
            return this;
        }

        public async Task ExecuteAsync(CancellationToken? stoppingToken = null)
        {
            foreach (var commands in commandTree)
            {
                await ExecuteCommandsAsync(commands, stoppingToken);
            }
        }

        private async Task ExecuteCommandsAsync(ICommand[] commands, CancellationToken? stoppingToken)
        {
            await Task.WhenAll(commands
                .Select(command =>
                    ExecuteCommandAsync(command, stoppingToken)));
        }

        private async Task ExecuteCommandAsync(ICommand command, CancellationToken? stoppingToken)
        {
            command.Info += Tasker_Info;
            command.Warning += Tasker_Warning;
            command.Err += Tasker_Err;
            try
            {
                await command.RunAsync(stoppingToken)
                    .ConfigureAwait(false);
            }
            finally
            {
                command.Info -= Tasker_Info;
                command.Warning -= Tasker_Warning;
                command.Err -= Tasker_Err;
            }
        }

        private void Tasker_Info(object sender, StringEventArgs e)
        {
            Info?.Invoke(sender, e);
        }

        private void Tasker_Warning(object sender, StringEventArgs e)
        {
            Warning?.Invoke(sender, e);
        }

        private void Tasker_Err(object sender, ErrorEventArgs e)
        {
            Err?.Invoke(sender, e);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    foreach (var commands in Tree)
                    {
                        foreach (var command in commands)
                        {
                            try
                            {
                                command.Dispose();
                            }
                            catch { }
                        }
                    }
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
