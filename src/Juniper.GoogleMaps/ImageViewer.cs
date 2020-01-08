using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

using Juniper.World.GIS;
using Juniper.World.GIS.Google.Geocoding;
using Juniper.World.GIS.Google.StreetView;

namespace Juniper.GoogleMaps
{
    public partial class ImageViewer : Form
    {
        public event EventHandler<StringEventArgs> LocationSubmitted;
        public event EventHandler<StringEventArgs> PanoSubmitted;
        public event EventHandler<LatLngPointEventArgs> LatLngSubmitted;

        public ImageViewer()
        {
            InitializeComponent();
        }

        public void SetImage(MetadataResponse metadata, GeocodingResponse geocode, Image image)
        {
            if (metadata is null)
            {
                throw new ArgumentNullException(nameof(metadata));
            }

            if (geocode is null)
            {
                throw new ArgumentNullException(nameof(geocode));
            }

            var address = (from result in geocode.Results
                           orderby result.Formatted_Address.Length descending
                           select result.Formatted_Address)
                        .FirstOrDefault();
            SetControls(metadata, image, address);
        }

        private void SetControls(MetadataResponse metadata, Image image, string address)
        {
            if (InvokeRequired)
            {
                _ = Invoke(new Action<MetadataResponse, Image, string>(SetControls), metadata, image, address);
            }
            else
            {
                locationTextBox.Text = address ?? string.Empty;
                panoTextbox.Text = metadata.Pano_ID;
                latLngTextbox.Text = metadata.Location.ToString(CultureInfo.InvariantCulture);
                cubeMapPictureBox.Image?.Dispose();
                cubeMapPictureBox.Image = image;
            }
        }

        public void SetError(Exception exp = null)
        {
            if (InvokeRequired)
            {
                _ = Invoke(new Action<Exception>(SetError), exp);
            }
            else
            {
                var msg = exp?.Message ?? "ERROR";

                if (panoTextbox.Text.Length == 0)
                {
                    panoTextbox.Text = msg;
                }

                if (latLngTextbox.Text.Length == 0)
                {
                    latLngTextbox.Text = msg;
                }

                if (locationTextBox.Text.Length == 0
                    || (panoTextbox.Text.Length > 0
                        && latLngTextbox.Text.Length > 0))
                {
                    locationTextBox.Text = msg;
                }

                cubeMapPictureBox.Image?.Dispose();
                cubeMapPictureBox.Image = null;
            }
        }

        private void LocationTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                locationTextBox.Text = locationTextBox.Text.Trim();
                panoTextbox.Text = string.Empty;
                latLngTextbox.Text = string.Empty;
                if (locationTextBox.Text.Length > 0)
                {
                    OnLocationSubmitted(locationTextBox.Text);
                }
            }
        }

        private void PanoTextbox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                panoTextbox.Text = panoTextbox.Text.Trim();
                latLngTextbox.Text = string.Empty;
                locationTextBox.Text = string.Empty;
                if (panoTextbox.Text.Length > 0)
                {
                    OnPanoSubmitted(panoTextbox.Text);
                }
            }
        }

        private void LatLngTextbox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                latLngTextbox.Text = latLngTextbox.Text.Trim();
                panoTextbox.Text = string.Empty;
                locationTextBox.Text = string.Empty;
                if (latLngTextbox.Text.Length > 0
                    && LatLngPoint.TryParse(latLngTextbox.Text, out var point))
                {
                    OnLatLngSubmitted(point);
                }
            }
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private void OnLocationSubmitted(string message)
        {
            LocationSubmitted?.Invoke(this, new StringEventArgs(message));
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private void OnPanoSubmitted(string message)
        {
            PanoSubmitted?.Invoke(this, new StringEventArgs(message));
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private void OnLatLngSubmitted(LatLngPoint point)
        {
            LatLngSubmitted?.Invoke(this, new LatLngPointEventArgs(point));
        }
    }
}