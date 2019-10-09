using System;
using Juniper.Progress;
using UnityEngine;

namespace Juniper.Imaging
{

    public class ImageDataUnityTextureTranscoder : IImageTranscoder<ImageData, Texture2D>
    {
        public ImageData Translate(Texture2D image, IProgress prog)
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
                image.GetRawTextureData());
        }

        public Texture2D Translate(ImageData image, IProgress prog)
        {
            prog.Report(0);
            var texture = new Texture2D(
                image.info.dimensions.width,
                image.info.dimensions.height,
                image.info.components == 4
                    ? TextureFormat.RGBA32
                    : TextureFormat.RGB24,
                false);
            texture.LoadRawTextureData(image.data);
            prog.Report(1);
            return texture;
        }
    }
}
