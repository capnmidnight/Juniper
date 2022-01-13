using Juniper.Processes;

namespace Juniper.Services
{
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
        ShellCommand Command { get; set; }

        Task RunAsync(CancellationToken? token);
    }

    public class ScopedShellCommand : IScopedShellCommand
    {
        private readonly ILogger<ScopedShellCommand> logger;
        public ShellCommand Command { get; set; }

        public ScopedShellCommand(ILogger<ScopedShellCommand> logger)
        {
            this.logger = logger;
        }

        private void Command_Info(object sender, StringEventArgs e)
        {
            logger.LogInformation("({LastCommand}): {Message}", Command.CommandName, e.Value);
        }

        private void Command_Warning(object sender, StringEventArgs e)
        {
            logger.LogWarning("({LastCommand}): {Message}", Command.CommandName, e.Value);
        }

        public async Task RunAsync(CancellationToken? token)
        {
            Command.Info += Command_Info;
            Command.Warning += Command_Warning;
            try
            {
                var result = await Command.RunAsync(token);
                if(result != 0)
                {
                    throw new Exception($"Non-zero exit value = {result}");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "({LastCommand}): {Message}", Command.CommandName, ex.Message);
            }
            finally
            {
                Command.Info -= Command_Info;
                Command.Warning -= Command_Warning;
            }
        }
    }

    public interface IShellCommandTree
    {
        IEnumerable<IEnumerable<ShellCommand>> CommandTree { get; }
        IShellCommandTree AddCommands(params ShellCommand[] commands);
    }

    public class ShellCommandTree : IShellCommandTree, IDisposable
    {
        private readonly List<ShellCommand[]> commandTree = new();
        private bool disposedValue;

        public IEnumerable<IEnumerable<ShellCommand>> CommandTree => commandTree;

        public IShellCommandTree AddCommands(params ShellCommand[] commands)
        {
            commandTree.Add(commands);
            return this;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    foreach (var commands in commandTree)
                    {
                        foreach (var command in commands)
                        {
                            command.Dispose();
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
                scopedCommand.Command = command;
                await scopedCommand.RunAsync(stoppingToken);
            }
        }
    }
}
