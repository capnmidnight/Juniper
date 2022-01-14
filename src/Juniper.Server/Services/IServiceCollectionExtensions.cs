namespace Juniper.Services
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddShellCommands(this IServiceCollection services, Action<CommandTree> addCommands)
        {
            return services.AddScoped<IScopedShellCommand, ScopedCommand>()
                .AddSingleton<IShellCommandTree, CommandTree>(_ =>
                {
                    var commandTree = new CommandTree();
                    addCommands(commandTree);
                    return commandTree;
                })
                .AddHostedService<CommandService>();
        }
    }
}
