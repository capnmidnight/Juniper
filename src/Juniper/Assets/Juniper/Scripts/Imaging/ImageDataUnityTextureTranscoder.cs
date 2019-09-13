using System;

using Juniper.HTTP;
using Juniper.Progress;

using UnityEngine;

namespace Juniper.Imaging.Unity
{
    public class ImageDataUnityTextureTranscoder : IImageTranscoder<ImageData, Texture2D>
    {
        public ImageData TranslateFrom(Texture2D image, IProgress prog)
        {
            int components;
            if (image.format == TextureFormat.RGBA32
                || image.format == TextureFormat.ARGB32
                || image.format == TextureFormat.BGRA32)
            {
                components = 4;
            }
            else if (image.format == TextureFormat.RGB24)
            {
                components = 3;
            }
            else
            {
                throw new NotSupportedException();
            }

            return new ImageData(
                new ImageInfo(image.width, image.height, components),
                MediaType.Image.Raw,
                image.GetRawTextureData());
        }

        public Texture2D TranslateTo(ImageData image, IProgress prog)
        {
            return image.CreateTexture();
        }
    }
}
