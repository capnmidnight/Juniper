#if WINDOWS
using System.Windows;
#endif

namespace Juniper.AppShell;


public class WpfAppShellFactory<AppShellT> : IAppShellFactory
    where AppShellT : 
    #if WINDOWS
    Window, 
    #endif
    IAppShell, new()
{
    public Task<IAppShell> StartAsync(CancellationToken cancellationToken)
    {
        #if WINDOWS
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

        return appShellCreating.Task;
        #else
        throw new PlatformNotSupportedException("WPF AppShell is only available on Windows.");
        #endif
    }
}

public class WpfAppShellFactory : WpfAppShellFactory<WpfAppShell> {
    
}