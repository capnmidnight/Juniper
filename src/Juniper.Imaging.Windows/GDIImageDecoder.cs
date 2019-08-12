using System;
using System.Drawing;
using System.IO;

using Juniper.Progress;
using Juniper.Serialization;

using GDIImageFormat = System.Drawing.Imaging.ImageFormat;

namespace Juniper.Imaging.Windows
{
    public class GDIImageDecoder : IImageDecoder<Image>
    {
        private readonly GDIImageFormat gdiFormat;

        public ImageFormat Format { get; private set; }

        public GDIImageDecoder(ImageFormat format)
        {
            Format = format;
            if (format == ImageFormat.JPEG)
            {
                gdiFormat = GDIImageFormat.Jpeg;
            }
            else if(format == ImageFormat.PNG)
            {
                gdiFormat = GDIImageFormat.Png;
            }
            else
            {
                throw new NotSupportedException(format.ToString());
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

        public Image Read(byte[] data, DataSource source = DataSource.None)
        {
            using (var mem = new MemoryStream(data))
            {
                return Read(mem);
            }
        }

#pragma warning disable CA1822 // Mark members as static

        public Image Read(Stream stream)
#pragma warning restore CA1822 // Mark members as static
        {
            return Image.FromStream(stream);
        }

#pragma warning disable CA1822 // Mark members as static

        public Image Read(string fileName)
#pragma warning restore CA1822 // Mark members as static
        {
            return Image.FromFile(fileName);
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
#pragma warning disable IDE0067 // Dispose objects before losing scope
                out var firstImage,
#pragma warning restore IDE0067 // Dispose objects before losing scope
                out var tileWidth, out var tileHeight);

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