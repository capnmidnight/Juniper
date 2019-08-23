using System;
using System.IO;

using Juniper.HTTP;
using Juniper.Progress;
using Juniper.Serialization;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;

namespace Juniper.Imaging.ImageSharp
{
    public class ImageSharpCodec : IImageDecoder<Image>
    {
        public ImageSharpCodec(MediaType.Image format)
        {
            Format = format;
        }

        public MediaType.Image Format { get; private set; }

        public int GetWidth(Image img)
        {
            return img.Width;
        }

        public int GetHeight(Image img)
        {
            return img.Height;
        }

        public ImageInfo GetImageInfo(byte[] data, DataSource source = DataSource.None)
        {
            using(var mem = new MemoryStream(data))
            {
                var imageInfo = Image.Identify(mem);
                return new ImageInfo(
                    source,
                    imageInfo.Width,
                    imageInfo.Height,
                    imageInfo.PixelType.BitsPerPixel / 8,
                    Format);
            }
        }

        public void Serialize(Stream stream, Image value, IProgress prog = null)
        {
            if (Format == MediaType.Image.Bmp)
            {
                value.SaveAsBmp(stream);
            }
            else if (Format == MediaType.Image.Gif)
            {
                value.SaveAsGif(stream);
            }
            else if (Format == MediaType.Image.Jpeg)
            {
                value.SaveAsJpeg(stream);
            }
            else if (Format == MediaType.Image.Png)
            {
                value.SaveAsPng(stream);
            }
            else
            {
                throw new NotSupportedException($"Don't know how to save format {Format}");
            }
        }

        public Image Concatenate(Image[,] images, IProgress prog = null)
        {
            this.ValidateImages(images, prog,
                out var rows, out var columns,
                out var firstImage,
                out var tileWidth, out var tileHeight);

            var combined = new Image<Rgba32>(columns * tileWidth, rows * tileHeight);

            combined.Mutate(o =>
            {
                for (int y = 0; y < rows; ++y)
                {
                    for (int x = 0; x < columns; ++x)
                    {
                        var tile = images[y, x];
                        if (tile != null)
                        {
                            o.DrawImage(tile, new Point(x * tileWidth, y * tileHeight), 1);
                        }
                    }
                }
            });

            return combined;
        }

        public Image Deserialize(Stream stream)
        {
            return Image.Load(stream);
        }
    }
}
