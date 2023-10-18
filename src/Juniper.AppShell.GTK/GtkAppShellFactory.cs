namespace Juniper.AppShell;

using System.Reflection;
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
        app.AddWindow(appShell);
        appShell.Show();

        return Task.FromResult((IAppShell)appShell);
    }
}
