using Juniper.Processes;

namespace Juniper.Services
{
    public class ShellCommand
    {
        private readonly string command;
        private readonly string[] args;

        public ShellCommand(string command, params string[] args)
        {
            if (command is null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            if (command.Length == 0)
            {
                throw new InvalidOperationException($"{nameof(command)} cannot be an empty string.");
            }

            this.command = command;
            this.args = args ?? Array.Empty<string>();
        }

        internal ProcessTasker CreateTask()
        {
            return new ProcessTasker(command, args);
        }
    }

    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddShellCommands(this IServiceCollection services, Action<ShellCommandTree> addCommands)
        {
            return services.AddScoped<IScopedShellCommand, ScopedShellCommand>()
                .AddSingleton<IShellCommandTree, ShellCommandTree>(_ =>
                {
                    var commandTree = new ShellCommandTree();
                    addCommands(commandTree);
                    return commandTree;
                })
                .AddHostedService<ShellCommandService>();
        }
    }

    public interface IScopedShellCommand
    {
        Task RunAsync(ShellCommand command, CancellationToken? token);
    }

    public class ScopedShellCommand : IScopedShellCommand
    {
        private readonly ILogger<ScopedShellCommand> logger;

        public ScopedShellCommand(ILogger<ScopedShellCommand> logger)
        {
            this.logger = logger;
        }

        private void Command_Info(object sender, StringEventArgs e)
        {
            if (sender is ProcessTasker proc)
            {
                logger.LogInformation("({LastCommand}): {Message}", proc.CommandName, e.Value);
            }
        }

        private void Command_Warning(object sender, StringEventArgs e)
        {
            if (sender is ProcessTasker proc)
            {
                logger.LogWarning("({LastCommand}): {Message}", proc.CommandName, e.Value);
            }
        }

        public async Task RunAsync(ShellCommand command, CancellationToken? token)
        {
            using var task = command.CreateTask();
            task.Info += Command_Info;
            task.Warning += Command_Warning;
            try
            {
                var result = await task.RunAsync(token);
                if(result != 0)
                {
                    throw new Exception($"Non-zero exit value = {result}");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "({LastCommand}): {Message}", task.CommandName, ex.Message);
            }
            finally
            {
                task.Info -= Command_Info;
                task.Warning -= Command_Warning;
            }
        }
    }

    public interface IShellCommandTree
    {
        IEnumerable<IEnumerable<ShellCommand>> CommandTree { get; }
        IShellCommandTree AddCommands(params ShellCommand[] commands);
    }

    public class ShellCommandTree : IShellCommandTree
    {
        private readonly List<ShellCommand[]> commandTree = new();

        public IEnumerable<IEnumerable<ShellCommand>> CommandTree => commandTree;

        public IShellCommandTree AddCommands(params ShellCommand[] commands)
        {
            commandTree.Add(commands);
            return this;
        }
    }

    public class ShellCommandService : BackgroundService
    {
        private readonly IServiceProvider services;
        private readonly IShellCommandTree commandTree;

        public ShellCommandService(IServiceProvider services, IShellCommandTree commandTree)
        {
            this.services = services;
            this.commandTree = commandTree;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.WhenAll(commandTree.CommandTree.Select(commands => ExecuteCommandsAsync(commands, stoppingToken)));
        }

        private async Task ExecuteCommandsAsync(IEnumerable<ShellCommand> commands, CancellationToken stoppingToken)
        {
            foreach (var command in commands)
            {
                using var scope = services.CreateScope();
                var scopedCommand = scope.ServiceProvider.GetRequiredService<IScopedShellCommand>();
                await scopedCommand.RunAsync(command, stoppingToken);
            }
        }
    }
}
