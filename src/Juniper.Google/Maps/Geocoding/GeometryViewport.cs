using System;
using System.Runtime.Serialization;
using Juniper.World.GIS;

namespace Juniper.Google.Maps.Geocoding
{
    [Serializable]
    public class GeometryViewport : ISerializable
    {
        public readonly LatLngPoint northeast;
        public readonly LatLngPoint southwest;

        protected GeometryViewport(SerializationInfo info, StreamingContext context)
        {
            northeast = info.GetValue<LatLngPoint>(nameof(northeast));
            southwest = info.GetValue<LatLngPoint>(nameof(southwest));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(northeast), new
            {
                lat = northeast.Latitude,
                lng = northeast.Longitude
            });

            info.AddValue(nameof(southwest), new
            {
                lat = southwest.Latitude,
                lng = southwest.Longitude
            });
        }
    }
}
