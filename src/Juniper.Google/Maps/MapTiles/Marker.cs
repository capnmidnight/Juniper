using Juniper.World.GIS;

namespace Juniper.Google.Maps.MapTiles
{
    public class Marker
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

        public override int GetHashCode()
        {
            return style.GetHashCode()
                ^ center.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is Marker marker && this == marker;
        }

        public static bool operator ==(Marker left, Marker right)
        {
            return ReferenceEquals(left, right)
                || !ReferenceEquals(left, null)
                    && !ReferenceEquals(right, null)
                    && left.style == right.style
                    && left.center == right.center;;
        }

        public static bool operator !=(Marker left, Marker right)
        {
            return !(left == right);
        }
    }
}