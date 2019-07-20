using System;
using System.IO;

using Juniper.Image;
using Juniper.World;
using Juniper.World.GIS;

namespace Juniper.Google.Maps.StreetView
{
    public class ImageSearch : AbstractStreetViewSearch<RawImage>
    {
        public ImageSearch(PanoID pano, int width, int height)
            : base("streetview", "image/jpeg", "jpeg", pano)
        {
            SetSize(width, height);
        }

        public ImageSearch(PlaceName placeName, int width, int height)
            : base("streetview", "image/jpeg", "jpeg", placeName)
        {
            SetSize(width, height);
        }

        public ImageSearch(LatLngPoint location, int width, int height)
            : base("streetview", "image/jpeg", "jpeg", location)
        {
            SetSize(width, height);
        }

        public ImageSearch SetSize(int width, int height)
        {
            SetQuery("size", $"{width}x{height}");
            return this;
        }

        public ImageSearch SetHeading(Heading heading)
        {
            SetQuery(nameof(heading), (int)heading);
            return this;
        }

        public ImageSearch SetPitch(Pitch pitch)
        {
            SetQuery(nameof(pitch), (int)pitch);
            return this;
        }

        public ImageSearch SetRadius(int radius)
        {
            SetQuery(nameof(radius), radius);
            return this;
        }

        public ImageSearch SetSource(bool outdoorOnly)
        {
            if (outdoorOnly)
            {
                SetQuery("source", "outdoor");
            }
            return this;
        }

        public bool FlipImage { get; set; }

        internal override Func<Stream, RawImage> GetDecoder(AbstractAPI _)
        {
            return stream => Decoder.DecodeJPEG(FlipImage, stream);
        }
    }
}