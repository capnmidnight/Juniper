namespace Juniper.AppShell;

using Gtk;
using WebKit;

public class GtkAppShell : Window, IAppShell
{
    private readonly WebView webView;
    private readonly TaskCompletionSource deletedTask = new();
    private readonly TaskCompletionSource<WebView> createdTask = new();
    public GtkAppShell()
    : base("Juniper AppShell")
    {
        SetSizeRequest(800, 600);
        webView = new WebView();
        Add(webView);

        DeleteEvent += delegate { deletedTask.SetResult(); };
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

    public Task CloseAsync() =>
        Do(Close);

    public Task WaitForCloseAsync() =>
        deletedTask.Task;

    public Task<bool> GetCanGoBackAsync() =>
        Do(webView.CanGoBack);

    public Task<bool> GetCanGoForwardAsync() =>
        Do(webView.CanGoForward);

    public Task<Uri> GetSourceAsync() =>
        Do(() => new Uri(webView.Uri));

    public Task<string> GetTitleAsync() =>
        Do(() => Title);

    public Task MaximizeAsync() =>
        Do(Maximize);

    public Task MinimizeAsync() =>
        Do(Iconify);

    public Task SetSizeAsync(int width, int height) =>
        Do(() => Resize(width, height));

    public Task SetSourceAsync(Uri source)
    {
        var task = new TaskCompletionSource();
        Application.Invoke(delegate
        {
            void cleanup()
            {
                webView.LoadChanged -= onLoad;
                webView.LoadFailed -= onError;
            }

            void onLoad(object? sender, LoadChangedArgs e)
            {
                if (e.LoadEvent.HasFlag(LoadEvent.Finished))
                {
                    cleanup();
                    task!.SetResult();
                }
            }

            void onError(object? sender, LoadFailedArgs e)
            {
                cleanup();
                task!.SetException(new Exception($"Load failed: {e.FailingUri}"));
            }

            webView.LoadChanged += onLoad;
            webView.LoadFailed += onError;

            webView.LoadUri(source.ToString());
        });
        return task.Task;
    }

    public Task SetTitleAsync(string title) =>
        Do(() => Title = title);

    public Task<bool> ToggleExpandedAsync() =>
        Do(() =>
        {
            if (IsMaximized)
            {
                Deiconify();
                return false;
            }
            else
            {
                Maximize();
                return true;
            }
        });
}
