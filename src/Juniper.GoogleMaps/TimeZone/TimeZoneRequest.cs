using System;

namespace Juniper.World.GIS.Google.TimeZone
{
    public class TimeZoneRequest : AbstractGoogleMapsRequest<MediaType.Application>
    {
        private LatLngPoint location;
        private DateTime timestamp;

        public TimeZoneRequest(string apiKey)
            : base("timezone/json", Juniper.MediaType.Application.Json, apiKey, null)
        { }

        public LatLngPoint Location
        {
            get => location;
            set
            {
                location = value;
                SetQuery(nameof(location), location);
            }
        }

        public DateTime Timestamp
        {
            get => timestamp;
            set
            {
                timestamp = value;
                var offset = new DateTimeOffset(timestamp);
                SetQuery(nameof(timestamp), offset.ToUnixTimeSeconds());
            }
        }
    }
}