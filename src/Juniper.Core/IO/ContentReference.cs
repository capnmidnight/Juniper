using System;
using System.IO;

namespace Juniper.IO
{
    public class ContentReference : IEquatable<ContentReference>
    {
        public static bool operator ==(ContentReference left, ContentReference right)
        {
            return left is null && right is null
                || left is object && left.Equals(right);
        }

        public static bool operator !=(ContentReference left, ContentReference right)
        {
            return !(left == right);
        }

        public static explicit operator string(ContentReference fileRef)
        {
            return fileRef.ContentType.AddExtension(fileRef.CacheID);
        }

        public static FileInfo operator+(DirectoryInfo directory, ContentReference fileRef)
        {
            var fileName = (string)fileRef;
            return new FileInfo(Path.Combine(directory.FullName, fileName));
        }

        public static FileInfo operator +(ContentReference fileRef, DirectoryInfo directory)
        {
            return directory + fileRef;
        }

        protected ContentReference(MediaType contentType)
        {
            ContentType = contentType;
        }

        public ContentReference(string cacheID, MediaType contentType)
            : this(contentType)
        {
            CacheID = cacheID;
        }

        public virtual string CacheID
        {
            get;
        }

        public virtual MediaType ContentType
        {
            get;
        }

        public bool Equals(ContentReference other)
        {
            return other is object
                && other.ContentType == ContentType
                && other.CacheID == CacheID;
        }

        public override bool Equals(object obj)
        {
            return obj is ContentReference other
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
    }
}
