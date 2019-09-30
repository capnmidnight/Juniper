using System;
using System.Collections.Generic;
using System.IO;

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
            : this(value, null) { }

        public static implicit operator string(MediaType mediaType)
        {
            return mediaType.Value;
        }
    }

    public static class MediaTypeExt
    {
        public static string AddExtension(this MediaType contentType, string fileName)
        {
            if (contentType != null
                && contentType.PrimaryExtension != null)
            {
                var expectedExt = "." + contentType.PrimaryExtension;
                if (Path.GetExtension(fileName) != expectedExt)
                {
                    fileName += expectedExt;
                }
            }

            return fileName;
        }
    }
}
