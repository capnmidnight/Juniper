using System;
using System.Drawing;
using System.IO;
using System.Linq;

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

        public void Serialize(Stream stream, Image value, IProgress prog = null)
        {
            value.Save(stream, gdiFormat);
        }

        public Image Deserialize(Stream stream)
        {
            return Image.FromStream(stream);
        }

        public ImageInfo GetImageInfo(byte[] data, DataSource source = DataSource.None)
        {
            if(Format == HTTP.MediaType.Image.Jpeg)
            {
                return ImageInfo.ReadJPEG(data, source);
            }
            else if (Format == HTTP.MediaType.Image.Png)
            {
                return ImageInfo.ReadPNG(data, source);
            }
            else
            {
                using(var image = this.Deserialize(data))
                {
                    return new ImageInfo(
                        source,
                        image.Width, image.Height,
                        image.PixelFormat.ToComponentCount(),
                        image.RawFormat.ToMediaType());
                }
            }
        }

        public int GetWidth(Image img)
        {
            return img.Width;
        }

        public int GetHeight(Image img)
        {
            return img.Height;
        }

        public Image Concatenate(Image[,] images, IProgress prog = null)
        {
            this.ValidateImages(images, prog,
                out var rows, out var columns,
#pragma warning restore IDE0067 // Dispose objects before losing scope
                out var tileWidth,
                out var tileHeight);

            var firstImage = images.Where(img => img != null).First();

            var combined = new Bitmap(
                columns * tileWidth,
                rows * tileHeight,
                firstImage.PixelFormat);

            using (var g = Graphics.FromImage(combined))
            {
                if (firstImage.PixelFormat == System.Drawing.Imaging.PixelFormat.Format32bppArgb)
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