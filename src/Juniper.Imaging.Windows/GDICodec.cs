using System;
using System.Drawing;
using System.IO;

using Juniper.Progress;
using Juniper.Serialization;

using GDIImageFormat = System.Drawing.Imaging.ImageFormat;

namespace Juniper.Imaging.Windows
{
    public class GDICodec : IImageDecoder<Image>
    {
        private readonly GDIImageFormat gdiFormat;

        public HTTP.MediaType.Image Format { get; private set; }

        public GDICodec(HTTP.MediaType.Image format)
        {
            Format = format;
            gdiFormat = format.ToGDIImageFormat();
        }

        public int GetWidth(Image img)
        {
            return img.Width;
        }

        public int GetHeight(Image img)
        {
            return img.Height;
        }

        public int GetComponents(Image img)
        {
            if (img.PixelFormat == System.Drawing.Imaging.PixelFormat.Format32bppArgb)
            {
                return 4;
            }
            else if (img.PixelFormat == System.Drawing.Imaging.PixelFormat.Format24bppRgb)
            {
                return 3;
            }
            else
            {
                throw new NotSupportedException($"Pixel format {img.PixelFormat}");
            }
        }

        public void Serialize(Stream stream, Image value, IProgress prog = null)
        {
            value.Save(stream, gdiFormat);
        }

        public Image Deserialize(Stream stream)
        {
            return Image.FromStream(stream);
        }

        public ImageInfo GetImageInfo(byte[] data)
        {
            if(Format == HTTP.MediaType.Image.Jpeg)
            {
                return ImageInfo.ReadJPEG(data);
            }
            else if (Format == HTTP.MediaType.Image.Png)
            {
                return ImageInfo.ReadPNG(data);
            }
            else
            {
                using(var image = this.Deserialize(data))
                {
                    return new ImageInfo(
                        image.Width,
                        image.Height,
                        GetComponents(image),
                        image.RawFormat.ToMediaType());
                }
            }
        }

        public Image Concatenate(Image[,] images, IProgress prog = null)
        {
            this.ValidateImages(images, prog,
                out var rows, out var columns, out var components,
                out var tileWidth,
                out var tileHeight);

            var combined = new Bitmap(
                columns * tileWidth,
                rows * tileHeight,
                components.ToGDIPixelFormat());

            using (var g = Graphics.FromImage(combined))
            {
                if (components == 4)
                {
                    g.Clear(Color.Transparent);
                }
                else
                {
                    g.Clear(Color.Black);
                }

                for (var i = 0; i < images.Length; ++i)
                {
                    var tileX = i % columns;
                    var tileY = rows - (i / columns) - 1;
                    var img = images[tileY, tileX];
                    if (img != null)
                    {
                        images[tileX, tileY] = null;
                        var imageX = tileX * tileWidth;
                        var imageY = tileY * tileHeight;
                        g.DrawImageUnscaled(img, imageX, imageY);
                        img.Dispose();
                        GC.Collect();
                    }
                    prog?.Report(i, images.Length);
                }

                g.Flush();
            }

            return combined;
        }
    }
}