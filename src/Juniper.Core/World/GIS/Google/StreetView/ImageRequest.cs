using Juniper.HTTP;
using Juniper.Imaging;

namespace Juniper.GIS.Google.StreetView
{
    public class ImageRequest : AbstractStreetViewRequest<MediaType.Image>
    {
        private int heading;
        private int pitch;
        private int fov;
        private Size size;

        public ImageRequest(string apiKey, string signingKey, Size size)
            : base("streetview", apiKey, signingKey, MediaType.Image.Jpeg)
        {
            Size = size;
        }

        public Size Size
        {
            get { return size; }
            set
            {
                size = value;
                SetQuery(nameof(size), size.ToString());
            }
        }

        public int Heading
        {
            get { return heading; }
            set
            {
                heading = value;
                SetQuery(nameof(heading), value);
            }
        }

        public int Pitch
        {
            get { return pitch; }
            set
            {
                pitch = value;
                SetQuery(nameof(pitch), value);
            }
        }

        public int FOV
        {
            get { return fov; }
            set
            {
                fov = value;
                SetQuery(nameof(fov), fov);
            }
        }
    }
}