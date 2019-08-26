using System;

using Juniper.World.GIS;

namespace Juniper.Google.Maps.TimeZone
{
    public class TimeZoneRequest : AbstractGoogleMapsRequest
    {
        private LatLngPoint location;
        private DateTime timestamp;

        public TimeZoneRequest(GoogleMapsRequestConfiguration api)
            : base(api, "timezone/json", "timezones", false) { }

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