using System.Collections.Generic;

using Juniper.Progress;

using UnityEngine;

namespace Juniper.Imaging
{
    public static class ImageDataExt
    {
        public static IEnumerator<Texture> ToTexture(this ImageData image, IProgress prog = null)
        {
            prog?.Report(0);
            var texture = new Texture2D(image.dimensions.width, image.dimensions.height, TextureFormat.RGB24, false);
            prog?.Report(0.25f);
            yield return null;
            if (image.format == ImageFormat.None)
            {
                texture.LoadRawTextureData(image.data);
                prog?.Report(0.5f);
                yield return null;
                texture.Compress(true);
                prog?.Report(0.75f);
                yield return null;
            }
            else if (image.format != ImageFormat.Unsupported)
            {
                texture.LoadImage(image.data);
                prog?.Report(0.5f);
                yield return null;
            }
            texture.Apply(false, true);
            prog?.Report(1);
            yield return texture;
        }
    }
}