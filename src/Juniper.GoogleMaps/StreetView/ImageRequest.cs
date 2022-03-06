using System.Globalization;
using System.Net.Http;

namespace Juniper.World.GIS.Google.StreetView
{
    public class ImageRequest : AbstractStreetViewRequest<MediaType.Image>
    {
        private int heading;
        private int pitch;
        private int fov;
        private Size size;

        public ImageRequest(HttpClient http, string apiKey, string signingKey, Size size)
            : base(http, "streetview", apiKey, signingKey, MediaType.Image_Jpeg)
        {
            Size = size;
        }

        public Size Size
        {
            get => size;
            set
            {
                size = value;
                SetQuery(nameof(size), size?.ToString(CultureInfo.InvariantCulture));
            }
        }

        public int Heading
        {
            get => heading;
            set
            {
                heading = value;
                SetQuery(nameof(heading), value);
            }
        }

        public int Pitch
        {
            get => pitch;
            set
            {
                pitch = value;
                SetQuery(nameof(pitch), value);
            }
        }

        public int FOV
        {
            get => fov;
            set
            {
                fov = value;
                SetQuery(nameof(fov), fov);
            }
        }
    }
}