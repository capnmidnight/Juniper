namespace Juniper.Processes
{
    public class CommandService : BackgroundService
    {
        private readonly IServiceProvider services;
        private readonly ICommandTree commandTree;

        public CommandService(IServiceProvider services, ICommandTree commandTree)
        {
            this.services = services;
            this.commandTree = commandTree;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            foreach (var commands in commandTree.Tree)
            {
                await ExecuteCommandsAsync(commands);
            }
        }

        private async Task ExecuteCommandsAsync(IEnumerable<ICommand> commands)
        {
            await Task.WhenAll(commands.Select(ExecuteCommandAsync));
        }

        private async Task ExecuteCommandAsync(ICommand command)
        {
            using var scope = services.CreateScope();
            var scopedCommand = scope.ServiceProvider.GetRequiredService<IScopedShellCommand>();
            await scopedCommand.RunAsync(command);
        }
    }
}
