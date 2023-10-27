namespace Juniper.AppShell;

using Gtk;
using WebKit;

public class GtkAppShell : FixedWindow, IAppShell
{
    private readonly WebView webView;
    private readonly TaskCompletionSource deletedTask = new();

    private bool isCtrl = false;
    private bool isR = false;

    public GtkAppShell()
    : base("Juniper AppShell")
    {
        AddEvents(
            Gdk.EventMask.KeyReleaseMask
            | Gdk.EventMask.KeyPressMask
        );

        webView = new WebView
        {
            Events = Events
        };

        Add(webView);
        SetSizeRequest(800, 600);
    }
    protected override bool OnDeleteEvent(Gdk.Event evnt)
    {
        deletedTask.SetResult();
        return base.OnDeleteEvent(evnt);
    }

    protected override bool OnKeyPressEvent(Gdk.EventKey evnt)
    {
        if (evnt.Key == Gdk.Key.Control_L
           || evnt.Key == Gdk.Key.Control_R)
        {
            isCtrl = true;
        }
        else if (evnt.Key == Gdk.Key.r)
        {
            isR = true;
        }
        return base.OnKeyPressEvent(evnt);
    }

    protected override bool OnKeyReleaseEvent(Gdk.EventKey evnt)
    {
        var wasCtrl = isCtrl;
        var wasR = isR;
        var wasReload = wasCtrl && wasR;
        if (evnt.Key == Gdk.Key.Control_L
           || evnt.Key == Gdk.Key.Control_R)
        {
            isCtrl = false;
        }
        else if (evnt.Key == Gdk.Key.r)
        {
            isR = false;
        }
        var isReload = isCtrl && isR;
        if (wasReload && !isReload)
        {
            webView.Reload();
        }
        return base.OnKeyReleaseEvent(evnt);
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

    public Task SetSourceAsync(Uri source) =>
        Do(() =>
        {
            var task = new TaskCompletionSource();
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
            return task.Task;
        });

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
