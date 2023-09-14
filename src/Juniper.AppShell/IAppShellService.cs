namespace Juniper.AppShell;

public interface IAppShellService
{
    Task StartAppShellAsync(string title, string splashPage, string iconPath);
    Task RunAppShellAsync();
}
