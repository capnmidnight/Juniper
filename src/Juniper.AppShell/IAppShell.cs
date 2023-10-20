namespace Juniper.AppShell;

public interface IAppShell
{
    Task<Uri> GetSourceAsync();
    Task SetSourceAsync(Uri source);

    Task<string> GetTitleAsync();
    Task SetTitleAsync(string title);

    Task<bool> GetCanGoBackAsync();
    Task<bool> GetCanGoForwardAsync();

    Task SetSizeAsync(int width, int height);
    Task MaximizeAsync();
    Task MinimizeAsync();
    Task<bool> ToggleExpandedAsync();

    Task CloseAsync();
    Task WaitForCloseAsync();
}