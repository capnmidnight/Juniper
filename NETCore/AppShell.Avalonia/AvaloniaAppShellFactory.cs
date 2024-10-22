using Avalonia;
using Avalonia.Controls;
using Xilium.CefGlue;
using Xilium.CefGlue.Common;

namespace Juniper.AppShell;

public class AvaloniaAppShellFactory<AppShellT> : IAppShellFactory
where AppShellT : Window, IAppShell, new()
{

    public bool OwnsRunning => false;
    public void Run() { }

    private TaskCompletionSource ready = new TaskCompletionSource();
    public Task Ready => ready.Task;

    private TaskCompletionSource complete = new TaskCompletionSource();
    public Task Complete => complete.Task;


    public async Task<IAppShell> StartAsync(CancellationToken cancellationToken)
    {
        var appShellCreating = new TaskCompletionSource<IAppShell>();
        var thread = new Thread(() =>
        {
            AppBuilder.Configure<AvaloniaApp>()
                .UsePlatformDetect()
                .WithInterFont()
                .LogToTrace()
                .AfterSetup(app =>
                {
                    var cachePath = new DirectoryInfo(Environment.CurrentDirectory).CD(".foxcache");
                    if (!cachePath.Exists)
                    {
                        cachePath.Create();
                    }

                    CefRuntimeLoader.Initialize(new CefSettings()
                    {
                        RootCachePath = cachePath.FullName,
                        CachePath = cachePath.FullName,
                        NoSandbox = true,
                        WindowlessRenderingEnabled = false
                    });

                    if (app.Instance is AvaloniaApp avApp)
                    {
                        avApp.DesktopReady += (_, evt) =>
                        {
                            var appShell = new AppShellT();
                            evt.Value.MainWindow = appShell;
                            appShellCreating.TrySetResult(appShell);
                            ready.TrySetResult();
                        };
                    }
                })
                .StartWithClassicDesktopLifetime(Environment.GetCommandLineArgs());
                complete.TrySetResult();
        });

        thread.Start();

        return await appShellCreating.Task;
    }
}

public class AvaloniaAppShellFactory : AvaloniaAppShellFactory<AvaloniaAppShell> { }