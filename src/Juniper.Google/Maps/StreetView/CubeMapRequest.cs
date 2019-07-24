using System;
using System.Linq;
using Juniper.HTTP.REST;
using Juniper.Image;
using Juniper.World;
using Juniper.World.GIS;

namespace Juniper.Google.Maps.StreetView
{
    public class CubeMapRequest : AbstractMultiRequest<RawImage, ImageRequest>
    {
        private CubeMapRequest(Func<ImageRequest> factory)
            : base(6, factory)
        {
            subSearches[0].SetHeading(Heading.North);
            subSearches[0].SetPitch(Pitch.Level);

            subSearches[1].SetHeading(Heading.East);
            subSearches[1].SetPitch(Pitch.Level);

            subSearches[2].SetHeading(Heading.West);
            subSearches[2].SetPitch(Pitch.Level);

            subSearches[3].SetHeading(Heading.South);
            subSearches[3].SetPitch(Pitch.Level);

            subSearches[4].SetHeading(Heading.North);
            subSearches[4].SetPitch(Pitch.Up);

            subSearches[5].SetHeading(Heading.North);
            subSearches[5].SetPitch(Pitch.Down);
        }

        public CubeMapRequest(PanoID pano, Size size)
            : this(() => new ImageRequest(pano, size))
        {
        }

        public CubeMapRequest(PanoID pano, int width, int height)
            : this(() => new ImageRequest(pano, new Size(width, height)))
        {
        }

        public CubeMapRequest(PlaceName placeName, Size size)
            : this(() => new ImageRequest(placeName, size))
        {
        }

        public CubeMapRequest(PlaceName placeName, int width, int height)
            : this(() => new ImageRequest(placeName, new Size(width, height)))
        {
        }

        public CubeMapRequest(LatLngPoint location, Size size)
            : this(() => new ImageRequest(location, size))
        {
        }

        public CubeMapRequest(LatLngPoint location, int width, int height)
            : this(() => new ImageRequest(location, new Size(width, height)))
        {
        }

        public CubeMapRequest SetRadius(int searchRadius)
        {
            foreach (var search in subSearches)
            {
                search.SetRadius(searchRadius);
            }
            return this;
        }

        public CubeMapRequest SetSource(bool outdoorOnly)
        {
            foreach (var search in subSearches)
            {
                search.SetSource(outdoorOnly);
            }
            return this;
        }

        public bool FlipImages
        {
            get
            {
                return subSearches.Any(search => search.FlipImage);
            }
            set
            {
                foreach (var search in subSearches)
                {
                    search.FlipImage = value;
                }
            }
        }
    }
}