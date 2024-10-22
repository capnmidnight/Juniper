using Avalonia.Controls;
using Avalonia.Threading;
using Xilium.CefGlue.Avalonia;
using Xilium.CefGlue.Common.Events;

namespace Juniper.AppShell;

public partial class AvaloniaAppShell : Window, IAppShell
{
    private readonly TaskCompletionSource closing = new();
    private readonly AvaloniaCefBrowser WebView;

    public AvaloniaAppShell()
    {
        InitializeComponent();

        WebView = new AvaloniaCefBrowser();
        browserWrapper.Child = WebView;
        Closing += (_, __) => closing.TrySetResult();
    }

    public void Dispose() => WebView.Dispose();



#pragma warning disable IDE0051 // Remove unused private members
    // This might become useful later, in which case, remove the `#pragma`s
    private async Task<T> Do<T>(Func<Task<T>> action) =>
        await Dispatcher.UIThread.InvokeAsync(action);
#pragma warning restore IDE0051

    private async Task Do(Func<Task> action) =>
        await Dispatcher.UIThread.InvokeAsync(action);

    private async Task<T> Do<T>(Func<T> action) =>
        await Dispatcher.UIThread.InvokeAsync(action);

    private async Task Do(Action action) =>
        await Dispatcher.UIThread.InvokeAsync(action);


    /////////////////
    //// CLOSING ////
    /////////////////

    public Task ShowAsync() =>
        Do(Show);

    public Task HideAsync() =>
        Do(Hide);

    public async Task CloseAsync() => 
        await Dispatcher.UIThread.InvokeAsync(Close);

    public Task WaitForCloseAsync() =>
        closing.Task;

    ////////////////
    //// SOURCE ////
    ////////////////

    public Task<Uri> GetSourceUriAsync() =>
        Do(() => new Uri(WebView.Address));

    public Task SetSourceUriAsync(Uri value) =>
        Do(() => LoadAsync(SetSourceInternal, value));

    private void SetSourceInternal(Uri uri)
    {
        var value = uri.ToString();
        if (value == WebView.Address)
        {
            WebView.Reload();
        }
        else
        {
            WebView.Address = value;
        }
    }

    private void NavigateToString(string html)
    {
        var filename = Path.GetTempFileName();
        File.WriteAllText(filename, html);
        WebView.Address = filename;
    }

    public Task SetSourceHTMLAsync(string html) =>
        Do(() => LoadAsync(NavigateToString, html));

    private async Task LoadAsync<T>(Action<T> act, T value)
    {
        var completer = new TaskCompletionSource();
        void contentLoaded(object? sender, LoadEndEventArgs e)
        {
            completer.SetResult();
        }

        WebView.LoadEnd += contentLoaded;

        act(value);

        await completer.Task;
        WebView.LoadEnd -= contentLoaded;
    }

    public Task ReloadAsync() =>
        Do(() => WebView.Reload());

    ///////////////
    //// TITLE ////
    ///////////////

    public Task<string> GetTitleAsync() =>
        Do(() => Title ?? "");

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
        Do(() => SystemDecorations == SystemDecorations.None);

    public Task SetIsBorderlessAsync(bool isBorderless) =>
        Do(() => SystemDecorations = isBorderless
            ? SystemDecorations.None
            : SystemDecorations.Full);

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