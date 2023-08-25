using System.Windows;

namespace Juniper.AppShell
{
    public partial class MainWindow : Window
    {
        public static void Start(string address)
        {
            var app = new Application();
            app.Dispatcher.Invoke(() =>
            {
                var window = new MainWindow(address);
                app.Run(window);
            });
        }

        public string Source { get; }

        public MainWindow(string source)
        {
            DataContext = this;
            Source = source;
            InitializeComponent();
        }
    }
}
