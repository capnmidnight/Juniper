namespace Juniper.AppShell;

using Gtk;

public class GtkAppShellFactory<AppShellT> : IAppShellFactory
    where AppShellT : Window, IAppShell, new()
{
    public Task<IAppShell> StartAsync(CancellationToken cancellationToken)
    {
        Application.Init();
        var shellTask = new TaskCompletionSource<IAppShell>();
        var thread = new Thread(new ThreadStart(delegate ()
        {
            var app = new Application("org.juniper.appshell", GLib.ApplicationFlags.None);
            app.Register(GLib.Cancellable.Current);

            var appShell = new AppShellT();
            app.AddWindow(appShell);
            shellTask.SetResult(appShell);
            appShell.Show();
            Application.Run();
        }));

        thread.Start();

        return shellTask.Task;
    }
}
