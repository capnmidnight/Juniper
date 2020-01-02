using System;
using System.Runtime.Serialization;

namespace Juniper.World.GIS.Google.Geocoding
{
    [Serializable]
    public class GeometryResult : ISerializable
    {
        public readonly LatLngPoint location;
        public readonly string locationTypeString;
        public readonly GeometryLocationType location_type;
        public readonly GeometryViewport viewport;
        public readonly GeometryViewport bounds;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Parameter `context` is required by ISerializable interface")]
        protected GeometryResult(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            location = info.GetValue<LatLngPoint>(nameof(location));
            locationTypeString = info.GetString(nameof(location_type));
            location_type = Enum.TryParse<GeometryLocationType>(locationTypeString, out var type)
                ? type
                : GeometryLocationType.Unknown;
            viewport = info.GetValue<GeometryViewport>(nameof(viewport));
            foreach (var field in info)
            {
                switch (field.Name)
                {
                    case nameof(bounds):
                    bounds = info.GetValue<GeometryViewport>(field.Name);
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

            info.AddValue(nameof(location), new
            {
                lat = location.Latitude,
                lng = location.Longitude
            });
            info.AddValue(nameof(location_type), location_type.ToString());
            info.AddValue(nameof(viewport), viewport);
        }
    }
}