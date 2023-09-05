namespace Juniper.AppShell;

public interface IAppShellService
{
    Task StartAsync(string splashPage);
    Task<IAppShell> RunAsync();
}
