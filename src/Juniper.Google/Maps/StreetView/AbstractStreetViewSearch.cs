using System;

using Juniper.World.GIS;

namespace Juniper.Google.Maps.StreetView
{
    public abstract class AbstractStreetViewSearch : AbstractMapsSearch
    {
        internal readonly string locString;

        private AbstractStreetViewSearch(string path, string extension, string locString)
            : base(path, extension)
        {
            this.locString = locString;
            uriBuilder.AddQuery(locString);
        }

        protected AbstractStreetViewSearch(string path, string extension, PanoID pano)
            : this(path, extension, $"pano={pano}") { }

        protected AbstractStreetViewSearch(string path, string extension, PlaceName placeName)
            : this(path, extension, $"location={placeName}") { }

        protected AbstractStreetViewSearch(string path, string extension, LatLngPoint location)
            : this(path, extension, $"location={location.Latitude},{location.Longitude}") { }
    }
}