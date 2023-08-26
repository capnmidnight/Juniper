using System.Windows;

namespace Juniper.AppShell.WPF
{
    public class AppShellFactory : IAppShellFactory
    {
        public Task<IAppShell> StartAsync()
        {
            var task = new TaskCompletionSource<IAppShell>();
            var thread = new Thread(() =>
            {
                var app = Application.Current ?? new Application();
                app.Dispatcher.Invoke(() =>
                {
                    var window = new AppShellWindow();
                    task.SetResult(window);
                    app.Run(window);
                });
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

            return task.Task;
        }
    }
}
