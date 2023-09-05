namespace Juniper.AppShell;

public interface IAppShellFactory
{
    Task<IAppShell> StartAsync();
}