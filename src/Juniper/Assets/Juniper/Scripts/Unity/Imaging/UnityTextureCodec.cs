using System;
using System.IO;

using Juniper.Progress;
using Juniper.Streams;

using UnityEngine;

namespace Juniper.Imaging
{
    public class UnityTextureCodec : IImageDecoder<Texture2D>
    {
        private readonly Texture2D.EXRFlags exrFlags;
        private readonly int jpegEncodingQuality;

        public UnityTextureCodec(HTTP.MediaType.Image format)
        {
            if (format != HTTP.MediaType.Image.EXR
                && format != HTTP.MediaType.Image.Jpeg
                && format != HTTP.MediaType.Image.Png
                && format != HTTP.MediaType.Image.X_Tga)
            {
                throw new NotSupportedException($"Unity doesn't know how to encode {Format.Value} image data.");
            }

            Format = format;
            exrFlags = Texture2D.EXRFlags.None;
            jpegEncodingQuality = 80;
        }

        public UnityTextureCodec(Texture2D.EXRFlags exrFlags)
            : this(HTTP.MediaType.Image.EXR)
        {
            this.exrFlags = exrFlags;
        }

        public UnityTextureCodec(int jpegEncodingQuality = 80)
            : this(HTTP.MediaType.Image.Jpeg)
        {
            this.jpegEncodingQuality = jpegEncodingQuality;
        }

        public HTTP.MediaType.Image Format { get; private set; }

        public ImageInfo GetImageInfo(byte[] data)
        {
            if (Format == HTTP.MediaType.Image.Jpeg)
            {
                return ImageInfo.ReadJPEG(data);
            }
            else if (Format == HTTP.MediaType.Image.Png)
            {
                return ImageInfo.ReadPNG(data);
            }
            else
            {
                throw new NotSupportedException($"Don't know how to read the raw image information from an {Format.Value} file.");
            }
        }

        public int GetWidth(Texture2D img)
        {
            return img.width;
        }

        public int GetHeight(Texture2D img)
        {
            return img.height;
        }

        public int GetComponents(Texture2D img)
        {
            if(img.format == TextureFormat.ARGB32
                || img.format == TextureFormat.RGBA32
                || img.format == TextureFormat.BGRA32)
            {
                return 4;
            }
            else if(img.format == TextureFormat.RGB24)
            {
                return 3;
            }
            else
            {
                throw new NotSupportedException($"Don't know how to handle pixel format {img.format}.");
            }
        }

        public Texture2D Concatenate(Texture2D[,] images, IProgress prog = null)
        {
            this.ValidateImages(images, prog,
                out var rows, out var columns, out var components,
                out var tileWidth, out var tileHeight);

            var totalLen = rows * tileHeight * columns * tileWidth;

            var outputImage = new Texture2D(columns * tileWidth, rows * tileHeight);
            for (var r = 0; r < rows; ++r)
            {
                for (var c = 0; c < columns; ++c)
                {
                    var img = images[r, c];
                    if (img != null)
                    {
                        for (var y = 0; y < tileHeight; ++y)
                        {
                            var pixels = img.GetPixels(0, tileHeight - y - 1, tileWidth, 1);
                            outputImage.SetPixels(c * tileWidth, r * tileHeight + y, tileWidth, 1, pixels);
                            prog?.Report(tileWidth * (r * tileHeight * columns + c * tileHeight + y), totalLen);
                        }
                    }
                }
            }

            return outputImage;
        }

        public byte[] Encode(Texture2D value)
        {
            if (Format == HTTP.MediaType.Image.EXR)
            {
                return value.EncodeToEXR(exrFlags);
            }
            else if (Format == HTTP.MediaType.Image.Jpeg)
            {
                return value.EncodeToJPG(jpegEncodingQuality);
            }
            else if (Format == HTTP.MediaType.Image.Png)
            {
                return value.EncodeToPNG();
            }
            else if (Format == HTTP.MediaType.Image.X_Tga)
            {
                return value.EncodeToTGA();
            }
            else
            {
                throw new NotSupportedException($"Unity doesn't know how to encode {Format.Value} image data.");
            }
        }

        public void Serialize(Stream stream, Texture2D value, IProgress prog = null)
        {
            var buf = Encode(value);
            var progStream = new ProgressStream(stream, buf.Length, prog);
            progStream.Write(buf, 0, buf.Length);
        }

        public Texture2D Deserialize(Stream stream)
        {
            using (var mem = new MemoryStream())
            {
                stream.CopyTo(mem);
                stream.Flush();
                var buffer = mem.ToArray();
                var info = GetImageInfo(buffer);
                var texture = new Texture2D(info.dimensions.width, info.dimensions.height);
                texture.LoadImage(buffer);
                return texture;
            }
        }
    }
}
