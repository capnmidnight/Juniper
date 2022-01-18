using System;

namespace Juniper.HTTP
{
    public sealed class BodyInfo : IEquatable<BodyInfo>
    {
        public string MIMEType { get; }
        public long Length { get; }

        public BodyInfo(string mime, long length)
        {
            MIMEType = mime;
            Length = length;
        }

        public override bool Equals(object obj)
        {
            return obj is BodyInfo body && Equals(body);
        }

        public bool Equals(BodyInfo other)
        {
            return other is not null
                && MIMEType == other.MIMEType
                && Length == other.Length;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(MIMEType, Length);
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