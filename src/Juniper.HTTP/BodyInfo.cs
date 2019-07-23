namespace Juniper.HTTP
{
    public struct BodyInfo
    {
        public readonly string MIMEType;
        public readonly long Length;

        public BodyInfo(string mime, long length)
        {
            MIMEType = mime;
            Length = length;
        }

        public override bool Equals(object obj)
        {
            return obj != null
                && obj is BodyInfo body
                && body.MIMEType.Equals(body)
                && body.Length.Equals(Length);
        }

        public override int GetHashCode()
        {
            return MIMEType.GetHashCode()
                ^ Length.GetHashCode();
        }

        public static bool operator ==(BodyInfo left, BodyInfo right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(BodyInfo left, BodyInfo right)
        {
            return !(left == right);
        }
    }
}