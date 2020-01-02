using System;
using System.Globalization;
using System.Net;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

using Juniper.Google;

namespace Juniper.World.GIS.Google.StreetView
{
    [Serializable]
    public class MetadataResponse :
        ISerializable,
        IEquatable<MetadataResponse>,
        IComparable<MetadataResponse>
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Parameter `context` is required by ISerializable interface")]
        protected MetadataResponse(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

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
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            info.AddValue(nameof(status), status.ToString());
            if (status == HttpStatusCode.OK)
            {
                info.MaybeAddValue(nameof(copyright), copyright);
                info.MaybeAddValue(nameof(date), date.ToString("yyyy-MM", CultureInfo.InvariantCulture));
                info.MaybeAddValue(nameof(pano_id), pano_id);
                info.MaybeAddValue(nameof(location), new
                {
                    lat = location.Latitude,
                    lng = location.Longitude
                });
            }
        }

        public int CompareTo(MetadataResponse other)
        {
            if (other is null)
            {
                return -1;
            }
            else
            {
                var byPano = string.CompareOrdinal(pano_id, other.pano_id);
                var byLocation = location.CompareTo(other.location);
                var byDate = date.CompareTo(other.date);
                var byCopyright = string.CompareOrdinal(copyright, other.copyright);

                if (byPano == 0
                    && byLocation == 0
                    && byDate == 0)
                {
                    return byCopyright;
                }
                else if (byPano == 0
                    && byLocation == 0)
                {
                    return byDate;
                }
                else if (byPano == 0)
                {
                    return byLocation;
                }
                else
                {
                    return byPano;
                }
            }
        }

        public override bool Equals(object obj)
        {
            return obj is MetadataResponse other
                && Equals(other);
        }

        public bool Equals(MetadataResponse other)
        {
            return other is object
                && CompareTo(other) == 0;
        }

        public override int GetHashCode()
        {
            return status.GetHashCode()
                ^ copyright.GetHashCode()
                ^ date.GetHashCode()
                ^ location.GetHashCode()
                ^ pano_id.GetHashCode();
        }

        public static bool operator ==(MetadataResponse left, MetadataResponse right)
        {
            return (left is null && right is null)
                || (left is object && left.Equals(right));
        }

        public static bool operator !=(MetadataResponse left, MetadataResponse right)
        {
            return !(left == right);
        }

        public static bool operator <(MetadataResponse left, MetadataResponse right)
        {
            return left is null
                ? right is object
                : left.CompareTo(right) < 0;
        }

        public static bool operator <=(MetadataResponse left, MetadataResponse right)
        {
            return left is null
                || left.CompareTo(right) <= 0;
        }

        public static bool operator >(MetadataResponse left, MetadataResponse right)
        {
            return left is object
                && left.CompareTo(right) > 0;
        }

        public static bool operator >=(MetadataResponse left, MetadataResponse right)
        {
            return left is null
                ? right is null
                : left.CompareTo(right) >= 0;
        }
    }
}