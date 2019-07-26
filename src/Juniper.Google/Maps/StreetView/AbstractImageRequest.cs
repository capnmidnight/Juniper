using System;
using System.IO;
using Juniper.HTTP.REST;
using Juniper.Image;
using Juniper.World;
using Juniper.World.GIS;

namespace Juniper.Google.Maps.StreetView
{
    public abstract class AbstractImageRequest : AbstractStreetViewRequest<RawImage>
    {
        private Size size;
        private int radius;
        private bool outdoorOnly;

        public AbstractImageRequest(PanoID pano, Size size)
            : base("streetview", "image/jpeg", "jpeg", pano)
        {
            Size = size;
        }

        public AbstractImageRequest(PanoID pano, int width, int height)
            : this(pano, new Size(width, height))
        {
        }

        public AbstractImageRequest(PlaceName placeName, Size size)
            : base("streetview", "image/jpeg", "jpeg", placeName)
        {
            Size = size;
        }

        public AbstractImageRequest(PlaceName placeName, int width, int height)
            : this(placeName, new Size(width, height))
        {
        }

        public AbstractImageRequest(LatLngPoint location, Size size)
            : base("streetview", "image/jpeg", "jpeg", location)
        {
            Size = size;
        }

        public AbstractImageRequest(LatLngPoint location, int width, int height)
            : this(location, new Size(width, height))
        {
            SetSize(width, height);
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
            return stream => Decoder.DecodeJPEG(stream, FlipImage);
        }
    }
}