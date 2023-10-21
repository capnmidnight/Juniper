namespace Juniper.AppShell;

using Gtk;

public class GtkAppShellFactory<AppShellT> : IAppShellFactory
    where AppShellT : Window, IAppShell, new()
{
    public Task<IAppShell> StartAsync(CancellationToken cancellationToken)
    {
        var thread = Thread.CurrentThread;
        Application.Init();
        var app = new Application("org.juniper.appshell", GLib.ApplicationFlags.None);
        app.Register(GLib.Cancellable.Current);

        var appShell = new AppShellT();
        app.AddWindow(appShell);
        appShell.ShowAll();

        return Task.FromResult(appShell as IAppShell);
    }
}
