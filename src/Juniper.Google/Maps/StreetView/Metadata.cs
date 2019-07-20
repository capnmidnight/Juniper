using System;
using System.Net;
using System.Runtime.Serialization;

using Juniper.World.GIS;

namespace Juniper.Google.Maps.StreetView
{
    [Serializable]
    public class Metadata : ISerializable
    {
        public readonly HttpStatusCode status;
        public readonly string error_message;
        public readonly string copyright;
        public readonly string date;
        public readonly PanoID pano_id;
        public readonly LatLngPoint location;

        protected Metadata(SerializationInfo info, StreamingContext context)
        {
            status = info.GetString(nameof(status)).MapToStatusCode();
            if (status == HttpStatusCode.OK)
            {
                copyright = info.GetString(nameof(copyright));
                date = info.GetString(nameof(date));
                pano_id = new PanoID(info.GetString(nameof(pano_id)));
                location = info.GetValue<LatLngPoint>(nameof(location));
            }
            else if(status != HttpStatusCode.NoContent)
            {
                error_message = info.GetString(nameof(error_message));
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(status), status.ToString());
            if (status == HttpStatusCode.OK)
            {
                info.MaybeAddValue(nameof(copyright), copyright);
                info.MaybeAddValue(nameof(date), date);
                info.MaybeAddValue(nameof(pano_id), pano_id.ToString());
                info.MaybeAddValue(nameof(location), new
                {
                    lat = location.Latitude,
                    lng = location.Longitude
                });
            }
            else if (status != HttpStatusCode.NoContent)
            {
                info.AddValue(nameof(error_message), error_message);
            }
        }
    }
}