using System;
using System.Collections.Generic;
using System.IO;
using Juniper.HTTP.REST;
using Juniper.World.GIS;

namespace Juniper.Google.Maps.TimeZone
{
    public class TimeZoneRequest : AbstractMapsRequest<TimeZoneResponse>
    {
        public TimeZoneRequest(LatLngPoint location, DateTime timestamp)
            : base("timezone/json", "timezones", "application/json", "json", false)
        {
            SetQuery(nameof(location), location);

            var offset = new DateTimeOffset(timestamp);
            SetQuery(nameof(timestamp), offset.ToUnixTimeSeconds());
        }

        public override Func<Stream, TimeZoneResponse> GetDecoder(AbstractEndpoint api)
        {
            return api.DecodeObject<TimeZoneResponse>;
        }
    }
}