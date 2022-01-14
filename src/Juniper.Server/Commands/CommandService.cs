namespace Juniper.Services
{
    public class CommandService : BackgroundService
    {
        private readonly IServiceProvider services;
        private readonly IShellCommandTree commandTree;

        public CommandService(IServiceProvider services, IShellCommandTree commandTree)
        {
            this.services = services;
            this.commandTree = commandTree;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.WhenAll(commandTree.Tree.Select(commands => ExecuteCommandsAsync(commands, stoppingToken)));
        }

        private async Task ExecuteCommandsAsync(IEnumerable<ICommand> commands, CancellationToken stoppingToken)
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
