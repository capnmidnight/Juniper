using System;
using System.IO;
using Juniper.HTTP.REST;
using Juniper.Image;
using Juniper.World;
using Juniper.World.GIS;

namespace Juniper.Google.Maps.StreetView
{
    public class ImageRequest : AbstractStreetViewRequest<RawImage>
    {
        public static ImageRequest Create(LocationTypes locationType, object value, Size size)
        {
            switch (locationType)
            {
                case LocationTypes.PanoID: return new ImageRequest((PanoID)value, size);
                case LocationTypes.PlaceName: return new ImageRequest((PlaceName)value, size);
                case LocationTypes.LatLngPoint: return new ImageRequest((LatLngPoint)value, size);
                default: return default;
            }
        }

        public static ImageRequest Create(LocationTypes locationType, object value, int width, int height)
        {
            return Create(locationType, value, new Size(width, height));
        }

        private Size size;
        private Heading heading;
        private Pitch pitch;
        private int radius;
        private bool outdoorOnly;

        public ImageRequest(PanoID pano, Size size)
            : base("streetview", "image/jpeg", "jpeg", pano)
        {
            Size = size;
        }

        public ImageRequest(PanoID pano, int width, int height)
            : this(pano, new Size(width, height))
        {
        }

        public ImageRequest(PlaceName placeName, Size size)
            : base("streetview", "image/jpeg", "jpeg", placeName)
        {
            Size = size;
        }

        public ImageRequest(PlaceName placeName, int width, int height)
            : this(placeName, new Size(width, height))
        {
        }

        public ImageRequest(LatLngPoint location, Size size)
            : base("streetview", "image/jpeg", "jpeg", location)
        {
            Size = size;
        }

        public ImageRequest(LatLngPoint location, int width, int height)
            : this(location, new Size(width, height))
        {
            SetSize(width, height);
        }

        public Size Size
        {
            get
            {
                return size;
            }
            set
            {
                size = SetQuery(nameof(size), value);
            }
        }

        public void SetSize(int width, int height)
        {
            Size = new Size(width, height);
        }

        public Heading Heading
        {
            get { return heading; }
            set
            {
                heading = value;
                SetQuery(nameof(heading), (int)heading);
            }
        }

        public Pitch Pitch
        {
            get { return pitch; }
            set { SetPitch(value); }
        }

        public void SetPitch(Pitch pitch)
        {
            this.pitch = pitch;
            SetQuery(nameof(pitch), (int)pitch);
        }

        public int Radius
        {
            get { return radius; }
            set
            {
                radius = SetQuery(nameof(radius), value);
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

        public bool FlipImage { get; set; }

        public override Func<Stream, RawImage> GetDecoder(AbstractEndpoint _)
        {
            return stream => Decoder.DecodeJPEG(FlipImage, stream);
        }
    }
}