using System;
using System.Collections.Generic;

namespace Juniper.HTTP
{
    public partial class MediaType
    {
        public static readonly MediaType Any = new MediaType("*/*");

        public static MediaType LookupExtension(string ext)
        {
            return byExtensions.Get(ext);
        }

        public static MediaType Lookup(string value)
        {
            var parts = value.SplitX(';');
            return byValue.Get(parts[0]);
        }

        public readonly string Value;
        public readonly string[] Extensions;
        public readonly string PrimaryExtension;

        public MediaType(string value, string[] extensions = null)
        {
            Value = value;
            Extensions = extensions;
            if(extensions?.Length >= 1)
            {
                PrimaryExtension = extensions[0];
            }
        }

        public static implicit operator string(MediaType mediaType)
        {
            return mediaType.Value;
        }
    }
}
