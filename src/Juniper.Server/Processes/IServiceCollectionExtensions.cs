namespace Juniper.Processes
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddCommandTree(this IServiceCollection services, Action<CommandTree> addCommands)
        {
            return services.AddScoped<IScopedShellCommand, ScopedCommand>()
                .AddSingleton<ICommandTree, CommandTree>(_ =>
                {
                    var commandTree = new CommandTree();
                    addCommands(commandTree);
                    return commandTree;
                })
                .AddHostedService<CommandService>();
        }
    }
}
