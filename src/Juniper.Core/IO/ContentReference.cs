using System;

namespace Juniper.IO
{
    public class ContentReference<MediaTypeT> : IContentReference<MediaTypeT>, IEquatable<IContentReference<MediaTypeT>>
        where MediaTypeT : MediaType
    {
        public ContentReference(string fileName, MediaTypeT contentType)
        {
            CacheID = fileName;
            ContentType = contentType;
        }

        public string CacheID
        {
            get;
        }

        public MediaTypeT ContentType
        {
            get;
        }

        public bool Equals(IContentReference<MediaTypeT> other)
        {
            return other is object
                && other.ContentType == ContentType
                && other.CacheID == CacheID;
        }

        public override bool Equals(object obj)
        {
            return obj is IContentReference<MediaTypeT> other
                && Equals(other);
        }

        public override int GetHashCode()
        {
            return ContentType.GetHashCode()
                ^ CacheID.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", CacheID, ContentType);
        }

        public static bool operator ==(ContentReference<MediaTypeT> left, IContentReference<MediaTypeT> right)
        {
            return left is null && right is null
                || left is object && left.Equals(right);
        }

        public static bool operator !=(ContentReference<MediaTypeT> left, IContentReference<MediaTypeT> right)
        {
            return !(left == right);
        }
    }
}
