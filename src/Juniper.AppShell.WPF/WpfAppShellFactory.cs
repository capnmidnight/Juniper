using System.Windows;

namespace Juniper.AppShell;

public class WpfAppShellFactory : IAppShellFactory
{
    public Task<IAppShell> StartAsync()
    {
        var task = new TaskCompletionSource<IAppShell>();
        var thread = new Thread(() =>
        {
            var app = Application.Current ?? new Application();
            app.Dispatcher.Invoke(() =>
            {
                var window = new WpfAppShell();
                task.SetResult(window);
                app.Run(window);
            });
        });

        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();

        return task.Task;
    }
}
