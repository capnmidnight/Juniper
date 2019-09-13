namespace Juniper.HTTP
{
    public sealed class BodyInfo
    {
        public readonly string MIMEType;
        public readonly long Length;

        public BodyInfo(string mime, long length)
        {
            MIMEType = mime;
            Length = length;
        }

        public override int GetHashCode()
        {
            return MIMEType.GetHashCode()
                ^ Length.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is BodyInfo body && this == body;
        }

        public static bool operator ==(BodyInfo left, BodyInfo right)
        {
            return ReferenceEquals(left, right)
                || !ReferenceEquals(left, null)
                    && !ReferenceEquals(right, null)
                    && left.MIMEType == right.MIMEType
                    && left.Length == right.Length;
        }

        public static bool operator !=(BodyInfo left, BodyInfo right)
        {
            return !(left == right);
        }
    }
}