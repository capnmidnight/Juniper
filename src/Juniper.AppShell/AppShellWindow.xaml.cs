using System.Runtime.CompilerServices;
using System.Windows;

namespace Juniper.AppShell
{
    public partial class AppShellWindow : Window
    {
        public static void Start(string address)
        {
            var app = new Application();
            app.Dispatcher.Invoke(() =>
            {
                var window = new AppShellWindow(address);
                app.Run(window);
            });
        }

        public string Source { get; }

        public AppShellWindow(string source)
        {
            DataContext = this;
            Source = source;
            InitializeComponent();
        }
    }
}
