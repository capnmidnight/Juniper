namespace Juniper.AppShell;

using Gtk;

public class GtkAppShellFactory<AppShellT> : IAppShellFactory
    where AppShellT : Window, IAppShell, new()
{
    public bool OwnsRunning => true;
    public void Run()
    {
        Application.Run();
        complete.TrySetResult();
    }

    private TaskCompletionSource ready = new TaskCompletionSource();
    public Task Ready => ready.Task;

    private TaskCompletionSource complete = new TaskCompletionSource();
    public Task Complete => complete.Task;

    public Task<IAppShell> StartAsync(CancellationToken cancellationToken)
    {
        Application.Init();

        var app = new Application("org.juniper.appshell", GLib.ApplicationFlags.None);
        app.Register(GLib.Cancellable.Current);

        var appShell = new AppShellT();
        appShell.WaitForCloseAsync().ContinueWith(delegate
        {
            Application.Quit();
        }, cancellationToken);

        app.AddWindow(appShell);

        appShell.ShowAll();

        ready.TrySetResult();
        return Task.FromResult((IAppShell)appShell);
    }
}

public class GtkAppShellFactory : GtkAppShellFactory<GtkAppShell>
{

}