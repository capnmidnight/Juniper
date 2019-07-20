namespace Juniper.Google.Maps.StreetView
{
    public struct PanoID
    {
        private readonly string id;

        public PanoID(string id)
        {
            this.id = id;
        }

        public override string ToString()
        {
            return id;
        }

        public static explicit operator string(PanoID value)
        {
            return value.ToString();
        }

        public override int GetHashCode()
        {
            return id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is PanoID pano && pano.id == id;
        }

        public static bool operator ==(PanoID left, PanoID right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PanoID left, PanoID right)
        {
            return !(left == right);
        }
    }
}