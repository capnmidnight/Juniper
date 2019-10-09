using System;
using System.Collections.Generic;
using System.IO;

namespace Juniper
{
    public partial class MediaType : IEquatable<MediaType>
    {
        public static readonly MediaType Any = new MediaType("*/*");

        public static MediaType GuessByExtension(string ext)
        {
            return byExtensions.Get(ext);
        }

        public static MediaType Guess(FileInfo file)
        {
            return GuessByExtension(PathExt.GetShortExtension(file.Name));
        }

        public static MediaType Lookup(string value)
        {
            var parts = value.SplitX(';');
            return byValue.Get(parts[0]);
        }

        public readonly string Value;
        public readonly string[] Extensions;
        public readonly string PrimaryExtension;

        public MediaType(string value, string[] extensions)
        {
            Value = value;
            Extensions = extensions;
            if(extensions?.Length >= 1)
            {
                PrimaryExtension = extensions[0];
            }
        }

        public MediaType(string value)
            : this(value, null)
        { }

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

        public static bool operator==(MediaType left, MediaType right)
        {
            return left is null && right is null
                || left is object && left.Equals(right);
        }

        public static bool operator!=(MediaType left, MediaType right)
        {
            return !(left == right);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
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
                if (Array.IndexOf(contentType.Extensions, currentExtension) == -1)
                {
                    fileName += "." + contentType.PrimaryExtension;
                }
            }

            return fileName;
        }
    }
}
