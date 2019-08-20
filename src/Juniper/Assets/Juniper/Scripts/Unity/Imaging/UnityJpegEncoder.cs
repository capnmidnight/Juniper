using System.IO;

using Juniper.Progress;
using Juniper.Serialization;
using Juniper.Streams;

using UnityEngine;

namespace Juniper.Imaging
{
    public class UnityJpegEncoder : IImageDecoder<Texture2D>
    {
        private readonly int encodingQuality;

        public UnityJpegEncoder(int encodingQuality = 100)
        {
            this.encodingQuality = encodingQuality;
        }
        
        public int GetWidth(Texture2D img)
        {
            return img.width;
        }

        public int GetHeight(Texture2D img)
        {
            return img.height;
        }

        public Texture2D Read(byte[] data, DataSource source = DataSource.None)
        {
            for (var i = 0; i < data.Length - 1; i++)
            {
                var b = data[i];
                var b2 = data[i + 1];
                if (b == byte.MaxValue && b2 == 192)
                {
                    var b3 = data[i + 5];
                    var b4 = data[i + 6];
                    var b5 = data[i + 7];
                    var b6 = data[i + 8];
                    var width = (b5 << 8) | b6;
                    var height = (b3 << 8) | b4;
                    var texture = new Texture2D(width, height);
                    texture.LoadImage(data);
                    return texture;
                }
            }

            return null;
        }

        public Texture2D Concatenate(Texture2D[,] images, IProgress prog = null)
        {
            this.ValidateImages(images, prog, 
                out var rows, out var columns, 
                out var firstImage, 
                out var tileWidth, out var tileHeight);

            var outputImage = new Texture2D(columns * tileWidth, rows * tileHeight);
            for(int y = 0; y < rows; ++y)
            {
                for(int x = 0; x < columns; ++x)
                {
                    var img = images[y, x];
                    if(img != null)
                    {
                        var pixels = img.GetPixels();
                        outputImage.SetPixels(x * tileWidth, y * tileHeight, tileWidth, tileHeight, pixels);
                    }
                }
            }

            return outputImage;
        }

        public ImageFormat Format { get { return ImageFormat.JPEG; } }

        public void Serialize(Stream stream, Texture2D value, IProgress prog = null)
        {
            var buf = value.EncodeToJPG(encodingQuality);
            var progStream = new ProgressStream(stream, buf.Length, prog);
            progStream.Write(buf, 0, buf.Length);
        }

        public Texture2D Deserialize(Stream stream)
        {
            var mem = new MemoryStream();
            stream.CopyTo(mem);
            return Read(mem.ToArray(), stream.DetermineSource());
        }
    }
}
