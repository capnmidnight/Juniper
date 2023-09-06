namespace Juniper.AppShell;

public interface IAppShellService
{
    Task StartAppShellAsync(string title, string splashPage);
    Task RunAppShellAsync();
}
