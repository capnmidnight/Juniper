using System;
using System.Runtime.Serialization;
using Juniper.World.GIS;

namespace Juniper.GIS.Google.MapTiles
{
    [Serializable]
    public sealed class Marker : IEquatable<Marker>, ISerializable
    {
        public readonly MarkerStyle style;
        public readonly string center;

        public Marker(string center, MarkerStyle style = default)
        {
            this.center = center;
            this.style = style;
        }

        public Marker(LatLngPoint center, MarkerStyle style = default)
            : this(center.ToString(), style) { }

        private Marker(SerializationInfo info, StreamingContext context)
        {
            style = info.GetValue<MarkerStyle>(nameof(style));
            center = info.GetString(nameof(center));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(style), style);
            info.AddValue(nameof(center), center);
        }

        public override int GetHashCode()
        {
            return style.GetHashCode()
                ^ center.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is Marker marker && Equals(marker);
        }

        public bool Equals(Marker other)
        {
            return other is object
                && style == other.style
                && center == other.center;
        }

        public static bool operator ==(Marker left, Marker right)
        {
            return ReferenceEquals(left, right)
                || left is object && left.Equals(right);
        }

        public static bool operator !=(Marker left, Marker right)
        {
            return !(left == right);
        }
    }
}