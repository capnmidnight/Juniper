using System;
using System.IO;

using Juniper.HTTP.REST;
using Juniper.Image;
using Juniper.World.GIS;

namespace Juniper.Google.Maps.StreetView
{
    public abstract class AbstractImageRequest : AbstractStreetViewRequest<RawImage>
    {
        private Size size;
        private int radius;
        private bool outdoorOnly;
        private ImageFormat format;
        protected IFactory factory;

        public AbstractImageRequest(PanoID pano, Size size, ImageFormat format)
            : base("streetview", pano)
        {
            Size = size;
            Format = format;
        }

        public AbstractImageRequest(PanoID pano, int width, int height, ImageFormat format)
            : this(pano, new Size(width, height), format)
        {
        }

        public AbstractImageRequest(PlaceName placeName, Size size, ImageFormat format)
            : base("streetview", placeName)
        {
            Size = size;
            Format = format;
        }

        public AbstractImageRequest(PlaceName placeName, int width, int height, ImageFormat format)
            : this(placeName, new Size(width, height), format)
        {
        }

        public AbstractImageRequest(LatLngPoint location, Size size, ImageFormat format)
            : base("streetview", location)
        {
            Size = size;
            Format = format;
        }

        public AbstractImageRequest(LatLngPoint location, int width, int height, ImageFormat format)
            : this(location, new Size(width, height), format)
        {
        }

        public ImageFormat Format
        {
            get { return format; }
            set
            {
                format = value;
                factory = new Image.Consolidated.Factory(format);
                SetContentType(RawImage.GetContentType(format), RawImage.GetExtension(format));
            }
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

        public bool FlipImage { get; set; }

        public override Func<Stream, RawImage> GetDecoder(AbstractEndpoint _)
        {
            return stream => factory.Decode(stream, FlipImage);
        }
    }
}