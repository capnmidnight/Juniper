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
        private readonly GDIImageFormat serializationFormat;

        public GDIImageDecoder(GDIImageFormat serializationFormat)
        {
            this.serializationFormat = serializationFormat;
        }

        public void Serialize(Stream stream, Image value, IProgress prog = null)
        {
            value.Save(stream, serializationFormat);
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

        public Image Read(Stream stream)
        {
            return Image.FromStream(stream);
        }

        public Image Read(string fileName)
        {
            return Image.FromFile(fileName);
        }

        public Image Concatenate(int columns, int rows, IProgress prog, params Image[] images)
        {
            prog?.Report(0);

            if (images == null)
            {
                throw new ArgumentNullException($"Parameter {nameof(images)} must not be null.");
            }

            if (images.Length == 0)
            {
                throw new ArgumentException($"Parameter {nameof(images)} must have at least one image.");
            }

            var numTiles = columns * rows;
            if (images.Length != numTiles)
            {
                throw new ArgumentException($"Expected {nameof(images)} parameter to be {numTiles} long, but it was {images.Length} long.");
            }

            var anyNotNull = false;
            Image firstImage = default;
            for (var i = 0; i < images.Length; ++i)
            {
                var img = images[i];
                if (img != null)
                {
                    if (!anyNotNull)
                    {
                        firstImage = images[i];
                    }

                    anyNotNull = true;
                    if (img?.Width != firstImage.Width || img?.Height != firstImage.Height)
                    {
                        throw new ArgumentException($"All elements of {nameof(images)} must be the same width and height. Image {i} did not match image 0.");
                    }
                }
            }

            if (!anyNotNull)
            {
                throw new ArgumentNullException($"Expected at least one image in {nameof(images)} to be not null");
            }

            var combined = new Bitmap(
                columns * firstImage.Width,
                rows * firstImage.Height,
                firstImage.PixelFormat);

            using (var g = Graphics.FromImage(combined))
            {
                g.Clear(Color.Black);

                for (var i = 0; i < images.Length; ++i)
                {
                    var img = images[i];
                    if (img != null)
                    {
                        var tileX = i % columns;
                        var tileY = rows - (i / columns) - 1;
                        var imageX = tileX * firstImage.Width;
                        var imageY = tileY * firstImage.Height;
                        g.DrawImageUnscaled(img, imageX, imageY);
                    }
                    prog?.Report(i, images.Length);
                }

                g.Flush();
            }
            return combined;
        }
    }
}
