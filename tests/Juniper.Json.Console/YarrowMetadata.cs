using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Juniper.GIS.Google.StreetView;
using Juniper.World.GIS;

namespace Yarrow
{
    [Serializable]
    public class YarrowMetadata : ISerializable
    {
        public string copyright;
        public DateTime date;
        public string pano_id;
        public LatLngPoint location;
        public List<string> navPoints;

        public YarrowMetadata(MetadataResponse content)
        {
            copyright = content.copyright;
            date = content.date;
            pano_id = content.pano_id;
            location = content.location;
            navPoints = new List<string>();
        }

        protected YarrowMetadata(SerializationInfo info, StreamingContext context)
        {
            copyright = info.GetString(nameof(copyright));
            date = info.GetDateTime(nameof(date));
            pano_id = info.GetString(nameof(pano_id));
            location = info.GetValue<LatLngPoint>(nameof(location));
            navPoints = new List<string>();

            foreach (var key in info)
            {
                switch (key.Name)
                {
                    case nameof(navPoints):
                    navPoints.AddRange(info.GetValue<string[]>(nameof(navPoints)));
                    break;
                }
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.MaybeAddValue(nameof(copyright), copyright);
            info.MaybeAddValue(nameof(date), date.ToString("yyyy-MM"));
            info.MaybeAddValue(nameof(pano_id), pano_id.ToString());
            info.MaybeAddValue(nameof(location), new
            {
                lat = location.Latitude,
                lng = location.Longitude
            });

            info.AddList(nameof(navPoints), navPoints);
        }
    }
}