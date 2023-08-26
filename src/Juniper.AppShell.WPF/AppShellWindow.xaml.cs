using System.Windows;

namespace Juniper.AppShell.WPF
{
    public partial class AppShellWindow : Window, IAppShell
    {
        public AppShellWindow()
        {
            InitializeComponent();
        }

        public Task<Uri> GetSourceAsync() => 
            Task.FromResult(Dispatcher.Invoke(() => WebView.Source));

        public Task SetSourceAsync(Uri value)
        {
            Dispatcher.Invoke(() =>
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
            return Task.CompletedTask;
        }

        public Task CloseAsync()
        {
            Dispatcher.Invoke(() => Close());
            return Task.CompletedTask;
        }
    }
}
