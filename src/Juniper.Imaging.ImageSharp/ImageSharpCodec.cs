using System;
using System.IO;

using Juniper.HTTP;
using Juniper.Progress;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;

namespace Juniper.Imaging.ImageSharp
{
    public class ImageSharpCodec<PixelT> : IImageCodec<Image<PixelT>>
        where PixelT : struct, IPixel, IPixel<PixelT>
    {
        public ImageSharpCodec(MediaType.Image format)
        {
            ContentType = format;
        }

        public MediaType ContentType { get; private set; }

        public int GetWidth(Image<PixelT> img)
        {
            return img.Width;
        }

        public int GetHeight(Image<PixelT> img)
        {
            return img.Height;
        }

        public int GetComponents(Image<PixelT> img)
        {
            return img.PixelType.BitsPerPixel / 8;
        }

        public ImageInfo GetImageInfo(byte[] data)
        {
            using (var mem = new MemoryStream(data))
            {
                var imageInfo = Image.Identify(mem);
                return new ImageInfo(
                    imageInfo.Width,
                    imageInfo.Height,
                    imageInfo.PixelType.BitsPerPixel / 8);
            }
        }

        public Image<PixelT> Deserialize(Stream stream)
        {
            return Image.Load<PixelT>(stream);
        }

        public void Serialize(Stream stream, Image<PixelT> value, IProgress prog = null)
        {
            if (ContentType == MediaType.Image.Bmp)
            {
                value.SaveAsBmp(stream);
            }
            else if (ContentType == MediaType.Image.Gif)
            {
                value.SaveAsGif(stream);
            }
            else if (ContentType == MediaType.Image.Jpeg)
            {
                value.SaveAsJpeg(stream);
            }
            else if (ContentType == MediaType.Image.Png)
            {
                value.SaveAsPng(stream);
            }
            else
            {
                throw new NotSupportedException($"Don't know how to save format {ContentType}");
            }
        }

        public Image<PixelT> Concatenate(Image<PixelT>[,] images, IProgress prog = null)
        {
            this.ValidateImages(images, prog,
                out var rows, out var columns, out var components,
                out var tileWidth,
                out var tileHeight);

            var combined = new Image<PixelT>(columns * tileWidth, rows * tileHeight);

            combined.Mutate(o =>
            {
                for (var y = 0; y < rows; ++y)
                {
                    for (var x = 0; x < columns; ++x)
                    {
                        var tile = images[y, x];
                        if (tile != null)
                        {
                            images[y, x] = null;
                            o.DrawImage(tile, new Point(x * tileWidth, y * tileHeight), 1);
                            tile.Dispose();
                            GC.Collect();
                        }
                    }
                }
            });

            return combined;
        }
    }
}
