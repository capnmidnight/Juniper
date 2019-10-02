using System;
using System.IO;
using Juniper.HTTP;
using Juniper.Progress;

using UnityEngine;

namespace Juniper.Imaging.Unity
{

    public class ImageDataUnityTextureTranscoder : IImageTranscoder<ImageData, Texture2D>
    {
        private readonly UnityTextureCodec codec;

        public ImageDataUnityTextureTranscoder(MediaType.Image format)
        {
            codec = new UnityTextureCodec(format);
        }

        public ImageDataUnityTextureTranscoder(Texture2D.EXRFlags flags)
        {
            codec = new UnityTextureCodec(flags);
        }

        public ImageDataUnityTextureTranscoder(int jpegQuality = 80)
        {
            codec = new UnityTextureCodec(jpegQuality);
        }

        public MediaType.Image ContentType
        {
            get
            {
                return codec.ContentType;
            }
        }

        public Texture2D Concatenate(Texture2D[,] images, IProgress prog)
        {
            return codec.Concatenate(images, prog);
        }

        public Texture2D Deserialize(Stream stream, IProgress prog)
        {
            return codec.Deserialize(stream, prog);
        }

        public int GetComponents(Texture2D img)
        {
            return codec.GetComponents(img);
        }

        public int GetHeight(Texture2D img)
        {
            return codec.GetHeight(img);
        }

        public ImageInfo GetImageInfo(byte[] data)
        {
            return codec.GetImageInfo(data);
        }

        public int GetWidth(Texture2D img)
        {
            return codec.GetWidth(img);
        }

        public void Serialize(Stream stream, Texture2D value, IProgress prog)
        {
            codec.Serialize(stream, value, prog);
        }

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
                MediaType.Image.Raw,
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
            if (image.contentType == MediaType.Image.Png
                || image.contentType == MediaType.Image.Jpeg
                || image.contentType == MediaType.Image.EXR
                || image.contentType == MediaType.Image.X_Tga)
            {
                texture.LoadImage(image.data);
            }
            else if (image.contentType == MediaType.Image.Raw)
            {
                texture.LoadRawTextureData(image.data);
            }
            prog.Report(1);
            return texture;
        }
    }
}
