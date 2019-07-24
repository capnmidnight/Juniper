namespace Juniper.Google.Maps
{
    public struct PlaceID
    {
        public static explicit operator string(PlaceID value)
        {
            return value.ToString();
        }

        public static explicit operator PlaceID(string placeName)
        {
            return new PlaceID(placeName);
        }

        private readonly string id;

        public PlaceID(string id)
        {
            this.id = id;
        }

        public override string ToString()
        {
            return id;
        }

        public override int GetHashCode()
        {
            return id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is PlaceID p && p.id == id;
        }

        public static bool operator ==(PlaceID left, PlaceID right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PlaceID left, PlaceID right)
        {
            return !(left == right);
        }
    }
}