#nullable enable
using Juniper.Logging;

namespace Juniper.Processes
{
    public class CommandTree : ICommandTree, ILoggingSource
    {
        private readonly List<ICommand[]> commandTree = new();

        public event EventHandler<StringEventArgs>? Info;
        public event EventHandler<StringEventArgs>? Warning;
        public event EventHandler<ErrorEventArgs>? Err;

        public IEnumerable<IEnumerable<ICommand>> Tree => commandTree;

        public ICommandTree AddCommands(params ICommand[] commands)
        {
            commandTree.Add(commands);
            return this;
        }

        public ICommandTree AddCommands(IEnumerable<ICommand> commands)
        {
            return AddCommands(commands.ToArray());
        }

        public async Task ExecuteAsync()
        {
            foreach (var commands in commandTree)
            {
                await ExecuteCommandsAsync(commands);
            }
        }

        private async Task ExecuteCommandsAsync(ICommand[] commands)
        {
            await Task.WhenAll(commands
                .Select(command =>
                    ExecuteCommandAsync(command)));
        }

        private async Task ExecuteCommandAsync(ICommand command)
        {
            command.Info += Tasker_Info;
            command.Warning += Tasker_Warning;
            command.Err += Tasker_Err;
            try
            {
                await command.RunAsync()
                    .ConfigureAwait(false);
            }
            finally
            {
                command.Info -= Tasker_Info;
                command.Warning -= Tasker_Warning;
                command.Err -= Tasker_Err;
            }
        }

        private void Tasker_Info(object? sender, StringEventArgs e)
        {
            Info?.Invoke(sender, e);
        }

        private void Tasker_Warning(object? sender, StringEventArgs e)
        {
            Warning?.Invoke(sender, e);
        }

        private void Tasker_Err(object? sender, ErrorEventArgs e)
        {
            Err?.Invoke(sender, e);
        }
    }
}
