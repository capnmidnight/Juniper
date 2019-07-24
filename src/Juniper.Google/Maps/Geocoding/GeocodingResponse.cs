using System;
using System.Net;
using System.Runtime.Serialization;

namespace Juniper.Google.Maps.Geocoding
{
    [Serializable]
    public class GeocodingResponse : ISerializable
    {
        public readonly HttpStatusCode status;
        public readonly GeocodingResult[] results;
        public readonly string error_message;

        protected GeocodingResponse(SerializationInfo info, StreamingContext context)
        {
            status = info.GetString(nameof(status)).MapToStatusCode();
            if (status == HttpStatusCode.OK)
            {
                results = info.GetValue<GeocodingResult[]>(nameof(results));
            }
            else if (status != HttpStatusCode.NoContent)
            {
                error_message = info.GetString(nameof(error_message));
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(status), status.ToGoogleString());
            if (status == HttpStatusCode.OK)
            {
                info.AddValue(nameof(results), results);
            }
            else if (status != HttpStatusCode.NoContent)
            {
                info.AddValue(nameof(error_message), error_message);
            }
        }
    }
}