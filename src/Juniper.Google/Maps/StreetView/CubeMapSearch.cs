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
            subSearches[0].SetHeading(Heading.North).SetPitch(Pitch.Level);
            subSearches[1].SetHeading(Heading.East).SetPitch(Pitch.Level);
            subSearches[2].SetHeading(Heading.West).SetPitch(Pitch.Level);
            subSearches[3].SetHeading(Heading.South).SetPitch(Pitch.Level);
            subSearches[4].SetHeading(Heading.North).SetPitch(Pitch.Up);
            subSearches[5].SetHeading(Heading.North).SetPitch(Pitch.Down);
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