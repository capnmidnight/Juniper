namespace Juniper.AppShell;

public interface IAppShellFactory : IAppShellService
{
    Task<IAppShell> StartAsync(CancellationToken cancellationToken);
}