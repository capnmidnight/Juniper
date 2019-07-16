namespace Juniper.Google.Maps.StreetView
{
    public struct PlaceName
    {
        private readonly string place;

        public PlaceName(string place)
        {
            this.place = place;
        }

        public override string ToString()
        {
            return place;
        }

        public override int GetHashCode()
        {
            return place.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is PlaceName p && p.place == place;
        }

        public static bool operator ==(PlaceName left, PlaceName right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PlaceName left, PlaceName right)
        {
            return !(left == right);
        }

        public static explicit operator PlaceName(string placeName)
        {
            return new PlaceName(placeName);
        }
    }
}