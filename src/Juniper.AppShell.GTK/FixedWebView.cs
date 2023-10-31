namespace WebKit;

public class FixedWebView : WebView
{
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

        LoadUri(source);
        return task.Task;
    }
}