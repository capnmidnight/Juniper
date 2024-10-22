using System.Windows;

namespace Juniper.AppShell;

public class WpfAppShellFactory<AppShellT> : IAppShellFactory
    where AppShellT : Window, IAppShell, new()
{
    public bool OwnsRunning => false;
    public void Run() {}

    private TaskCompletionSource ready = new TaskCompletionSource();
    public Task Ready => ready.Task;

    private TaskCompletionSource complete = new TaskCompletionSource();
    public Task Complete => complete.Task;
    
    public async Task<IAppShell> StartAsync(CancellationToken cancellationToken)
    {
        var appShellCreating = new TaskCompletionSource<IAppShell>();
        var thread = new Thread(() =>
        {
            var app = Application.Current ?? new Application();
            app.Dispatcher.Invoke(() =>
            {
                var appShell = new AppShellT();
                appShellCreating.TrySetResult(appShell);
                ready.TrySetResult();
                app.Run(appShell);
                complete.TrySetResult();
            });
        });

        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();

        return await appShellCreating.Task;
    }
}

public class WpfAppShellFactory : WpfAppShellFactory<WpfAppShell>
{

}