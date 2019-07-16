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
            : base("streetview", "jpeg", pano)
        {
            SetSize(width, height);
        }

        public ImageSearch(PlaceName placeName, int width, int height)
            : base("streetview", "jpeg", placeName)
        {
            SetSize(width, height);
        }

        public ImageSearch(LatLngPoint location, int width, int height)
            : base("streetview", "jpeg", location)
        {
            SetSize(width, height);
        }

        public ImageSearch SetSize(int width, int height)
        {
            return AddSize($"{width}x{height}");
        }

        private ImageSearch AddSize(string size)
        {
            uriBuilder.AddQuery(nameof(size), size);
            return this;
        }

        public ImageSearch AddHeading(Heading heading)
        {
            uriBuilder.AddQuery(nameof(heading), (int)heading);
            return this;
        }

        public ImageSearch AddPitch(Pitch pitch)
        {
            uriBuilder.AddQuery(nameof(pitch), (int)pitch);
            return this;
        }

        public ImageSearch AddRadius(int radius)
        {
            uriBuilder.AddQuery(nameof(radius), radius);
            return this;
        }

        public ImageSearch AddSource(bool outdoorOnly)
        {
            if (outdoorOnly)
            {
                uriBuilder.AddQuery("source=outdoor");
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