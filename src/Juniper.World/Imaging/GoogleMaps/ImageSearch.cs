using Juniper.World.GIS;
using System;

namespace Juniper.World.Imaging.GoogleMaps
{
    public class ImageSearch : Search
    {
        private static readonly Uri imageURIBase = new Uri(baseServiceURI);

        public ImageSearch(PanoID pano, int width, int height)
            : base(imageURIBase, "jpeg", pano)
        {
            SetSize(width, height);
        }

        public ImageSearch(string placeName, int width, int height)
            : base(imageURIBase, "jpeg", placeName)
        {
            SetSize(width, height);
        }

        public ImageSearch(LatLngPoint location, int width, int height)
            : base(imageURIBase, "jpeg", location)
        {
            SetSize(width, height);
        }

        private void SetSize(int width, int height)
        {
            uriBuilder.AddQuery("size", $"{width}x{height}");
        }

        public ImageSearch AddHeading(Heading heading)
        {
            uriBuilder.AddQuery("heading", (int)heading);
            return this;
        }

        public ImageSearch AddPitch(Pitch pitch)
        {
            uriBuilder.AddQuery("pitch", (int)pitch);
            return this;
        }

        public ImageSearch AddRadius(int searchRadius)
        {
            uriBuilder.AddQuery("radius", searchRadius);
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
    }
}