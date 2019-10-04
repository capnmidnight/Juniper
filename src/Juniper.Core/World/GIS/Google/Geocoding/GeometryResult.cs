using System;
using System.Runtime.Serialization;

using Juniper.World.GIS;

namespace Juniper.GIS.Google.Geocoding
{
    [Serializable]
    public class GeometryResult : ISerializable
    {
        public readonly LatLngPoint location;
        public readonly string locationTypeString;
        public readonly GeometryLocationType location_type;
        public readonly GeometryViewport viewport;
        public readonly GeometryViewport bounds;

        protected GeometryResult(SerializationInfo info, StreamingContext context)
        {
            location = info.GetValue<LatLngPoint>(nameof(location));
            locationTypeString = info.GetString(nameof(location_type));
            location_type = Enum.TryParse<GeometryLocationType>(locationTypeString, out var type)
                ? type : GeometryLocationType.Unknown;
            viewport = info.GetValue<GeometryViewport>(nameof(viewport));
            foreach (var field in info)
            {
                switch (field.Name)
                {
                    case nameof(bounds): bounds = info.GetValue<GeometryViewport>(field.Name); break;
                }
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
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