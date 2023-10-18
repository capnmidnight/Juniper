namespace Juniper.AppShell;

using Gtk;
using WebKit;

public class GtkAppShell : Window, IAppShell
{
    private readonly WebView webView;
    private readonly TaskCompletionSource deletedTask = new ();
    public GtkAppShell()
    : base("Juniper AppShell")
    {
        webView = new WebView();
        Add(webView);
        
        DeleteEvent += (object src, DeleteEventArgs evt) => deletedTask.SetResult();
    }

    public Task CloseAsync()
    {
        Close();
        return Task.CompletedTask;
    }

    public Task WaitForCloseAsync() =>
        deletedTask.Task;

    public Task<bool> GetCanGoBackAsync() =>
        Task.FromResult(webView.CanGoBack());

    public Task<bool> GetCanGoForwardAsync() =>
        Task.FromResult(webView.CanGoForward());

    public Task<Uri> GetSourceAsync() =>
        Task.FromResult(new Uri(webView.Uri));

    public Task<string> GetTitleAsync() =>
        Task.FromResult(Title);

    public Task MinimizeAsync()
    {
        Iconify();
        return Task.CompletedTask;
    }

    public Task SetSizeAsync(int width, int height)
    {
        SetSizeRequest(width, height);
        return Task.CompletedTask;
    }

    public Task SetSourceAsync(Uri source)
    {
        webView.LoadUri(source.ToString());
        return Task.CompletedTask;
    }

    public Task SetTitleAsync(string title)
    {
        Title = title;
        return Task.CompletedTask;
    }

    public Task<bool> ToggleExpandedAsync()
    {
        if(IsMaximized){
            Deiconify();
            return Task.FromResult(false);
        }
        else{
            Maximize();
            return Task.FromResult(true);
        }
    }
}
