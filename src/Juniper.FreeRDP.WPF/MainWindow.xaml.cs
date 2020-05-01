using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Juniper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public ObservableCollection<Connection> Connections { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        private Connection connection;
        private static readonly PropertyChangedEventArgs connectionChanged = new PropertyChangedEventArgs(nameof(CurrentConnection));
        private static readonly PropertyChangedEventArgs hasConnectionChanged = new PropertyChangedEventArgs(nameof(HasConnection));
        public bool HasConnection => connection != null;
        public Connection CurrentConnection
        {
            get
            {
                return connection;
            }
            private set
            {
                if(connection != value)
                {
                    var hadConnection = HasConnection;
                    connection = value;
                    PropertyChanged?.Invoke(this, connectionChanged);
                    if(hadConnection != HasConnection)
                    {
                        PropertyChanged?.Invoke(this, hasConnectionChanged);
                    }
                }
            }
        }

        public MainWindow()
        {
            Connections = new ObservableCollection<Connection>(Connection.Load());
            InitializeComponent();
            DataContext = this;
            WindowSizeCombo.ItemsSource = Connection.WindowSizes;
            ColorDepthCombo.ItemsSource = Connection.ColorDepths;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Connection.Save(Connections);
            UpdateConnectionsList();
        }

        private void Save()
        {
            Connection.Save(Connections);
            UpdateConnectionsList();
        }

        private void UpdateConnectionsList()
        {
            var curSelection = ConnectionsCombo.SelectedIndex;
            if (curSelection >= Connections.Count)
            {
                curSelection = Connections.Count - 1;
            }
            else if (curSelection < 0)
            {
                curSelection = 0;
            }


            if (0 <= curSelection
                && curSelection < Connections.Count)
            {
                ConnectionsCombo.SelectedIndex = curSelection;
            }
        }

        private void ConnectionsCombo_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (0 <= ConnectionsCombo.SelectedIndex
                && ConnectionsCombo.SelectedIndex < Connections.Count)
            {
                CurrentConnection = Connections[ConnectionsCombo.SelectedIndex];
            }
            else
            {
                CurrentConnection = null;
            }
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            var newIndex = Connections.Count;
            Connections.Add(new Connection());
            UpdateConnectionsList();
            ConnectionsCombo.SelectedIndex = newIndex;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Connections.RemoveAt(ConnectionsCombo.SelectedIndex);
            Save();
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            Save();

            if (CurrentConnection is object
                && !string.IsNullOrEmpty(CurrentConnection.Server))
            {
                CurrentConnection.Start(PasswordBox.Password);
            }

            Close();
        }

        private void PortBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (CurrentConnection is null)
            {
                return;
            }

            if (int.TryParse(PortBox.Text, out var port))
            {
                CurrentConnection.Port = port;
            }
            else
            {
                var selection = PortBox.SelectionStart;
                PortBox.Text = CurrentConnection.Port.ToString(CultureInfo.InvariantCulture);
                PortBox.SelectionStart = selection - 1;
                PortBox.SelectionLength = 0;
            }
        }

        private void SettingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var settings = new SettingsWindow
            {
                FreeRDPPath = Properties.Settings.Default.FreeRDPPath
            };
            if (settings.ShowDialog() == true)
            {
                if (File.Exists(settings.FreeRDPPath))
                {
                    Properties.Settings.Default.FreeRDPPath = settings.FreeRDPPath;
                    Properties.Settings.Default.Save();
                }
            }
        }
    }
}
