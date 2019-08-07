using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Juniper.Image;

namespace Yarrow.Client.GUI.WinForms
{
    public partial class ImageViewer : Form
    {
        private readonly Action<ImageData> SetImageDelegate;

        public ImageViewer()
        {
            InitializeComponent();
            SetImageDelegate = SetImage;
        }

        public void SetImage(ImageData image)
        {
            if (InvokeRequired)
            {
                Invoke(SetImageDelegate, image);
            }
            else
            {
                cubeMapPictureBox.Image?.Dispose();
                using (var mem = new MemoryStream(image.data))
                {
                    cubeMapPictureBox.Image = Image.FromStream(mem);
                }
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