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

        public ImageRequest(PanoID pano, Size size)
            : base("streetview", "image/jpeg", "jpeg", pano)
        {
            SetSize(size);
        }

        public ImageRequest(PanoID pano, int width, int height)
            : this(pano, new Size(width, height))
        {
        }

        public ImageRequest(PlaceName placeName, Size size)
            : base("streetview", "image/jpeg", "jpeg", placeName)
        {
            SetSize(size);
        }

        public ImageRequest(PlaceName placeName, int width, int height)
            : this(placeName, new Size(width, height))
        {
        }

        public ImageRequest(LatLngPoint location, Size size)
            : base("streetview", "image/jpeg", "jpeg", location)
        {
            SetSize(size);
        }

        public ImageRequest(LatLngPoint location, int width, int height)
            : this(location, new Size(width, height))
        {
            SetSize(width, height);
        }

        public void SetSize(Size size)
        {
            SetQuery(nameof(size), size);
        }

        public void SetSize(int width, int height)
        {
            SetSize(new Size(width, height));
        }

        public void SetHeading(Heading heading)
        {
            SetQuery(nameof(heading), (int)heading);
        }

        public void SetPitch(Pitch pitch)
        {
            SetQuery(nameof(pitch), (int)pitch);
        }

        public void SetRadius(int radius)
        {
            SetQuery(nameof(radius), radius);
        }

        public void SetSource(bool outdoorOnly)
        {
            if (outdoorOnly)
            {
                SetQuery("source", "outdoor");
            }
        }

        public bool FlipImage { get; set; }

        public override Func<Stream, RawImage> GetDecoder(AbstractEndpoint _)
        {
            return stream => Decoder.DecodeJPEG(FlipImage, stream);
        }
    }
}