using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;

using Juniper.Serialization;
using Juniper.World.GIS;

namespace Juniper.Google.Maps.StreetView
{
    [Serializable]
    public class Metadata : ISerializable
    {
        private static readonly Dictionary<string, HttpStatusCode> CODE_MAP = new Dictionary<string, HttpStatusCode>
        {
            { "OK", HttpStatusCode.OK },
            { "ZERO_RESULTS", HttpStatusCode.NoContent },
            { "NOT_FOUND", HttpStatusCode.NotFound },
            { "OVER_QUERY_LIMIT", (HttpStatusCode)429 },
            { "REQUEST_DENIED", HttpStatusCode.Forbidden },
            { "INVALID_REQUEST", HttpStatusCode.BadRequest },
            { "UNKOWN_ERROR", HttpStatusCode.InternalServerError }
        };

        private static HttpStatusCode MapStatus(string code)
        {
            return CODE_MAP.ContainsKey(code)
                ? CODE_MAP[code]
                : HttpStatusCode.InternalServerError;
        }

        public readonly HttpStatusCode status;
        public readonly string error_message;
        public readonly string copyright;
        public readonly string date;
        public readonly PanoID pano_id;
        public readonly LatLngPoint location;

        protected Metadata(SerializationInfo info, StreamingContext context)
        {
            status = MapStatus(info.GetString(nameof(status)));
            if (status == HttpStatusCode.OK)
            {
                copyright = info.GetString(nameof(copyright));
                date = info.GetString(nameof(date));
                pano_id = new PanoID(info.GetString(nameof(pano_id)));
                location = info.GetValue<LatLngPoint>(nameof(location));
            }
            else
            {
                error_message = info.GetString(nameof(error_message));
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(status), status.ToString());
            info.MaybeAddValue(nameof(error_message), error_message);
            info.MaybeAddValue(nameof(copyright), copyright);
            info.MaybeAddValue(nameof(date), date);
            info.MaybeAddValue(nameof(pano_id), pano_id.ToString());
            info.MaybeAddValue(nameof(location), new
            {
                lat = location.Latitude,
                lng = location.Longitude
            });
        }
    }
}