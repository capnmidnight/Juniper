using System;
using System.Runtime.Serialization;

using Juniper.Serialization;
using Juniper.World.GIS;

namespace Juniper.Google.Maps.StreetView
{
    [Serializable]
    public class Metadata : ISerializable
    {
        public readonly API.StatusCode status;
        public readonly string error_message;
        public readonly string copyright;
        public readonly string date;
        public readonly PanoID pano_id;
        public readonly LatLngPoint location;

        protected Metadata(SerializationInfo info, StreamingContext context)
        {
            status = info.GetEnumFromString<API.StatusCode>(nameof(status));
            if (status == API.StatusCode.OK)
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