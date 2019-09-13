using System;
using System.IO;

using Juniper.HTTP;
using Juniper.Progress;
using Juniper.Streams;

using UnityEngine;

namespace Juniper.Imaging.Unity
{
    public class UnityTextureCodec : IImageCodec<Texture2D>
    {
        private readonly Texture2D.EXRFlags exrFlags;
        private readonly int jpegEncodingQuality;

        public UnityTextureCodec(MediaType.Image format)
        {
            if (format != MediaType.Image.EXR
                && format != MediaType.Image.Jpeg
                && format != MediaType.Image.Png
                && format != MediaType.Image.X_Tga
                && format != MediaType.Image.Raw)
            {
                throw new NotSupportedException($"Unity doesn't know how to encode {ContentType.Value} image data.");
            }

            ContentType = format;
            exrFlags = Texture2D.EXRFlags.None;
            jpegEncodingQuality = 80;
        }

        public UnityTextureCodec(Texture2D.EXRFlags exrFlags)
            : this(MediaType.Image.EXR)
        {
            this.exrFlags = exrFlags;
        }

        public UnityTextureCodec(int jpegEncodingQuality = 80)
            : this(MediaType.Image.Jpeg)
        {
            this.jpegEncodingQuality = jpegEncodingQuality;
        }

        public MediaType ContentType { get; private set; }

        public ImageInfo GetImageInfo(byte[] data)
        {
            if (ContentType == MediaType.Image.Jpeg)
            {
                return ImageInfo.ReadJPEG(data);
            }
            else if (ContentType == MediaType.Image.Png)
            {
                return ImageInfo.ReadPNG(data);
            }
            else
            {
                throw new NotSupportedException($"Don't know how to read the raw image information from an {ContentType.Value} file.");
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
            return img.GetComponents();
        }

        public Texture2D Concatenate(Texture2D[,] images, IProgress prog)
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

        public byte[] Serialize(Texture2D value, IProgress prog)
        {
            prog?.Report(0);
            byte[] buffer;
            if (ContentType == MediaType.Image.EXR)
            {
                buffer = value.EncodeToEXR(exrFlags);
            }
            else if (ContentType == MediaType.Image.Jpeg)
            {
                buffer = value.EncodeToJPG(jpegEncodingQuality);
            }
            else if (ContentType == MediaType.Image.Png)
            {
                buffer = value.EncodeToPNG();
            }
            else if (ContentType == MediaType.Image.X_Tga)
            {
                buffer = value.EncodeToTGA();
            }
            else if (ContentType == MediaType.Image.Raw)
            {
                buffer = value.GetRawTextureData();
            }
            else
            {
                throw new NotSupportedException($"Unity doesn't know how to encode {ContentType.Value} image data.");
            }

            prog?.Report(1);
            return buffer;
        }

        public void Serialize(Stream stream, Texture2D value, IProgress prog)
        {
            var subProgs = prog.Split(2);
            var buf = Serialize(value, subProgs[0]);
            var progStream = new ProgressStream(stream, buf.Length, subProgs[1]);
            progStream.Write(buf, 0, buf.Length);
        }

        public Texture2D Deserialize(Stream stream, IProgress prog)
        {
            var mem = new MemoryStream();
            stream.CopyTo(mem);
            stream.Flush();
            var buffer = mem.ToArray();
            return Deserialize(buffer, prog);
        }

        public Texture2D Deserialize(byte[] buffer, IProgress prog)
        {
            prog?.Report(0);
            var info = GetImageInfo(buffer);
            var texture = new Texture2D(
                info.dimensions.width,
                info.dimensions.height,
                info.components == 3 ? TextureFormat.RGB24 : TextureFormat.RGBA32,
                false);

            if (ContentType == MediaType.Image.EXR
                || ContentType == MediaType.Image.Jpeg
                || ContentType == MediaType.Image.Png
                || ContentType == MediaType.Image.X_Tga)
            {
                texture.LoadImage(buffer);
            }
            else if (ContentType == MediaType.Image.Raw)
            {
                texture.LoadRawTextureData(buffer);
            }
            texture.Apply();
            prog?.Report(1);
            return texture;
        }
    }
}
