using Juniper.Imaging;

namespace Juniper.Google.Maps.StreetView
{
    public class ImageRequest<T> : AbstractStreetViewRequest<IImageDecoder<T>, T>
    {
        private int heading;
        private int pitch;
        private int fov;
        private Size size;

        public ImageRequest(GoogleMapsRequestConfiguration api, IImageDecoder<T> decoder, Size size)
            : base(api, decoder, "streetview")
        {
            Size = size;
            var format = decoder.Format;
            var acceptType = ImageData.GetContentType(format);
            var extension = ImageData.GetExtension(format);
            SetContentType(acceptType, extension);
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