namespace Juniper.Google.Maps.Geocoding
{
    public struct USAddress
    {
        private readonly string street;
        private readonly string city;
        private readonly string state;
        private readonly string zip;

        public USAddress(string street, string city, string state, string zip)
        {
            this.street = street;
            this.city = city;
            this.state = state;
            this.zip = zip;
        }

        public USAddress(string street, string city, string state)
            : this(street, city, state, string.Empty) { }

        public override string ToString()
        {
            return string.Join(", ", street, city, state, zip);
        }

        public override bool Equals(object obj)
        {
            return obj != null
                && obj is USAddress add
                && add.street.Equals(street)
                && add.city.Equals(city)
                && add.state.Equals(state)
                && add.zip.Equals(zip);
        }

        public override int GetHashCode()
        {
            return street.GetHashCode()
                ^ city.GetHashCode()
                ^ state.GetHashCode()
                ^ zip.GetHashCode();
        }

        public static bool operator ==(USAddress left, USAddress right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(USAddress left, USAddress right)
        {
            return !(left == right);
        }
    }
}
