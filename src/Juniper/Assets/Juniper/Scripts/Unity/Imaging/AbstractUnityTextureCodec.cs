using System.IO;

using Juniper.Progress;
using Juniper.Serialization;
using Juniper.Streams;

using UnityEngine;

namespace Juniper.Imaging
{
    public abstract class AbstractUnityTextureCodec : IImageDecoder<Texture2D>
    {
        public int GetWidth(Texture2D img)
        {
            return img.width;
        }

        public int GetHeight(Texture2D img)
        {
            return img.height;
        }

        public Texture2D Concatenate(Texture2D[,] images, IProgress prog = null)
        {
            this.ValidateImages(images, prog,
                out var rows, out var columns,
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

        protected abstract byte[] Encode(Texture2D value);

        public abstract ImageInfo GetImageInfo(byte[] data, DataSource source = DataSource.None);

        public abstract HTTP.MediaType.Image Format { get; }
    }
}
