using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Linq;

using Juniper.Google.Maps.Geocoding;
using Juniper.Google.Maps.StreetView;
using Juniper.Image;

namespace Yarrow.Client.GUI.WinForms
{
    public partial class ImageViewer : Form
    {
        private readonly Action<MetadataResponse, GeocodingResponse, ImageData> SetImageDelegate;

        public ImageViewer()
        {
            InitializeComponent();
            SetImageDelegate = SetImage;
        }

        public void SetImage(MetadataResponse metadata, GeocodingResponse geocode, ImageData image)
        {
            if (InvokeRequired)
            {
                Invoke(SetImageDelegate, metadata, geocode, image);
            }
            else
            {
                var address = (from result in geocode.results
                               orderby result.formatted_address.Length descending
                               select result.formatted_address)
                            .FirstOrDefault();

                locationTextBox.Text = address ?? string.Empty;
                panoTextbox.Text = metadata.pano_id.ToString();
                latLngTextbox.Text = metadata.location.ToString();
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

        private void LocationTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                OnLocationSubmitted();
            }
        }

        public event EventHandler<string> PanoSubmitted;

        private void OnPanoSubmitted()
        {
            panoTextbox.Text = panoTextbox.Text.Trim();
            if (panoTextbox.Text.Length > 0)
            {
                PanoSubmitted?.Invoke(this, panoTextbox.Text);
            }
        }

        private void PanoTextbox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                OnPanoSubmitted();
            }
        }

        public event EventHandler<string> LatLngSubmitted;

        private void OnLatLngSubmitted()
        {
            latLngTextbox.Text = latLngTextbox.Text.Trim();
            if (latLngTextbox.Text.Length > 0)
            {
                LatLngSubmitted?.Invoke(this, latLngTextbox.Text);
            }
        }

        private void LatLngTextbox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                OnLatLngSubmitted();
            }
        }
    }
}