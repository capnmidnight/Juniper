using System;
using System.Linq;
using Juniper.Image;
using Juniper.World;
using Juniper.World.GIS;

namespace Juniper.Google.Maps.StreetView
{
    public class CubeMapSearch : AbstractMultiSearch<RawImage, ImageSearch>
    {
        private CubeMapSearch(Func<ImageSearch> factory)
            : base(6, factory)
        {
            subSearches[0].AddHeading(Heading.North).AddPitch(Pitch.Level);
            subSearches[1].AddHeading(Heading.East).AddPitch(Pitch.Level);
            subSearches[2].AddHeading(Heading.West).AddPitch(Pitch.Level);
            subSearches[3].AddHeading(Heading.South).AddPitch(Pitch.Level);
            subSearches[4].AddHeading(Heading.North).AddPitch(Pitch.Up);
            subSearches[5].AddHeading(Heading.North).AddPitch(Pitch.Down);
        }

        public CubeMapSearch(PanoID pano, int width, int height)
            : this(() => new ImageSearch(pano, width, height))
        {
        }

        public CubeMapSearch(PlaceName placeName, int width, int height)
            : this(() => new ImageSearch(placeName, width, height))
        {
        }

        public CubeMapSearch(LatLngPoint location, int width, int height)
            : this(() => new ImageSearch(location, width, height))
        {
        }

        public CubeMapSearch AddRadius(int searchRadius)
        {
            foreach (var search in subSearches)
            {
                search.AddRadius(searchRadius);
            }
            return this;
        }

        public CubeMapSearch AddSource(bool outdoorOnly)
        {
            foreach (var search in subSearches)
            {
                search.AddSource(outdoorOnly);
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
                foreach(var search in subSearches)
                {
                    search.FlipImage = value;
                }
            }
        }
    }
}