using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Juniper.IO
{
    public class ContentReference : IEquatable<ContentReference>
    {
        public static bool operator ==(ContentReference left, ContentReference right)
        {
            return (left is null && right is null)
                || (left is object && left.Equals(right));
        }

        public static bool operator !=(ContentReference left, ContentReference right)
        {
            return !(left == right);
        }

        public static explicit operator string(ContentReference fileRef)
        {
            if (fileRef is null)
            {
                return null;
            }

            return fileRef.CacheID?.AddExtension(fileRef.ContentType);
        }

        public static FileInfo operator +(DirectoryInfo directory, ContentReference fileRef)
        {
            if (directory is null)
            {
                return null;
            }

            if (fileRef is null)
            {
                return null;
            }

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

        public virtual bool DoNotCache
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

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0} ({1})", CacheID, ContentType);
        }

        public string FileName
        {
            get
            {
                var nameStub = CacheID;
                var fileStub = new FileInfo(nameStub);
                nameStub = fileStub.Name;
                if (ContentType.PrimaryExtension is object)
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
            var hashCode = -1239926094;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(CacheID);
            hashCode = hashCode * -1521134295 + EqualityComparer<MediaType>.Default.GetHashCode(ContentType);
            return hashCode;
        }
    }
}
