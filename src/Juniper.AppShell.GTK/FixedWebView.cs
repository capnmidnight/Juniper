namespace WebKit;

public class FixedWebView : WebView
{
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
            JavascriptCanAccessClipboard = true,
            JavascriptCanOpenWindowsAutomatically = true,
            MediaPlaybackAllowsInline = true,
            MediaPlaybackRequiresUserGesture = false,
        })
    {
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