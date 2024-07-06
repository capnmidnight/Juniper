using Juniper.Logging;

namespace Juniper.Processes;

public class CommandTree : ILoggingSource
{
    private readonly List<ICommand[]> commandTree = new();

    public event EventHandler<StringEventArgs>? Info;
    public event EventHandler<StringEventArgs>? Warning;
    public event EventHandler<ErrorEventArgs>? Err;

    public IEnumerable<IEnumerable<ICommand>> Tree => commandTree;

    public CommandTree  AddMessage(string format, params object[] args)
    {
        return AddCommands(new MessageCommand(format, args));
    }

    public CommandTree  AddCommands(params ICommand[] commands)
    {
        if (commands.Length > 0)
        {
            commandTree.Add(commands);
        }
        return this;
    }

    public CommandTree  AddCommands(Action<CommandTree > addCommands)
    {
        addCommands(this);
        return this;
    }

    public CommandTree  AddCommands(IEnumerable<ICommand> commands)
    {
        return AddCommands(commands.ToArray());
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var job = new Job("CommandTree");
        
        foreach (var commands in commandTree)
        {
            await ExecuteCommandsAsync(job, commands, cancellationToken);
        }
    }

    private async Task ExecuteCommandsAsync(Job job, ICommand[] commands, CancellationToken cancellationToken)
    {
        await Task.WhenAll(commands
            .Select(command =>
                ExecuteCommandAsync(job, command, cancellationToken)));
    }

    private async Task ExecuteCommandAsync(Job job, ICommand command, CancellationToken cancellationToken)
    {
        command.Info += Tasker_Info;
        command.Warning += Tasker_Warning;
        command.Err += Tasker_Err;
        try
        {
            if(command is ShellCommand shellCommand) {
                shellCommand.job = job;
            }
            await command.RunAsync(cancellationToken)
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
