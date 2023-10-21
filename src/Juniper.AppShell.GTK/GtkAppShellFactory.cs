namespace Juniper.AppShell;

using Gtk;

public class GtkAppShellFactory<AppShellT> : IAppShellFactory
    where AppShellT : Window, IAppShell, new()
{
    public Task<IAppShell> StartAsync(CancellationToken cancellationToken)
    {
        Application.Init();

        var app = new Application("org.juniper.appshell", GLib.ApplicationFlags.None);
        app.Register(GLib.Cancellable.Current);

        var appShell = new AppShellT();
        appShell.WaitForCloseAsync().ContinueWith(delegate {
            Application.Invoke(delegate {
                Application.Quit();
            });
        }, cancellationToken);

        app.AddWindow(appShell);

        appShell.ShowAll();

        return Task.FromResult((IAppShell)appShell);
    }
}

public class GtkAppShellFactory : GtkAppShellFactory<GtkAppShell>
{

}