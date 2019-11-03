#if !NETSTANDARD
using System;
using System.Drawing;
using Juniper.Progress;

namespace Juniper.Imaging
{
    public class GDIProcessor : IImageProcessor<Image>
    {
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
            return img.GetComponents();
        }

        public Image Concatenate(Image[,] images, IProgress prog)
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

                for (var y = 0; y < rows; ++y)
                {
                    for (var x = 0; x < columns; ++x)
                    {
                        prog.Report(y * columns + x, rows * columns);
                        var img = images[y, x];
                        if (img != null)
                        {
                            images[y, x] = null;
                            var imageX = y * tileWidth;
                            var imageY = x * tileHeight;
                            g.DrawImageUnscaled(img, imageX, imageY);
                            img.Dispose();
                            GC.Collect();
                        }
                        prog.Report(y * columns + x + 1, rows * columns);
                    }
                }

                g.Flush();
            }

            return combined;
        }
    }
}
#endif