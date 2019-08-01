using Juniper.HTTP.REST;
using Juniper.Image;
using Juniper.World.GIS;

namespace Juniper.Google.Maps.StreetView
{
    public abstract class AbstractStreetViewImageRequest : AbstractStreetViewRequest<ImageData>
    {
        private Size size;
        private int radius;
        private bool outdoorOnly;

        public AbstractStreetViewImageRequest(AbstractEndpoint api, PanoID pano, Size size)
            : base(api, new Image.JPEG.Factory(), "streetview", pano)
        {
            Size = size;
            SetContentType(ImageData.GetContentType(ImageFormat.JPEG), ImageData.GetExtension(ImageFormat.JPEG));
        }

        public AbstractStreetViewImageRequest(AbstractEndpoint api, PanoID pano, int width, int height)
            : this(api, pano, new Size(width, height))
        {
        }

        public AbstractStreetViewImageRequest(AbstractEndpoint api, PlaceName placeName, Size size)
            : base(api, new Image.JPEG.Factory(), "streetview", placeName)
        {
            Size = size;
            SetContentType(ImageData.GetContentType(ImageFormat.JPEG), ImageData.GetExtension(ImageFormat.JPEG));
        }

        public AbstractStreetViewImageRequest(AbstractEndpoint api, PlaceName placeName, int width, int height)
            : this(api, placeName, new Size(width, height))
        {
        }

        public AbstractStreetViewImageRequest(AbstractEndpoint api, LatLngPoint location, Size size)
            : base(api, new Image.JPEG.Factory(), "streetview", location)
        {
            Size = size;
            SetContentType(ImageData.GetContentType(ImageFormat.JPEG), ImageData.GetExtension(ImageFormat.JPEG));
        }

        public AbstractStreetViewImageRequest(AbstractEndpoint api, LatLngPoint location, int width, int height)
            : this(api, location, new Size(width, height))
        {
        }

        public virtual Size Size
        {
            get { return size; }
            set { size = SetQuery(nameof(size), value); }
        }

        public void SetSize(int width, int height)
        {
            Size = new Size(width, height);
        }

        public int Radius
        {
            get { return radius; }
            set { radius = SetQuery(nameof(radius), value); }
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