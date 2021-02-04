using System;
using System.Runtime.Serialization;

namespace Juniper.World.GIS.Google.Geocoding
{
    [Serializable]
    public class GeometryResult : ISerializable
    {
        private static readonly string LOCATION_FIELD = nameof(Location).ToLowerInvariant();
        private static readonly string LOCATION_TYPE_FIELD = nameof(Location_Type).ToLowerInvariant();
        private static readonly string VIEWPORT_FIELD = nameof(Viewport).ToLowerInvariant();

        public LatLngPoint Location { get; }

        public string LocationTypeString { get; }

        public GeometryLocationType Location_Type { get; }

        public GeometryViewport Viewport { get; }

        public GeometryViewport Bounds { get; }

        protected GeometryResult(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            Location = info.GetValue<LatLngPoint>(LOCATION_FIELD);
            LocationTypeString = info.GetString(LOCATION_TYPE_FIELD);
            Location_Type = Enum.TryParse<GeometryLocationType>(LocationTypeString, out var type)
                ? type
                : GeometryLocationType.Unknown;
            Viewport = info.GetValue<GeometryViewport>(VIEWPORT_FIELD);
            foreach (var field in info)
            {
                switch (field.Name)
                {
                    case nameof(Bounds):
                    Bounds = info.GetValue<GeometryViewport>(field.Name);
                    break;
                }
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            info.AddValue(LOCATION_FIELD, new
            {
                lat = Location.Lat,
                lng = Location.Lng
            });
            info.AddValue(LOCATION_TYPE_FIELD, Location_Type.ToString());
            info.AddValue(VIEWPORT_FIELD, Viewport);
        }
    }
}