namespace Juniper.AppShell;

using Gtk;

public class GtkAppShellFactory<AppShellT> : IAppShellFactory
    where AppShellT : Window, IAppShell, new()
{
    private readonly TaskCompletionSource<IAppShell> appShellCreating = new TaskCompletionSource<IAppShell>();
    public Task<IAppShell> StartAsync(CancellationToken cancellationToken)
    {
        return appShellCreating.Task;
    }

    public void Run()
    {
        Application.Init();
        var app = new Application("org.juniper.appshell", GLib.ApplicationFlags.None);
        var appShell = new AppShellT();
        app.Register(GLib.Cancellable.Current);
        app.AddWindow(appShell);
        appShell.ShowAll();
        appShellCreating.SetResult(appShell);
        Application.Run();
    }
}

public class GtkAppShellFactory : GtkAppShellFactory<GtkAppShell>
{

}