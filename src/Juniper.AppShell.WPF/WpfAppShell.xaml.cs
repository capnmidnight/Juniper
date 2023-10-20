using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;

namespace Juniper.AppShell;

public partial class WpfAppShell : Window, IAppShell
{
    public WpfAppShell()
    {
        InitializeComponent();
    }

    private Task<T> Do<T>(Func<T> action) =>
        Dispatcher.InvokeAsync(action).Task;

    private Task Do(Action action) =>
        Dispatcher.InvokeAsync(action).Task;

    public Task<Uri> GetSourceAsync() =>
        Do(() => WebView.Source);

    public Task SetSourceAsync(Uri value) =>
        Do(async () =>
        {
            var completer = new TaskCompletionSource();
            void WebView_Loaded(object sender, RoutedEventArgs e)
            {
                completer.SetResult();
            }

            WebView.Loaded += WebView_Loaded;

            if (value == WebView.Source)
            {
                WebView.Reload();
            }
            else
            {
                WebView.Source = value;
            }

            await completer.Task;
            WebView.Loaded -= WebView_Loaded;
        });

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