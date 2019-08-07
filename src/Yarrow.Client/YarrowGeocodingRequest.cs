using Juniper.Google.Maps.Geocoding;
using Juniper.HTTP.REST;
using Juniper.World.GIS;

namespace Yarrow.Client
{
    public class YarrowGeocodingRequest : AbstractJsonRequest<GeocodingResponse>
    {
        private LatLngPoint latlng;

        public YarrowGeocodingRequest(YarrowRequestConfiguration api)
            : base(api, "api/geocode", "geocoding")
        {
        }

        public LatLngPoint LatLng
        {
            get { return latlng; }
            set
            {
                latlng = value;
                SetQuery(nameof(latlng), latlng);
            }
        }
    }
}