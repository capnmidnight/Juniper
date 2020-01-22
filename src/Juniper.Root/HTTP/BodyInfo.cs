using System;
using System.Collections.Generic;

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
            return other is object
                && MIMEType == other.MIMEType
                && Length == other.Length;
        }

        public override int GetHashCode()
        {
            var hashCode = -1731715182;
            hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(MIMEType);
            hashCode = (hashCode * -1521134295) + Length.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(BodyInfo left, BodyInfo right)
        {
            return (left is null && right is null)
                || (left is object && left.Equals(right));
        }

        public static bool operator !=(BodyInfo left, BodyInfo right)
        {
            return !(left == right);
        }
    }
}