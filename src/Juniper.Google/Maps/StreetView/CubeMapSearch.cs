using System;
using System.Linq;
using Juniper.HTTP.REST;
using Juniper.Image;
using Juniper.World;
using Juniper.World.GIS;

namespace Juniper.Google.Maps.StreetView
{
    public class CubeMapSearch : AbstractMultiRequest<RawImage, ImageSearch>
    {
        private CubeMapSearch(Func<ImageSearch> factory)
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

        public CubeMapSearch(PanoID pano, Size size)
            : this(() => new ImageSearch(pano, size))
        {
        }

        public CubeMapSearch(PanoID pano, int width, int height)
            : this(() => new ImageSearch(pano, new Size(width, height)))
        {
        }

        public CubeMapSearch(PlaceName placeName, Size size)
            : this(() => new ImageSearch(placeName, size))
        {
        }

        public CubeMapSearch(PlaceName placeName, int width, int height)
            : this(() => new ImageSearch(placeName, new Size(width, height)))
        {
        }

        public CubeMapSearch(LatLngPoint location, Size size)
            : this(() => new ImageSearch(location, size))
        {
        }

        public CubeMapSearch(LatLngPoint location, int width, int height)
            : this(() => new ImageSearch(location, new Size(width, height)))
        {
        }

        public CubeMapSearch SetRadius(int searchRadius)
        {
            foreach (var search in subSearches)
            {
                search.SetRadius(searchRadius);
            }
            return this;
        }

        public CubeMapSearch SetSource(bool outdoorOnly)
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