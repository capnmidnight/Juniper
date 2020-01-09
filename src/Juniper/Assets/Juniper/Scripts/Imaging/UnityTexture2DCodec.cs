using System;
using System.IO;

using UnityEngine;

namespace Juniper.Imaging
{
    public class UnityTexture2DCodec : IImageCodec<Texture2D>
    {
        private readonly Texture2D.EXRFlags exrFlags;
        private readonly int jpegEncodingQuality;

        public UnityTexture2DCodec(MediaType.Image format)
        {
            if (format != MediaType.Image.EXR
                && format != MediaType.Image.Jpeg
                && format != MediaType.Image.Png
                && format != MediaType.Image.X_Tga
                && format != MediaType.Image.Raw)
            {
                throw new NotSupportedException($"Unity doesn't know how to encode {format.Value} image data.");
            }

            ContentType = format;

            exrFlags = Texture2D.EXRFlags.None;
            jpegEncodingQuality = 80;
        }

        public UnityTexture2DCodec(Texture2D.EXRFlags exrFlags)
            : this(MediaType.Image.EXR)
        {
            this.exrFlags = exrFlags;
        }

        public UnityTexture2DCodec(int jpegEncodingQuality = 80)
            : this(MediaType.Image.Jpeg)
        {
            this.jpegEncodingQuality = jpegEncodingQuality;
        }

        public MediaType.Image ContentType
        {
            get;
            private set;
        }

        private ImageInfo GetImageInfo(byte[] data)
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
                throw new NotSupportedException($"Don't know how to read image data for type {ContentType}");
            }
        }

        public byte[] Serialize(Texture2D value)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

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

            return buffer;
        }

        public long Serialize(Stream stream, Texture2D value)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var buffer = Serialize(value);
            stream.Write(buffer, 0, buffer.Length);
            return buffer.Length;
        }

        public Texture2D Deserialize(Stream stream)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            var mem = new MemoryStream();
            stream.CopyTo(mem);
            stream.Flush();
            var buffer = mem.ToArray();
            return Deserialize(buffer);
        }

        public Texture2D Deserialize(byte[] buffer)
        {
            if (buffer is null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            var info = GetImageInfo(buffer);
            return JuniperSystem.OnMainThread(() =>
                MakeTexture(info, buffer));
        }

        private Texture2D MakeTexture(ImageInfo info, byte[] buffer)
        {
            var texture = new Texture2D(
                info.Dimensions.Width,
                info.Dimensions.Height,
                info.Components == 3
                    ? TextureFormat.RGB24
                    : TextureFormat.RGBA32,
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
            return texture;
        }
    }
}
