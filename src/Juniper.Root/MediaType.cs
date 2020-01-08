using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

using Juniper.IO;

namespace Juniper
{
    public partial class MediaType : IEquatable<MediaType>
    {
        public static readonly MediaType Any = new MediaType("*/*");

        private static Dictionary<string, MediaType> byExtensions;

        private static Dictionary<string, MediaType> byValue;

        public static MediaType GuessByExtension(string ext)
        {
            if (string.IsNullOrEmpty(ext))
            {
                ext = "unknown";
            }

            if (byExtensions.ContainsKey(ext))
            {
                return byExtensions[ext];
            }
            else
            {
                return new Unknown("unknown/" + ext);
            }
        }

        public static MediaType GuessByExtension(FileInfo file)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            var ext = file.Extension;
            if (ext.Length > 0
                && ext[0] == '.')
            {
                ext = ext.Substring(1);
            }

            return GuessByExtension(ext);
        }

        public static MediaType Lookup(string value)
        {
            var parts = value.SplitX(';');
            foreach (var part in parts)
            {
                if (byValue.ContainsKey(part))
                {
                    return byValue[part];
                }
            }

            var name = Array.Find(parts, p => p.Length > 0);

            if (string.IsNullOrEmpty(name))
            {
                return Lookup("unknown/unknown");
            }
            else
            {
                return new Unknown(name);
            }
        }

        public static ContentReference operator +(string cacheID, MediaType contentType)
        {
            return new ContentReference(cacheID, contentType);
        }

        public static explicit operator MediaType(string fileName)
        {
            return GuessByExtension(PathExt.GetShortExtension(fileName));
        }

        public static explicit operator MediaType(FileInfo file)
        {
            if (file is null)
            {
                return null;
            }

            return (MediaType)file.Name;
        }

        public static implicit operator string(MediaType mediaType)
        {
            return mediaType?.Value;
        }

        public string Value { get; }

        public ReadOnlyCollection<string> Extensions { get; }

        public string PrimaryExtension { get; }

        protected MediaType(string value, string[] extensions)
        {
            Value = value;
            if (extensions is null)
            {
                extensions = Array.Empty<string>();
            }

            Extensions = Array.AsReadOnly(extensions);
            if (Extensions.Count >= 1)
            {
                PrimaryExtension = extensions[0];
            }

            if (byValue is null)
            {
                byValue = new Dictionary<string, MediaType>(1000);
            }

            if (byExtensions is null)
            {
                byExtensions = new Dictionary<string, MediaType>(1000);
            }

            _ = byValue.Default(Value, this);

            foreach (var ext in Extensions)
            {
                _ = byExtensions.Default(ext, this);
            }
        }

        protected MediaType(string value)
            : this(value, null)
        { }

        public virtual bool Matches(string fileName)
        {
            return (this == Any && Values.Any(v => v.Matches(fileName)))
                || Extensions.Contains(PathExt.GetLongExtension(fileName))
                || Extensions.Contains(PathExt.GetShortExtension(fileName));
        }

        public bool Matches(FileInfo file)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            return Matches(file.Name);
        }

        public string AddExtension(string fileName)
        {
            if (fileName is null)
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            if (PrimaryExtension is object)
            {
                var currentExtension = PathExt.GetShortExtension(fileName);
                if (Extensions.IndexOf(currentExtension) == -1)
                {
                    fileName += "." + PrimaryExtension;
                }
            }

            return fileName;
        }

        public bool Equals(MediaType other)
        {
            return other is object
                && other.Value == Value;
        }

        public override bool Equals(object obj)
        {
            return obj is MediaType other
                && Equals(other);
        }

        public static bool operator ==(MediaType left, MediaType right)
        {
            return (left is null && right is null)
                || (left is object && left.Equals(right));
        }

        public static bool operator !=(MediaType left, MediaType right)
        {
            return !(left == right);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
