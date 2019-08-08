using Juniper.Google.Maps;
using Juniper.Google.Maps.StreetView;
using Juniper.HTTP.REST;
using Juniper.World.GIS;

namespace Yarrow.Client
{
    public class YarrowMetadataRequest : AbstractJsonRequest<MetadataResponse>
    {
        private PlaceName location;
        private LatLngPoint latlng;
        private PanoID pano;

        public YarrowMetadataRequest(YarrowRequestConfiguration api)
            : base(api, "api/metadata", "metadata")
        {
        }

        public PlaceName Place
        {
            get { return location; }
            set
            {
                location = value;
                SetQuery(nameof(location), location);

                latlng = default;
                RemoveQuery(nameof(latlng));

                pano = default;
                RemoveQuery(nameof(pano));
            }
        }

        public LatLngPoint Location
        {
            get { return latlng; }
            set
            {
                location = default;
                RemoveQuery(nameof(location));

                latlng = value;
                SetQuery(nameof(latlng), latlng);

                pano = default;
                RemoveQuery(nameof(pano));
            }
        }

        public PanoID Pano
        {
            get { return pano; }
            set
            {
                location = default;
                RemoveQuery(nameof(location));

                latlng = default;
                RemoveQuery(nameof(latlng));

                pano = value;
                SetQuery(nameof(pano), pano);
            }
        }
    }
}