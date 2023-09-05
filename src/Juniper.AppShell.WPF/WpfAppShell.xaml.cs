using System.Windows;

namespace Juniper.AppShell;

public partial class WpfAppShell : Window, IAppShell
{
    public WpfAppShell()
    {
        InitializeComponent();
    }

    private Task<T> Do<T>(Func<T> action) =>
        Task.FromResult(Dispatcher.Invoke(action));

    private Task Do(Action action)
    {
        Dispatcher.Invoke(action);
        return Task.CompletedTask;
    }

    public Task<Uri> GetSourceAsync() =>
        Do(() => WebView.Source);

    public Task SetSourceAsync(Uri value) =>
        Do(() =>
        {
            if (value == WebView.Source)
            {
                WebView.Reload();
            }
            else
            {
                WebView.Source = value;
            }
        });

    public Task<string> GetTitleAsync() =>
        Do(() => Title);

    public Task SetTitleAsync(string title) =>
        Do(() => Title = title);

    public Task CloseAsync() =>
        Do(Close);

    public async Task WaitForCloseAsync() =>
        await await Do(() =>
        {
            var task = new TaskCompletionSource();
            Closed += (_, __) => 
                task.SetResult();
            return task.Task;
        });
}
