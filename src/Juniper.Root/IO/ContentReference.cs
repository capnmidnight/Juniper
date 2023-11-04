using System.Globalization;

namespace Juniper.IO
{
    public class ContentReference : IEquatable<ContentReference>
    {
        public static bool operator ==(ContentReference left, ContentReference right)
        {
            return (left is null && right is null)
                || (left is not null && left.Equals(right));
        }

        public static bool operator !=(ContentReference left, ContentReference right)
        {
            return !(left == right);
        }

        public static explicit operator string(ContentReference fileRef)
        {
            return fileRef.ContentType.AddExtension(fileRef.CacheID);
        }

        public static FileInfo operator +(DirectoryInfo directory, ContentReference fileRef)
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

        public virtual string? CacheID
        {
            get;
        }

        public virtual MediaType ContentType
        {
            get;
        }

        public virtual bool DoNotCache
        {
            get;
        }

        public bool Equals(ContentReference? other)
        {
            return other is not null
                && other.ContentType == ContentType
                && other.CacheID == CacheID;
        }

        public override bool Equals(object? obj)
        {
            return obj is ContentReference other
                && Equals(other);
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0} ({1})", CacheID, ContentType);
        }

        public string? FileName
        {
            get
            {
                if (CacheID is null)
                {
                    return null;
                }

                var nameStub = CacheID;
                var fileStub = new FileInfo(nameStub);
                nameStub = fileStub.Name;
                if (ContentType.PrimaryExtension is not null)
                {
                    return $"{nameStub}.{ContentType.PrimaryExtension}";
                }
                else
                {
                    return nameStub;
                }
            }
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(CacheID, ContentType);
        }
    }
}
