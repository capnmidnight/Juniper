namespace Juniper.AppShell;

using Gtk;
using WebKit;

public class GtkAppShell : FixedWindow, IAppShell
{
    private readonly FixedWebView webView;
    private readonly TaskCompletionSource deletedTask = new();

    public GtkAppShell()
    : base("Juniper AppShell")
    {
        SetSizeRequest(800, 600);
        Add(webView = new FixedWebView());
    }

    protected override bool OnDeleteEvent(Gdk.Event evnt)
    {
        Remove(webView);
        webView.Dispose();
        deletedTask.SetResult();
        return base.OnDeleteEvent(evnt);
    }

    private static Task<T> Do<T>(Func<T> action)
    {
        var completer = new TaskCompletionSource<T>();
        Application.Invoke(delegate
        {
            completer.SetResult(action());
        });
        return completer.Task;
    }

    private static async Task<T> Do<T>(Func<Task<T>> action)
    {
        var completer = new TaskCompletionSource<Task<T>>();
        Application.Invoke(delegate
        {
            completer.SetResult(action());
        });
        var task = await completer.Task;
        return await task;
    }

    private static async Task Do(Func<Task> action)
    {
        var completer = new TaskCompletionSource<Task>();
        Application.Invoke(delegate
        {
            completer.SetResult(action());
        });
        var task = await completer.Task;
        await task;
    }

    private static Task Do(System.Action action)
    {
        var completer = new TaskCompletionSource();
        Application.Invoke(delegate
        {
            action();
            completer.SetResult();
        });
        return completer.Task;
    }

    /////////////////
    //// CLOSING ////
    /////////////////

    public Task ShowAsync() =>
        Do(Show);

    public Task HideAsync() =>
        Do(Hide);

    public Task CloseAsync() =>
        Do(Close);

    public Task WaitForCloseAsync() =>
        deletedTask.Task;

    ////////////////
    //// SOURCE ////
    ////////////////

    public Task<Uri> GetSourceAsync() =>
        Do(() => new Uri(webView.Uri));

    public Task SetSourceAsync(Uri source) =>
        Do(() => webView.LoadUriAsync(source.ToString()));

    public Task ReloadAsync() =>
        Do(() => webView.Reload());

    ///////////////
    //// TITLE ////
    ///////////////

    public Task<string> GetTitleAsync() =>
        Do(() => Title);

    public Task SetTitleAsync(string title) =>
        Do(() => Title = title);

    /////////////////
    //// HISTORY ////
    /////////////////

    public Task<bool> GetCanGoBackAsync() =>
        Do(webView.CanGoBack);

    public Task<bool> GetCanGoForwardAsync() =>
        Do(webView.CanGoForward);

    //////////////
    //// SIZE ////
    //////////////

    public Task<Size> GetSizeAsync() =>
        Do(() =>
        {
            GetSize(out var width, out var height);
            return new Size(width, height);
        });

    public Task SetSizeAsync(int width, int height) =>
        Do(() => Resize(width, height));

    ////////////////////
    //// Fullscreen ////
    ////////////////////

    public Task<bool> GetIsFullscreenAsync() =>
        Do(() => IsFullscreen);

    public Task SetIsFullscreenAsync(bool isFullscreen) =>
        Do(() => IsFullscreen = isFullscreen);

    ////////////////////
    //// BORDERLESS ////
    ////////////////////

    public Task<bool> GetIsBorderlessAsync() =>
        Do(() => Decorated);

    public Task SetIsBorderlessAsync(bool isBorderless) =>
        Do(() => Decorated = !isBorderless);

    ///////////////////
    //// MAXIMIZED ////
    ///////////////////

    public Task<bool> GetIsMaximizedAsync() =>
        Do(() => IsMaximized);

    public Task SetIsMaximizedAsync(bool isMaximized) =>
        Do(() => IsMaximized = isMaximized);

    ///////////////////
    //// MINIMIZED ////
    ///////////////////

    public Task<bool> GetIsMinimizedAsync() =>
        Do(() => IsIconified);

    public Task SetIsMinimizedAsync(bool isMinimized) =>
        Do(() => IsIconified = isMinimized);
}
