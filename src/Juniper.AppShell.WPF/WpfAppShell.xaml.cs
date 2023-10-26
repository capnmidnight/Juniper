using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;

namespace Juniper.AppShell;

public partial class WpfAppShell : Window, IAppShell
{
    private readonly Task init;

    public WpfAppShell()
    {
        InitializeComponent();

        WebView.ZoomFactorChanged += delegate
        {
            // This event will fire whenever the user changes zoom factor (e.g.,
            // by typing CTRL+PLUS/CTRL+MINUS, or doing CTRL-MOUSE_WHEEL).
            // But WebView2 doesn't persist user-selected zoom factor between
            // page loads. So we read the current (user-selected) zoom factor
            // and set it to be (program-selected) zoom factor and now it will
            // persist between page loads.
            WebView.ZoomFactor = WebView.ZoomFactor;
        };

        init = WebView.EnsureCoreWebView2Async();
    }

    private async Task<T> Do<T>(Func<Task<T>> action)
    {
        await init;
        var task = await Dispatcher.InvokeAsync(action).Task;
        return await task;
    }

    private async Task Do(Func<Task> action)
    {
        await init;
        var task = await Dispatcher.InvokeAsync(action).Task;
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

    public Task<Uri> GetSourceAsync() =>
        Do(() => WebView.Source);

    public Task SetSourceAsync(Uri value) =>
        Do(async () =>
        {
            var completer = new TaskCompletionSource();
            void CoreWebView2_DOMContentLoaded(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2DOMContentLoadedEventArgs e)
            {
                completer.SetResult();
            }

            WebView.CoreWebView2.DOMContentLoaded += CoreWebView2_DOMContentLoaded;

            SetSourceInternal(value);

            await completer.Task;
            WebView.CoreWebView2.DOMContentLoaded -= CoreWebView2_DOMContentLoaded;
        });

    private void SetSourceInternal(Uri value)
    {
        if (value == WebView.Source)
        {
            WebView.Reload();
        }
        else
        {
            WebView.Source = value;
        }
    }

    public Task<string> GetTitleAsync() =>
        Do(() => Title);

    public Task SetTitleAsync(string title) =>
        Do(() => Title = title);

    public Task<bool> GetCanGoBackAsync() =>
        Do(() => WebView.CanGoBack);

    public Task<bool> GetCanGoForwardAsync() =>
        Do(() => WebView.CanGoForward);

    public Task SetSizeAsync(int width, int height) =>
        Do(() =>
        {
            Width = width;
            Height = height;
        });

    public Task MaximizeAsync() =>
        Do(() => WindowState = WindowState.Maximized);

    public Task MinimizeAsync() =>
        Do(() => WindowState = WindowState.Minimized);

    public Task<bool> ToggleExpandedAsync() =>
        Do(() => (WindowState = WindowState == WindowState.Normal
                ? WindowState.Maximized
                : WindowState.Normal) == WindowState.Maximized);

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

    private readonly TaskCompletionSource closing = new();

    protected override void OnClosing(CancelEventArgs e)
    {
        base.OnClosing(e);
        closing.TrySetResult();
    }

    public Task WaitForCloseAsync() =>
        closing.Task;
}