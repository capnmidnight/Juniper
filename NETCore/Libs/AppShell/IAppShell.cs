namespace Juniper.AppShell;

public interface IBaseAppShell
{
    Task ShowAsync();
    Task HideAsync();
    Task CloseAsync();

    Task<string> GetTitleAsync();
    Task SetTitleAsync(string title);

    Task<bool> GetCanGoBackAsync();
    Task<bool> GetCanGoForwardAsync();

    Task<Size> GetSizeAsync();
    Task SetSizeAsync(int width, int height);

    Task<bool> GetIsFullscreenAsync();
    Task SetIsFullscreenAsync(bool isFullscreen);

    Task<bool> GetIsBorderlessAsync();
    Task SetIsBorderlessAsync(bool isBorderless);

    Task<bool> GetIsMaximizedAsync();
    Task SetIsMaximizedAsync(bool IsMaximized);

    Task<bool> GetIsMinimizedAsync();
    Task SetIsMinimizedAsync(bool isMinimized);

}

public interface IAppShell : IBaseAppShell
{
    Task WaitForCloseAsync();

    Task<Uri> GetSourceUriAsync();
    Task SetSourceUriAsync(Uri source);
    Task SetSourceHTMLAsync(string html);

    Task ReloadAsync();
}

public static class IAppShellExt
{
    public static Task MaximizeAsync(this IAppShell appShell)
    {
        return appShell.SetIsMaximizedAsync(true);
    }

    public static Task MinimizeAsync(this IAppShell appShell)
    {
        return appShell.SetIsMinimizedAsync(true);
    }

    public static async Task<bool> ToggleExpandedAsync(this IAppShell appShell)
    {
        var isMaximized = !await appShell.GetIsMaximizedAsync();
        await appShell.SetIsMaximizedAsync(isMaximized);
        return isMaximized;
    }
}