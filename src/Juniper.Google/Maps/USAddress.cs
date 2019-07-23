namespace Juniper.Google.Maps
{
    public struct USAddress
    {
        public static explicit operator string(USAddress value)
        {
            return value.ToString();
        }

        public static bool operator ==(USAddress left, USAddress right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(USAddress left, USAddress right)
        {
            return !(left == right);
        }

        public static implicit operator PlaceName(USAddress address)
        {
            return (PlaceName)address.ToString();
        }

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
                && ((obj is USAddress add
                        && add.street.Equals(street)
                        && add.city.Equals(city)
                        && add.state.Equals(state)
                        && add.zip.Equals(zip))
                    || (obj is PlaceName name
                        && name == this));
        }

        public override int GetHashCode()
        {
            return street.GetHashCode()
                ^ city.GetHashCode()
                ^ state.GetHashCode()
                ^ zip.GetHashCode();
        }
    }
}