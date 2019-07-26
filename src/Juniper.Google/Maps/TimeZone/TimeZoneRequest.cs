using System;
using System.IO;

using Juniper.HTTP.REST;
using Juniper.World.GIS;

namespace Juniper.Google.Maps.TimeZone
{
    public class TimeZoneRequest : AbstractMapsRequest<TimeZoneResponse>
    {
        private LatLngPoint location;
        private DateTime timestamp;

        public TimeZoneRequest(LatLngPoint location, DateTime timestamp)
            : base("timezone/json", "timezones", false)
        {
            Location = location;
            Timestamp = timestamp;
        }

        public override Func<Stream, TimeZoneResponse> GetDecoder(AbstractEndpoint api)
        {
            return api.DecodeObject<TimeZoneResponse>;
        }

        public LatLngPoint Location
        {
            get { return location; }
            set { location = SetQuery(nameof(location), value); }
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