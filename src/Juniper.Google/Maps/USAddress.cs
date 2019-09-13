namespace Juniper.Google.Maps
{
    public class USAddress
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
            return obj is USAddress addr && this == addr
                || obj is string name && ToString() == name;
        }
        public static bool operator ==(USAddress left, USAddress right)
        {
            return ReferenceEquals(left, right)
                || !ReferenceEquals(left, null)
                    && !ReferenceEquals(right, null)
                    && left.street == right.street
                    && left.city == right.city
                    && left.state == right.state
                    && left.zip == right.zip;
        }

        public static bool operator !=(USAddress left, USAddress right)
        {
            return !(left == right);
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