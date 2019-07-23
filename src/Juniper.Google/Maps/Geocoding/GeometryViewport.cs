using System;
using System.Runtime.Serialization;
using Juniper.World.GIS;

namespace Juniper.Google.Maps.Geocoding
{
    [Serializable]
    public class GeometryViewport : ISerializable
    {
        public readonly LatLngPoint southwest;
        public readonly LatLngPoint northeast;

        public GeometryViewport(LatLngPoint southwest, LatLngPoint northeast)
        {
            this.southwest = southwest;
            this.northeast = northeast;
        }

        protected GeometryViewport(SerializationInfo info, StreamingContext context)
        {
            southwest = info.GetValue<LatLngPoint>(nameof(southwest));
            northeast = info.GetValue<LatLngPoint>(nameof(northeast));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(southwest), new
            {
                lat = southwest.Latitude,
                lng = southwest.Longitude
            });

            info.AddValue(nameof(northeast), new
            {
                lat = northeast.Latitude,
                lng = northeast.Longitude
            });
        }
    }
}