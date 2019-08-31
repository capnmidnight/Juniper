using System;

namespace UnityEngine
{
    public static class TextureExt
    {
        public static int GetComponents(this Texture2D img)
        {
            if (img.format == TextureFormat.ARGB32
                || img.format == TextureFormat.RGBA32
                || img.format == TextureFormat.BGRA32)
            {
                return 4;
            }
            else if (img.format == TextureFormat.RGB24)
            {
                return 3;
            }
            else
            {
                throw new NotSupportedException($"Don't know how to handle pixel format {img.format}.");
            }
        }
    }
}
