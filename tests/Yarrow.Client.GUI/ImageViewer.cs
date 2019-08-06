using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Juniper.Image;

namespace Yarrow.Client.GUI
{
    public partial class ImageViewer : Form
    {
        public ImageViewer()
        {
            InitializeComponent();
        }

        public void OpenImage(Stream stream)
        {
            cubeMapPictureBox.Image?.Dispose();
            var image = Image.FromStream(stream);
            cubeMapPictureBox.Image = image;
        }

        public void SetImage(ImageData image)
        {
            using (var mem = new MemoryStream(image.data))
            {
                OpenImage(mem);
            }
        }
    }
}