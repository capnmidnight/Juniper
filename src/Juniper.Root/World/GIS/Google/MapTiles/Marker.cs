using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;

namespace Juniper.World.GIS.Google.MapTiles
{
    [Serializable]
    public sealed class Marker : IEquatable<Marker>, ISerializable
    {
        private static readonly string STYLE_FIELD = nameof(Style).ToLowerInvariant();
        private static readonly string CENTER_FIELD = nameof(Center).ToLowerInvariant();

        public MarkerStyle Style { get; }
        public string Center { get; }

        public Marker(string center, MarkerStyle style = default)
        {
            Center = center;
            Style = style;
        }

        public Marker(LatLngPoint center, MarkerStyle style = default)
            : this(center?.ToString(CultureInfo.InvariantCulture) ?? throw new ArgumentNullException(nameof(center)), style) { }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Parameter `context` is required by ISerializable interface")]
        private Marker(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            Style = info.GetValue<MarkerStyle>(STYLE_FIELD);
            Center = info.GetString(CENTER_FIELD);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            info.AddValue(STYLE_FIELD, Style);
            info.AddValue(CENTER_FIELD, Center);
        }

        public override bool Equals(object obj)
        {
            return obj is Marker marker && Equals(marker);
        }

        public bool Equals(Marker other)
        {
            return other is object
                && Style == other.Style
                && Center == other.Center;
        }

        public override int GetHashCode()
        {
            var hashCode = -1099193654;
            hashCode = (hashCode * -1521134295) + EqualityComparer<MarkerStyle>.Default.GetHashCode(Style);
            hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(Center);
            return hashCode;
        }

        public static bool operator ==(Marker left, Marker right)
        {
            return ReferenceEquals(left, right)
                || (left is object && left.Equals(right));
        }

        public static bool operator !=(Marker left, Marker right)
        {
            return !(left == right);
        }
    }
}