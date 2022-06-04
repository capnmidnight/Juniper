namespace Juniper.HTTP
{
    public sealed class BodyInfo : IEquatable<BodyInfo>
    {
        public MediaType ContentType { get; }
        public long Length { get; }

        public BodyInfo(MediaType contentType, long length)
        {
            ContentType = contentType;
            Length = length;
        }

        public override bool Equals(object obj)
        {
            return obj is BodyInfo body && Equals(body);
        }

        public bool Equals(BodyInfo other)
        {
            return other is not null
                && ContentType == other.ContentType
                && Length == other.Length;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ContentType, Length);
        }

        public static bool operator ==(BodyInfo left, BodyInfo right)
        {
            return (left is null && right is null)
                || (left is not null && left.Equals(right));
        }

        public static bool operator !=(BodyInfo left, BodyInfo right)
        {
            return !(left == right);
        }
    }
}