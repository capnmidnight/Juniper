using System;

using Juniper.World;
using Juniper.World.GIS;

namespace Juniper.Google.Maps.StreetView
{
    public class CubeMapSearch
    {
        private static ImageSearch[] Make(Func<ImageSearch> factory)
        {
            var searches = new ImageSearch[6];
            for (var i = 0; i < searches.Length; ++i)
            {
                searches[i] = factory();
            }

            return searches;
        }

        private CubeMapSearch(ImageSearch[] images)
        {
            SubSearches = images;

            images[0].AddHeading(Heading.North).AddPitch(Pitch.Level);
            images[1].AddHeading(Heading.East).AddPitch(Pitch.Level);
            images[2].AddHeading(Heading.West).AddPitch(Pitch.Level);
            images[3].AddHeading(Heading.South).AddPitch(Pitch.Level);
            images[4].AddHeading(Heading.North).AddPitch(Pitch.Up);
            images[5].AddHeading(Heading.North).AddPitch(Pitch.Down);
        }

        public CubeMapSearch(PanoID pano, int width, int height)
            : this(Make(() => new ImageSearch(pano, width, height)))
        {
        }

        public CubeMapSearch(PlaceName placeName, int width, int height)
            : this(Make(() => new ImageSearch(placeName, width, height)))
        {
        }

        public CubeMapSearch(LatLngPoint location, int width, int height)
            : this(Make(() => new ImageSearch(location, width, height)))
        {
        }

        public CubeMapSearch AddRadius(int searchRadius)
        {
            foreach (var search in SubSearches)
            {
                search.AddRadius(searchRadius);
            }
            return this;
        }

        public CubeMapSearch AddSource(bool outdoorOnly)
        {
            foreach (var search in SubSearches)
            {
                search.AddSource(outdoorOnly);
            }
            return this;
        }

        internal ImageSearch[] SubSearches { get; }
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