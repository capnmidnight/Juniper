namespace Juniper.AppShell;

using Gdk;
using Gtk;
using WebKit;

public class GtkAppShell : FixedWindow, IAppShell
{
    private readonly WebView webView;
    private readonly TaskCompletionSource deletedTask = new();

    private bool isIconified = false;
    private bool isFullscreen = false;

    private bool isCtrl = false;
    private bool isR = false;

    public GtkAppShell()
    : base("Juniper AppShell")
    {
        WindowStateEvent += delegate (object? sender, WindowStateEventArgs e)
        {
            isIconified = (e.Event.NewWindowState & WindowState.Iconified) != 0;
            isFullscreen = (e.Event.NewWindowState & WindowState.Fullscreen) != 0;
        };

        AddEvents(
            EventMask.KeyReleaseMask
            | EventMask.KeyPressMask
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

    /////////////////
    //// CLOSING ////
    /////////////////

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

    public Task<Juniper.Size> GetSizeAsync() =>
        Do(() =>
        {
            GetSize(out var width, out var height);
            return new Juniper.Size(width, height);
        });

    public Task SetSizeAsync(int width, int height) =>
        Do(() => Resize(width, height));

    ////////////////////
    //// Fullscreen ////
    ////////////////////

    public Task<bool> GetIsFullscreenAsync() =>
        Do(() => isFullscreen);

    public Task SetIsFullscreenAsync(bool isFullscreen) =>
        Do(() =>
        {
            if (this.isFullscreen != isFullscreen)
            {
                if (isFullscreen)
                {
                    Fullscreen();
                }
                else
                {
                    Unfullscreen();
                }
            }
        });

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
        Do(() =>
        {
            if (IsMaximized != isMaximized)
            {
                if (isMaximized)
                {
                    Maximize();
                }
                else
                {
                    Unmaximize();
                }
            }
        });

    ///////////////////
    //// MINIMIZED ////
    ///////////////////

    public Task<bool> GetIsMinimizedAsync() =>
        Do(() => isIconified);

    public Task SetIsMinimizedAsync(bool isMinimized) =>
        Do(() =>
        {
            if (isIconified != isMinimized)
            {
                if (isMinimized)
                {
                    Iconify();
                }
                else
                {
                    Deiconify();
                }
            }
        });
}
