using System.Windows;

namespace Juniper.AppShell.WPF
{
    public partial class AppShellWindow : Window, IAppShell
    {
        public AppShellWindow()
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

        public Task CloseAsync() =>
            Do(Close);
    }
}
