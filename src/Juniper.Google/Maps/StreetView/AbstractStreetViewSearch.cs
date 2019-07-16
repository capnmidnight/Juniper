using System;

using Juniper.World.GIS;

namespace Juniper.Google.Maps.StreetView
{
    public abstract class AbstractStreetViewSearch<T> : AbstractMapsSearch<T>
    {
        private AbstractStreetViewSearch(string path, string cacheLocString, string extension)
            : base(path, cacheLocString, extension)
        {
            uriBuilder.AddQuery(cacheLocString);
        }

        protected AbstractStreetViewSearch(string path, string extension, PanoID pano)
            : this(path, $"pano={pano}", extension) { }

        protected AbstractStreetViewSearch(string path, string extension, PlaceName placeName)
            : this(path, $"location={placeName}", extension) { }

        protected AbstractStreetViewSearch(string path, string extension, LatLngPoint location)
            : this(path, $"location={location.Latitude},{location.Longitude}", extension) { }
    }
}