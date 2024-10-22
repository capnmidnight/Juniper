using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace Juniper.AppShell;

public partial class WpfAppShell : Window, IAppShell
{
    private readonly Task init;

    public WpfAppShell()
    {
        // WebView2 encounters a directory access permission error when running
        // in VS Code. Overriding its USER_DATA_FOLDER environment variable
        // apparently fixes it.
        string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AppName");
        Environment.SetEnvironmentVariable("WEBVIEW2_USER_DATA_FOLDER", path);

        InitializeComponent();

        WebView.ZoomFactorChanged += delegate
        {
            // This event will fire whenever the user changes zoom factor (e.g.,
            // by typing CTRL+PLUS/CTRL+MINUS, or doing CTRL+MOUSE_WHEEL).
            // But WebView2 doesn't persist user-selected zoom factor between
            // page loads. So we read the current (user-selected) zoom factor
            // and set it to be (program-selected) zoom factor and now it will
            // persist between page loads.
            WebView.ZoomFactor = WebView.ZoomFactor;
        };

        init = WebView.EnsureCoreWebView2Async();
    }

#pragma warning disable IDE0051 // Remove unused private members
    // This might become useful later, in which case, remove the `#pragma`s
    private async Task<T> Do<T>(Func<Task<T>> action)
#pragma warning restore IDE0051 // Remove unused private members
    {
        await init;
        var task = await Dispatcher.InvokeAsync(action).Task;
        return await task;
    }

    private async Task Do(Func<Task> action)
    {
        await init;
        var task = await Dispatcher.InvokeAsync(action);
        await task;
    }

    private async Task<T> Do<T>(Func<T> action)
    {
        await init;
        return await Dispatcher.InvokeAsync(action).Task;
    }

    private async Task Do(Action action)
    {
        await init;
        await Dispatcher.InvokeAsync(action).Task;
    }

    /////////////////
    //// CLOSING ////
    /////////////////

    public Task ShowAsync() =>
        Do(Show);

    public Task HideAsync() =>
        Do(Hide);

    private readonly TaskCompletionSource closing = new();

    public Task CloseAsync()
    {
        try
        {
            Dispatcher.Invoke(Close);
        }
        catch (TaskCanceledException)
        {
            // do nothing
        }
        closing.TrySetResult();
        return Task.CompletedTask;
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        base.OnClosing(e);
        closing.TrySetResult();
    }

    public Task WaitForCloseAsync() =>
        closing.Task;

    ////////////////
    //// SOURCE ////
    ////////////////

    public Task<Uri> GetSourceUriAsync() =>
        Do(() => WebView.Source);

    public Task SetSourceUriAsync(Uri value) =>
        Do(() => LoadAsync(SetSourceInternal, value));

    private void SetSourceInternal(Uri value)
    {
        if (value.ToString() == WebView.Source.ToString())
        {
            WebView.Reload();
        }
        else
        {
            WebView.Source = value;
        }
    }

    public Task SetSourceHTMLAsync(string html) =>
        Do(() => LoadAsync(WebView.NavigateToString, html));

    private async Task LoadAsync<T>(Action<T> act, T value)
    {
        var completer = new TaskCompletionSource();
        void CoreWebView2_DOMContentLoaded(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2DOMContentLoadedEventArgs e)
        {
            completer.SetResult();
        }

        WebView.CoreWebView2.DOMContentLoaded += CoreWebView2_DOMContentLoaded;

        act(value);

        await completer.Task;
        WebView.CoreWebView2.DOMContentLoaded -= CoreWebView2_DOMContentLoaded;
    }

    public Task ReloadAsync() =>
        Do(WebView.Reload);

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
        Do(() => WebView.CanGoBack);

    public Task<bool> GetCanGoForwardAsync() =>
        Do(() => WebView.CanGoForward);

    //////////////
    //// SIZE ////
    //////////////

    public Task<Juniper.Size> GetSizeAsync() =>
        Do(() => new Juniper.Size((int)Width, (int)Height));

    public Task SetSizeAsync(int width, int height) =>
        Do(() =>
        {
            Width = width;
            Height = height;
        });

    ////////////////////
    //// FULLSCREEN ////
    ////////////////////

    public Task<bool> GetIsFullscreenAsync() =>
        Do(() => Topmost);

    public Task SetIsFullscreenAsync(bool isFullscreen) =>
        Do(() => Topmost = isFullscreen);

    ////////////////////
    //// BORDERLESS ////
    ////////////////////

    public Task<bool> GetIsBorderlessAsync() =>
        Do(() => WindowStyle == WindowStyle.None);

    public Task SetIsBorderlessAsync(bool isBorderless) =>
        Do(() => WindowStyle = isBorderless
            ? WindowStyle.None
            : WindowStyle.SingleBorderWindow);

    ///////////////////
    //// MAXIMIZED ////
    ///////////////////

    public Task<bool> GetIsMaximizedAsync() =>
        Do(() => WindowState == WindowState.Maximized);

    public Task SetIsMaximizedAsync(bool isMaximized) =>
        Do(() => WindowState = isMaximized
            ? WindowState.Maximized
            : WindowState.Normal);

    ///////////////////
    //// MINIMIZED ////
    ///////////////////

    public Task<bool> GetIsMinimizedAsync() =>
        Do(() => WindowState == WindowState.Minimized);

    public Task SetIsMinimizedAsync(bool isMinimized) =>
        Do(() => WindowState = isMinimized
            ? WindowState.Minimized
            : WindowState.Normal);
}