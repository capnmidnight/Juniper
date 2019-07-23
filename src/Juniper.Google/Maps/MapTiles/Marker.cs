using Juniper.World.GIS;

namespace Juniper.Google.Maps.MapTiles
{
    public struct Marker
    {
        public readonly MarkerStyle style;
        public readonly string center;

        public Marker(string center, MarkerStyle style = default)
        {
            this.center = center;
            this.style = style;
        }

        public Marker(LatLngPoint center, MarkerStyle style = default)
            : this(center.ToCSV(), style) { }

        public override bool Equals(object obj)
        {
            return obj != null
                && obj is Marker marker
                && marker.style == style
                && marker.center == center;
        }

        public override int GetHashCode()
        {
            return style.GetHashCode()
                ^ center.GetHashCode();
        }

        public static bool operator ==(Marker left, Marker right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Marker left, Marker right)
        {
            return !(left == right);
        }
    }
}