using System;

namespace Juniper.IO
{
    public class ContentReference : IContentReference, IEquatable<IContentReference>
    {
        public ContentReference(string fileName, MediaType contentType)
        {
            CacheID = fileName;
            ContentType = contentType;
        }

        public string CacheID
        {
            get;
        }

        public MediaType ContentType
        {
            get;
        }

        public bool Equals(IContentReference other)
        {
            return other is object
                && other.ContentType == ContentType
                && other.CacheID == CacheID;
        }

        public override bool Equals(object obj)
        {
            return obj is IContentReference other
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

        public static implicit operator string(ContentReference fileRef)
        {
            return fileRef.ContentType.AddExtension(fileRef.CacheID);
        }

        public static bool operator ==(ContentReference left, IContentReference right)
        {
            return left is null && right is null
                || left is object && left.Equals(right);
        }

        public static bool operator !=(ContentReference left, IContentReference right)
        {
            return !(left == right);
        }
    }
}
