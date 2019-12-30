using System;

namespace Juniper.HTTP.Client
{
    public sealed class BodyInfo : IEquatable<BodyInfo>
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
            return obj is BodyInfo body && Equals(body);
        }

        public bool Equals(BodyInfo other)
        {
            return other is object
                && MIMEType == other.MIMEType
                && Length == other.Length;
        }

        public static bool operator ==(BodyInfo left, BodyInfo right)
        {
            return ReferenceEquals(left, right)
                || left is object && left.Equals(right);
        }

        public static bool operator !=(BodyInfo left, BodyInfo right)
        {
            return !(left == right);
        }
    }
}