using Juniper.World.GIS;

namespace Juniper.Google.Maps.StreetView
{
    public abstract class AbstractStreetViewRequest<ResultType> : AbstractMapsRequest<ResultType>
    {
        private AbstractStreetViewRequest(string path, string acceptType, string extension, string locationKey, string locationValue)
            : base(path, $"{locationKey}={locationValue}", acceptType, extension, true)
        {
            SetQuery(locationKey, locationValue);
        }

        protected AbstractStreetViewRequest(string path, string acceptType, string extension, PanoID pano)
            : this(path, acceptType, extension, "pano", pano.ToString()) { }

        protected AbstractStreetViewRequest(string path, string acceptType, string extension, PlaceName placeName)
            : this(path, acceptType, extension, "location", placeName.ToString()) { }

        protected AbstractStreetViewRequest(string path, string acceptType, string extension, LatLngPoint location)
            : this(path, acceptType, extension, "location", location.ToCSV()) { }
    }
}