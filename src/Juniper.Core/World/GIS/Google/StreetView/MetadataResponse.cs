using System;
using System.Net;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using Juniper.Google;
using Juniper.World.GIS;

namespace Juniper.GIS.Google.StreetView
{
    [Serializable]
    public class MetadataResponse : ISerializable
    {
        private static readonly Regex PANO_PATTERN = new Regex("^[a-zA-Z0-9_\\-]+$", RegexOptions.Compiled);

        public static bool IsPano(string panoString)
        {
            return PANO_PATTERN.IsMatch(panoString);
        }

        public readonly HttpStatusCode status;
        public readonly string copyright;
        public readonly DateTime date;
        public readonly string pano_id;
        public readonly LatLngPoint location;

        protected MetadataResponse(SerializationInfo info, StreamingContext context)
        {
            status = info.GetString(nameof(status)).MapToStatusCode();
            if (status == HttpStatusCode.OK)
            {
                copyright = info.GetString(nameof(copyright));
                date = info.GetDateTime(nameof(date));
                pano_id = info.GetString(nameof(pano_id));
                location = info.GetValue<LatLngPoint>(nameof(location));
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(status), status.ToString());
            if (status == HttpStatusCode.OK)
            {
                info.MaybeAddValue(nameof(copyright), copyright);
                info.MaybeAddValue(nameof(date), date.ToString("yyyy-MM"));
                info.MaybeAddValue(nameof(pano_id), pano_id);
                info.MaybeAddValue(nameof(location), new
                {
                    lat = location.Latitude,
                    lng = location.Longitude
                });
            }
        }
    }
}