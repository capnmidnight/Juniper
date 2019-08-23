using System;

using Juniper.Json;
using Juniper.Serialization;
using Juniper.World.GIS;

namespace Juniper.Google.Maps.TimeZone
{
    public class TimeZoneRequest : AbstractGoogleMapsRequest<IDeserializer<TimeZoneResponse>, TimeZoneResponse>
    {
        private LatLngPoint location;
        private DateTime timestamp;

        public TimeZoneRequest(GoogleMapsRequestConfiguration api)
            : base(api, new JsonFactory().Specialize<TimeZoneResponse>(), "timezone/json", "timezones", false)
        {
            SetContentType(HTTP.MediaType.Application.Json);
        }

        public LatLngPoint Location
        {
            get { return location; }
            set
            {
                location = value;
                SetQuery(nameof(location), location);
            }
        }

        public DateTime Timestamp
        {
            get { return timestamp; }
            set
            {
                timestamp = value;
                var offset = new DateTimeOffset(timestamp);
                SetQuery(nameof(timestamp), offset.ToUnixTimeSeconds());
            }
        }
    }
}