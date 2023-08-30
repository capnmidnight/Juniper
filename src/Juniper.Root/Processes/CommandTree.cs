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

        public ICommandTree AddMessage(string format, params object[] args)
        {
            return AddCommands(new MessageCommand(format, args));
        }

        public ICommandTree AddCommands(params ICommand[] commands)
        {
            commandTree.Add(commands);
            return this;
        }

        public ICommandTree AddCommands(Action<ICommandTree> addCommands)
        {
            addCommands(this);
            return this;
        }

        public ICommandTree AddCommands(IEnumerable<ICommand> commands)
        {
            return AddCommands(commands.ToArray());
        }

        public async Task ExecuteAsync()
        {
            using var job = new Job("CommandTree");
            
            foreach (var commands in commandTree)
            {
                await ExecuteCommandsAsync(job, commands);
            }
        }

        private async Task ExecuteCommandsAsync(Job job, ICommand[] commands)
        {
            await Task.WhenAll(commands
                .Select(command =>
                    ExecuteCommandAsync(job, command)));
        }

        private async Task ExecuteCommandAsync(Job job, ICommand command)
        {
            command.Info += Tasker_Info;
            command.Warning += Tasker_Warning;
            command.Err += Tasker_Err;
            try
            {
                if(command is ShellCommand shellCommand) {
                    shellCommand.job = job;
                }
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
