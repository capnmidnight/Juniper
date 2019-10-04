using System;
using System.Net;
using System.Runtime.Serialization;

using Juniper.Google;

namespace Juniper.GIS.Google.TimeZone
{
    [Serializable]
    public class TimeZoneResponse : ISerializable
    {
        public readonly HttpStatusCode status;
        public readonly string errorMessage;

        public readonly long dstOffset;
        public readonly long rawOffset;
        public readonly string timeZoneId;
        public readonly string timeZoneName;

        protected TimeZoneResponse(SerializationInfo info, StreamingContext context)
        {
            status = info.GetString(nameof(status)).MapToStatusCode();
            if (status == HttpStatusCode.OK)
            {
                dstOffset = info.GetInt64(nameof(dstOffset));
                rawOffset = info.GetInt64(nameof(rawOffset));
                timeZoneId = info.GetString(nameof(timeZoneId));
                timeZoneName = info.GetString(nameof(timeZoneName));
            }
            else if (status != HttpStatusCode.NoContent)
            {
                errorMessage = info.GetString(nameof(errorMessage));
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(status), status.ToGoogleString());
            if (status == HttpStatusCode.OK)
            {
                info.AddValue(nameof(dstOffset), dstOffset);
                info.AddValue(nameof(rawOffset), rawOffset);
                info.AddValue(nameof(timeZoneId), timeZoneId);
                info.AddValue(nameof(timeZoneName), timeZoneName);
            }
            else if (status != HttpStatusCode.NoContent)
            {
                info.AddValue(nameof(errorMessage), errorMessage);
            }
        }
    }
}