using System.Windows;

using Microsoft.Win32;

namespace Juniper
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private readonly OpenFileDialog picker;

        public string FreeRDPPath
        {
            get
            {
                return FreeRDPPathBox.Text;
            }
            set
            {
                FreeRDPPathBox.Text = value;
            }
        }

        public SettingsWindow()
        {
            picker = new OpenFileDialog();
            picker.Multiselect = false;
            picker.Filter = "Executables|*.exe|All files|*.*";
            InitializeComponent();
        }

        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            if (picker.ShowDialog() == true)
            {
                FreeRDPPathBox.Text = picker.FileName;
            }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
