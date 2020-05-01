using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Accord;

namespace Juniper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly string[] WindowSizes =
        {
            "Fullscreen",
            "1920x1080",
            "1280x720",
            "1152x864",
            "1024x768",
            "800x600"
        };

        private static readonly int[] ColorDepths = {
            0,
            16,
            32
        };

        private readonly List<Connection> connections;
        private Connection connection;


        public MainWindow()
        {
            connections = Connection.Load();
            InitializeComponent();
            foreach (var size in WindowSizes)
            {
                WindowSizeCombo.Items.Add(size);
            }

            foreach (var bits in ColorDepths)
            {
                ColorDepthCombo.Items.Add(bits == 0 ? "Default" : bits.ToString(CultureInfo.InvariantCulture));
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Connection.Save(connections);
            UpdateConnectionsList();
        }

        private void Save()
        {
            Connection.Save(connections);
            UpdateConnectionsList();
        }

        private void UpdateConnectionsList()
        {
            var curSelection = ConnectionsCombo.SelectedIndex;
            ConnectionsCombo.Items.Clear();
            foreach (var connection in connections)
            {
                ConnectionsCombo.Items.Add(connection.Name);
            }

            if (curSelection >= connections.Count)
            {
                curSelection = connections.Count - 1;
            }
            else if (curSelection < 0)
            {
                curSelection = 0;
            }


            if (0 <= curSelection
                && curSelection < connections.Count)
            {
                ConnectionsCombo.SelectedIndex = curSelection;
            }
        }

        private void ConnectionsCombo_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            connection = null;
            NameBox.Text = string.Empty;
            ServerBox.Text = string.Empty;
            PortBox.Text = string.Empty;
            WindowSizeCombo.SelectedIndex = 0;
            ColorDepthCombo.SelectedIndex = 0;
            UserNameBox.Text = string.Empty;
            PasswordBox.Password = string.Empty;
            SavePasswordCheck.IsChecked = null;
            DomainBox.Text = string.Empty;
            SaveButton.IsEnabled = false;
            DeleteButton.IsEnabled = false;
            ConnectionGroup.IsEnabled = false;

            if (0 <= ConnectionsCombo.SelectedIndex
                && ConnectionsCombo.SelectedIndex < connections.Count)
            {
                var connection = connections[ConnectionsCombo.SelectedIndex];
                NameBox.Text = connection.Name;
                ServerBox.Text = connection.Server ?? string.Empty;
                PortBox.Text = connection.Port.ToString(CultureInfo.InvariantCulture);
                WindowSizeCombo.SelectedIndex = Array.IndexOf(WindowSizes, connection.WindowSize);
                ColorDepthCombo.SelectedIndex = Math.Max(0, Array.IndexOf(ColorDepths, connection.ColorDepth));
                UserNameBox.Text = connection.UserName ?? string.Empty;
                PasswordBox.Password = connection.Password ?? string.Empty;
                SavePasswordCheck.IsChecked = connection.SavePassword;
                DomainBox.Text = connection.Domain ?? string.Empty;
                DeleteButton.IsEnabled = true;
                ConnectionGroup.IsEnabled = true;
                this.connection = connection;
            }
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            var newIndex = connections.Count;
            connections.Add(new Connection());
            UpdateConnectionsList();
            ConnectionsCombo.SelectedIndex = newIndex;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Save();
            SaveButton.IsEnabled = false;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            connections.RemoveAt(ConnectionsCombo.SelectedIndex);
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

            if (connection is object
                && !string.IsNullOrEmpty(connection.Server))
            {
                connection.Start();
            }

            Close();
        }

        private void NameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (connection is null)
            {
                return;
            }

            connection.Name = NameBox.Text;
            SaveButton.IsEnabled = true;
        }

        private void ServerBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (connection is null)
            {
                return;
            }

            connection.Server = ServerBox.Text;
            SaveButton.IsEnabled = true;
        }

        private void PortBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (connection is null)
            {
                return;
            }

            if (int.TryParse(PortBox.Text, out var port))
            {
                connection.Port = port;
                SaveButton.IsEnabled = true;
            }
            else
            {
                PortBox.Text = connection.Port.ToString(CultureInfo.InvariantCulture);
            }
        }

        private void WindowSizeCombo_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (connection is null)
            {
                return;
            }

            connection.WindowSize = WindowSizes[WindowSizeCombo.SelectedIndex];
            SaveButton.IsEnabled = true;
        }

        private void ColorDepthCombo_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (connection is null)
            {
                return;
            }

            connection.ColorDepth = ColorDepths[ColorDepthCombo.SelectedIndex];
            SaveButton.IsEnabled = true;
        }

        private void UserNameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (connection is null)
            {
                return;
            }

            connection.UserName = UserNameBox.Text;
            SaveButton.IsEnabled = true;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (connection is null)
            {
                return;
            }

            connection.Password = PasswordBox.Password;
            SaveButton.IsEnabled = true;
        }

        private void DomainBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (connection is null)
            {
                return;
            }

            connection.Domain = DomainBox.Text;
            SaveButton.IsEnabled = true;
        }

        private void SavePasswordCheck_Checked(object sender, RoutedEventArgs e)
        {
            if (connection is null)
            {
                return;
            }

            connection.SavePassword = SavePasswordCheck.IsChecked == true;
            SaveButton.IsEnabled = true;
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
