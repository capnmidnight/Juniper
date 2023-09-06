using System.Windows;

namespace Juniper.AppShell;

public class WpfAppShellFactory<AppShellT> : IAppShellFactory
    where AppShellT : Window, IAppShell, new()
{
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
                app.Run(appShell);
            });
        });

        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();

        return await appShellCreating.Task;
    }
}
