using System;
using System.Net;
using System.Runtime.Serialization;

using Juniper.Google;

namespace Juniper.World.GIS.Google.TimeZone
{
    [Serializable]
    public class TimeZoneResponse : ISerializable
    {
        private static readonly string STATUS_FIELD = nameof(Status).ToLowerInvariant();
        private static readonly string DST_OFFSET_FIELD = nameof(DstOffset).ToLowerInvariant();
        private static readonly string RAW_OFFSET_FIELD = nameof(RawOffset).ToLowerInvariant();
        private static readonly string TIME_ZONE_ID_FIELD = nameof(TimeZoneId).ToLowerInvariant();
        private static readonly string TIME_ZONE_NAME_FIELD = nameof(TimeZoneName).ToLowerInvariant();
        private static readonly string ERROR_MESSAGE_FIELD = nameof(ErrorMessage).ToLowerInvariant();

        public HttpStatusCode Status { get; }

        public string ErrorMessage { get; }

        public long DstOffset { get; }

        public long RawOffset { get; }

        public string TimeZoneId { get; }

        public string TimeZoneName { get; }

        protected TimeZoneResponse(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            Status = info.GetString(STATUS_FIELD).MapToStatusCode();

            if (Status == HttpStatusCode.OK)
            {
                DstOffset = info.GetInt64(DST_OFFSET_FIELD);
                RawOffset = info.GetInt64(RAW_OFFSET_FIELD);
                TimeZoneId = info.GetString(TIME_ZONE_ID_FIELD);
                TimeZoneName = info.GetString(TIME_ZONE_NAME_FIELD);
            }
            else if (Status != HttpStatusCode.NoContent)
            {
                ErrorMessage = info.GetString(ERROR_MESSAGE_FIELD);
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            info.AddValue(STATUS_FIELD, Status.ToGoogleString());
            if (Status == HttpStatusCode.OK)
            {
                info.AddValue(DST_OFFSET_FIELD, DstOffset);
                info.AddValue(RAW_OFFSET_FIELD, RawOffset);
                info.AddValue(TIME_ZONE_ID_FIELD, TimeZoneId);
                info.AddValue(TIME_ZONE_NAME_FIELD, TimeZoneName);
            }
            else if (Status != HttpStatusCode.NoContent)
            {
                info.AddValue(ERROR_MESSAGE_FIELD, ErrorMessage);
            }
        }
    }
}