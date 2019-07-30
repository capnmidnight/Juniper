using Juniper.Image;
using Juniper.World.GIS;

namespace Juniper.Google.Maps.StreetView
{
    public abstract class AbstractImageRequest : AbstractStreetViewRequest<RawImage>
    {
        private Size size;
        private int radius;
        private bool outdoorOnly;

        public AbstractImageRequest(PanoID pano, Size size)
            : base(new Image.JPEG.Factory(), "streetview", pano)
        {
            Size = size;
            SetContentType(RawImage.GetContentType(ImageFormat.JPEG), RawImage.GetExtension(ImageFormat.JPEG));
        }

        public AbstractImageRequest(PanoID pano, int width, int height)
            : this(pano, new Size(width, height))
        {
        }

        public AbstractImageRequest(PlaceName placeName, Size size)
            : base(new Image.JPEG.Factory(), "streetview", placeName)
        {
            Size = size;
            SetContentType(RawImage.GetContentType(ImageFormat.JPEG), RawImage.GetExtension(ImageFormat.JPEG));
        }

        public AbstractImageRequest(PlaceName placeName, int width, int height)
            : this(placeName, new Size(width, height))
        {
        }

        public AbstractImageRequest(LatLngPoint location, Size size)
            : base(new Image.JPEG.Factory(), "streetview", location)
        {
            Size = size;
            SetContentType(RawImage.GetContentType(ImageFormat.JPEG), RawImage.GetExtension(ImageFormat.JPEG));
        }

        public AbstractImageRequest(LatLngPoint location, int width, int height)
            : this(location, new Size(width, height))
        {
        }

        public Size Size
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