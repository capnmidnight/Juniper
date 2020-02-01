using System;
using System.Runtime.Serialization;

namespace Juniper.World.GIS.Google.Geocoding
{
    [Serializable]
    public class GeometryViewport : ISerializable
    {
        private static readonly string SOUTHWEST_FIELD = nameof(SouthWest).ToLowerInvariant();
        private static readonly string NORTHEAST_FIELD = nameof(NorthEast).ToLowerInvariant();

        public LatLngPoint SouthWest { get; }
        public LatLngPoint NorthEast { get; }

        public GeometryViewport(LatLngPoint southwest, LatLngPoint northeast)
        {
            SouthWest = southwest;
            NorthEast = northeast;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Parameter `context` is required by ISerializable interface")]
        protected GeometryViewport(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            SouthWest = info.GetValue<LatLngPoint>(SOUTHWEST_FIELD);
            NorthEast = info.GetValue<LatLngPoint>(NORTHEAST_FIELD);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            info.AddValue(SOUTHWEST_FIELD, new
            {
                lat = SouthWest.Latitude,
                lng = SouthWest.Longitude
            });

            info.AddValue(NORTHEAST_FIELD, new
            {
                lat = NorthEast.Latitude,
                lng = NorthEast.Longitude
            });
        }
    }
}