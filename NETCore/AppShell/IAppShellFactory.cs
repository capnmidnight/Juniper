namespace Juniper.AppShell;

public interface IAppShellFactory
{
    Task<IAppShell> StartAsync(CancellationToken cancellationToken);
}