
using System;

using Juniper.Progress;

using UnityEngine;

namespace Juniper.Imaging
{
    public class UnityTextureImageDataCodec : AbstractImageDataTranscoder<UnityTextureCodec, Texture2D>
    {
        public UnityTextureImageDataCodec(HTTP.MediaType.Image format)
            : base(new UnityTextureCodec(format)) { }

        public UnityTextureImageDataCodec(Texture2D.EXRFlags exrFlags)
            : base(new UnityTextureCodec(exrFlags)) { }

        public UnityTextureImageDataCodec(int jpegEncodingQuality = 80)
            : base(new UnityTextureCodec(jpegEncodingQuality)) { }

        public override Texture2D TranslateTo(ImageData image, IProgress prog = null)
        {
            prog?.Report(0);
            var texture = new Texture2D(image.info.dimensions.width, image.info.dimensions.height, TextureFormat.RGB24, false);
            prog?.Report(0.25f);
            if (image.contentType == HTTP.MediaType.Image.Jpeg
                || image.contentType == HTTP.MediaType.Image.Png)
            {
                texture.LoadRawTextureData(image.data);
                prog?.Report(0.5f);
                texture.Compress(true);
                prog?.Report(0.75f);
            }
            else if (image.contentType == HTTP.MediaType.Image.Raw)
            {
                texture.LoadImage(image.data);
                prog?.Report(0.5f);
            }
            texture.Apply(false, true);
            prog?.Report(1);
            return texture;
        }

        public override ImageData TranslateFrom(Texture2D image)
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
                Format,
                subCodec.Encode(image));
        }
    }
}
