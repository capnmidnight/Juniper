using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.IO;

namespace Juniper
{
    public partial class MediaType : IEquatable<MediaType>
    {
        public static readonly MediaType Any = new MediaType("*/*");

        private static Dictionary<string, MediaType> byExtensions;

        private static Dictionary<string, MediaType> byValue;

        public static MediaType GuessByExtension(string ext)
        {
            if(ext == null)
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

        public static MediaType Guess(string fileName)
        {
            return GuessByExtension(PathExt.GetShortExtension(fileName));
        }

        public static MediaType Guess(FileInfo file)
        {
            return Guess(file.Name);
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

            parts = parts
                .Where(p => p.Length > 0)
                .ToArray();

            if (parts.Length == 0)
            {
                return Lookup("unknown/unknown");
            }
            else
            {
                return new Unknown(parts[0]);
            }
        }

        public readonly string Value;
        public readonly ReadOnlyCollection<string> Extensions;
        public readonly string PrimaryExtension;

        protected MediaType(string value, string[] extensions)
        {
            Value = value;
            if(extensions == null)
            {
                extensions = Array.Empty<string>();
            }

            Extensions = Array.AsReadOnly(extensions);
            if (Extensions.Count >= 1)
            {
                PrimaryExtension = extensions[0];
            }

            if(byValue == null)
            {
                byValue = new Dictionary<string, MediaType>(1000);
            }

            if(byExtensions == null)
            {
                byExtensions = new Dictionary<string, MediaType>(1000);
            }

            if (!byValue.ContainsKey(Value))
            {
                byValue.Add(Value, this);
            }

            foreach (var ext in Extensions)
            {
                if (!byExtensions.ContainsKey(ext))
                {
                    byExtensions.Add(ext, this);
                }
            }
        }

        protected MediaType(string value)
            : this(value, null)
        { }

        public virtual bool Matches(string fileName)
        {
            return this == Any && Values.Any(v => v.Matches(fileName))
                || Extensions.Contains(PathExt.GetLongExtension(fileName))
                || Extensions.Contains(PathExt.GetShortExtension(fileName));
        }

        public bool Matches(FileInfo file)
        {
            return Matches(file.Name);
        }

        public static implicit operator string(MediaType mediaType)
        {
            return mediaType.Value;
        }

        public bool Equals(MediaType other)
        {
            return other is object
                && other.Value == Value;
        }

        public override bool Equals(object other)
        {
            return other is MediaType mediaType
                && Equals(mediaType);
        }

        public static bool operator ==(MediaType left, MediaType right)
        {
            return left is null && right is null
                || left is object && left.Equals(right);
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

    public static class MediaTypeExt
    {
        public static string AddExtension(this MediaType contentType, string fileName)
        {
            if (contentType != null
                && contentType.PrimaryExtension != null)
            {
                var currentExtension = PathExt.GetShortExtension(fileName);
                if (contentType.Extensions.IndexOf(currentExtension) == -1)
                {
                    fileName += "." + contentType.PrimaryExtension;
                }
            }

            return fileName;
        }
    }
}
