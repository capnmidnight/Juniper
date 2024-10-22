using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace Juniper.AppShell;

public partial class AvaloniaApp : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public event EventHandler<EventArgs<IClassicDesktopStyleApplicationLifetime>>? DesktopReady;

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            DesktopReady?.Invoke(this, new EventArgs<IClassicDesktopStyleApplicationLifetime>(desktop));
        }

        base.OnFrameworkInitializationCompleted();
    }
}