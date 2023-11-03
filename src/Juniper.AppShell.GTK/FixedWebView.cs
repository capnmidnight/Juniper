using Gdk;
using Gtk;

namespace WebKit;

public class FixedWebView : WebView
{
    private readonly KeyStateManager keyState = new();

    private bool showingInspector = false;

    public FixedWebView()
        : base(new Settings
        {
#if DEBUG
            EnableDeveloperExtras = true,
#else
            EnableDeveloperExtras = false,
#endif
            AllowFileAccessFromFileUrls = false,
            AllowModalDialogs = true,
            AllowTopNavigationToDataUrls = true,
            AllowUniversalAccessFromFileUrls = false,
            AutoLoadImages = true,
            EnableBackForwardNavigationGestures = true,
            EnableHtml5Database = true,
            EnableHtml5LocalStorage = true,
            EnableSmoothScrolling = true,
            EnableJavascript = true,
            EnableMedia = true,
            EnableMediaCapabilities = true,
            EnableMediaStream = true,
            EnableMediasource = true,
            EnableWebaudio = true,
            EnableWebgl = true,
            EnableWriteConsoleMessagesToStdout = true,
            JavascriptCanAccessClipboard = true,
            JavascriptCanOpenWindowsAutomatically = true,
            MediaPlaybackAllowsInline = true,
            MediaPlaybackRequiresUserGesture = false,
        })
    {
    }

    protected override bool OnKeyPressEvent(EventKey evnt)
    {
        keyState.OnKeyPressEvent(evnt);
        return base.OnKeyPressEvent(evnt);
    }

    protected override bool OnKeyReleaseEvent(EventKey evnt)
    {
        keyState.OnKeyReleaseEvent(evnt);

        if (keyState.WasPressed(Gdk.Key.Control_L, Gdk.Key.r)
            || keyState.WasPressed(Gdk.Key.Control_R, Gdk.Key.r)
            || keyState.WasPressed(Gdk.Key.F5))
        {
            Reload();
        }

#if DEBUG
        if (keyState.WasPressed(Gdk.Key.F12))
        {
            if (showingInspector)
            {
                showingInspector = false;
                Inspector.Detach();
                Inspector.Close();
            }
            else
            {
                Inspector.Show();
                Inspector.Attach();
                showingInspector = true;
            }
        }
#endif
        return base.OnKeyReleaseEvent(evnt);
    }

    public Task LoadUriAsync(string source)
    {
        var task = new TaskCompletionSource();
        void cleanup()
        {
            LoadChanged -= onLoad;
            LoadFailed -= onError;
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

        LoadChanged += onLoad;
        LoadFailed += onError;

        UserContentManager.RegisterScriptMessageHandler("console");
        UserContentManager.ScriptMessageReceived += delegate (object? sender, ScriptMessageReceivedArgs e)
        {
            Console.WriteLine(sender);
            Console.WriteLine(e);
        };

        LoadUri(source);
        return task.Task;
    }
}