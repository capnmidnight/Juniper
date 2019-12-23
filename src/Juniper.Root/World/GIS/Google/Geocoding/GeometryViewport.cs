using System;
using System.Runtime.Serialization;

namespace Juniper.World.GIS.Google.Geocoding
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Parameter `context` is required by ISerializable interface")]
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