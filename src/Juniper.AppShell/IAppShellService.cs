namespace Juniper.AppShell;

public interface IAppShellService
{
    Task StartAppShellAsync(string title, string splashPage, string iconPath = null);
    Task RunAppShellAsync();
}
