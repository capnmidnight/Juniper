using System.Windows;

namespace Juniper.AppShell.WPF
{
    public partial class AppShellWindow : Window, IAppShell
    {
        public AppShellWindow()
        {
            InitializeComponent();
        }

        public Uri Source
        {
            get => Dispatcher.Invoke(() => WebView.Source);
            set => Dispatcher.Invoke(() => WebView.Source = value);
        }
    }
}
