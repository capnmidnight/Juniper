using System;
using System.Collections.Generic;
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
        private static readonly string STATUS_FIELD = nameof(Status).ToLowerInvariant();
        private static readonly string COPYRIGHT_FIELD = nameof(Copyright).ToLowerInvariant();
        private static readonly string DATE_FIELD = nameof(Date).ToLowerInvariant();
        private static readonly string PANO_ID_FIELD = nameof(Pano_ID).ToLowerInvariant();
        private static readonly string LOCATION_FIELD = nameof(Location).ToLowerInvariant();

        private static readonly Regex PANO_PATTERN = new Regex("^[a-zA-Z0-9_\\-]+$", RegexOptions.Compiled);

        public static bool IsPano(string panoString)
        {
            return PANO_PATTERN.IsMatch(panoString);
        }

        public HttpStatusCode Status { get; }

        public string Copyright { get; }

        public DateTime Date { get; }

        public string Pano_ID { get; }

        public LatLngPoint Location { get; }

        protected MetadataResponse(MetadataResponse copy)
        {
            if (copy is null)
            {
                throw new ArgumentNullException(nameof(copy));
            }

            Status = copy.Status;
            Copyright = copy.Copyright;
            Date = copy.Date;
            Pano_ID = copy.Pano_ID;
            Location = copy.Location;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Parameter `context` is required by ISerializable interface")]
        protected MetadataResponse(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            Status = info.GetString(STATUS_FIELD).MapToStatusCode();

            if (Status == HttpStatusCode.OK)
            {
                Copyright = info.GetString(COPYRIGHT_FIELD);
                Date = info.GetDateTime(DATE_FIELD);
                Pano_ID = info.GetString(PANO_ID_FIELD);
                Location = info.GetValue<LatLngPoint>(LOCATION_FIELD);
            }
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            info.AddValue(STATUS_FIELD, Status.ToString());
            if (Status == HttpStatusCode.OK)
            {
                _ = info.MaybeAddValue(COPYRIGHT_FIELD, Copyright);
                _ = info.MaybeAddValue(DATE_FIELD, Date.ToString("yyyy-MM", CultureInfo.InvariantCulture));
                _ = info.MaybeAddValue(PANO_ID_FIELD, Pano_ID);
                _ = info.MaybeAddValue(LOCATION_FIELD, new
                {
                    lat = Location.Latitude,
                    lng = Location.Longitude
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
                var byPano = string.CompareOrdinal(Pano_ID, other.Pano_ID);
                var byLocation = Location.CompareTo(other.Location);
                var byDate = Date.CompareTo(other.Date);
                var byCopyright = string.CompareOrdinal(Copyright, other.Copyright);

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
            var hashCode = -1311455165;
            hashCode = (hashCode * -1521134295) + Status.GetHashCode();
            hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(Copyright);
            hashCode = (hashCode * -1521134295) + Date.GetHashCode();
            hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(Pano_ID);
            hashCode = (hashCode * -1521134295) + EqualityComparer<LatLngPoint>.Default.GetHashCode(Location);
            return hashCode;
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