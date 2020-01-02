using System;
using System.Net;
using System.Runtime.Serialization;

using Juniper.Google;

namespace Juniper.World.GIS.Google.Geocoding
{
    [Serializable]
    public class GeocodingResponse : ISerializable
    {
        public readonly HttpStatusCode status;
        public readonly GeocodingResult[] results;
        public readonly string error_message;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Parameter `context` is required by ISerializable interface")]
        protected GeocodingResponse(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

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
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

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