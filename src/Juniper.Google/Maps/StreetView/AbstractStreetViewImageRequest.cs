using Juniper.Imaging;
using Juniper.World.GIS;

namespace Juniper.Google.Maps.StreetView
{
    public abstract class AbstractStreetViewImageRequest<T> : AbstractStreetViewRequest<T>
    {
        private Size size;
        private int radius;
        private bool outdoorOnly;

        protected AbstractStreetViewImageRequest(GoogleMapsRequestConfiguration api, IImageDecoder<T> decoder, Size size)
            : base(api, decoder, "streetview")
        {
            Size = size;
            SetContentType(ImageData.GetContentType(ImageFormat.JPEG), ImageData.GetExtension(ImageFormat.JPEG));
        }

        protected AbstractStreetViewImageRequest(GoogleMapsRequestConfiguration api, IImageDecoder<T> decoder, Size size, PanoID pano)
            : this(api, decoder, size)
        {
            Pano = pano;
        }

        protected AbstractStreetViewImageRequest(GoogleMapsRequestConfiguration api, IImageDecoder<T> decoder, Size size, PlaceName placeName)
            : this(api, decoder, size)
        {
            Place = placeName;
        }

        protected AbstractStreetViewImageRequest(GoogleMapsRequestConfiguration api, IImageDecoder<T> decoder, Size size, LatLngPoint location)
            : this(api, decoder, size)
        {
            Location = location;
        }

        public Size Size
        {
            get { return size; }
            set
            {
                size = value;
                SetQuery(nameof(size), value);
            }
        }

        public int Radius
        {
            get { return radius; }
            set
            {
                radius = value;
                SetQuery(nameof(radius), value);
            }
        }

        public bool OutdoorOnly
        {
            get { return outdoorOnly; }
            set
            {
                outdoorOnly = value;
                if (outdoorOnly)
                {
                    SetQuery("source", "outdoor");
                }
                else
                {
                    RemoveQuery("source");
                }
            }
        }
    }
}