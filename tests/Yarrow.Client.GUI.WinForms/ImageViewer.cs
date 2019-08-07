using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Yarrow.Client.GUI.WinForms
{
    public partial class ImageViewer : Form
    {
        private readonly Action<FileInfo> SetImageDelegate;

        public ImageViewer()
        {
            InitializeComponent();
            SetImageDelegate = SetImage;
        }

        public void SetImage(FileInfo file)
        {
            if (InvokeRequired)
            {
                Invoke(SetImageDelegate, file);
            }
            else
            {
                cubeMapPictureBox.Image?.Dispose();
                var image = Image.FromFile(file.FullName);
                cubeMapPictureBox.Image = image;
            }
        }

        public event EventHandler<string> LocationSubmitted;

        private void OnLocationSubmitted()
        {
            locationTextBox.Text = locationTextBox.Text.Trim();
            if (locationTextBox.Text.Length > 0)
            {
                LocationSubmitted?.Invoke(this, locationTextBox.Text);
            }
        }

        private void SubmitButton_Click(object sender, EventArgs e)
        {
            OnLocationSubmitted();
        }

        private void LocationTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                OnLocationSubmitted();
            }
        }
    }
}