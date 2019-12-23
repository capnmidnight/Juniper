using System;

using BitMiracle.LibJpeg;

using Juniper.Progress;

namespace Juniper.Imaging
{
    public class LibJpegNETProcessor : IImageProcessor<JpegImage>
    {
        public int GetWidth(JpegImage img)
        {
            return img.Width;
        }

        public int GetHeight(JpegImage img)
        {
            return img.Height;
        }

        public int GetComponents(JpegImage img)
        {
            return img.ComponentsPerSample;
        }

        public JpegImage Concatenate(JpegImage[,] images, IProgress prog)
        {
            this.ValidateImages(images, prog,
                out var rows, out var columns, out var components,
                out var tileWidth,
                out var tileHeight);

            var bufferWidth = columns * tileWidth;
            var bufferHeight = rows * tileHeight;
            var combined = new SampleRow[bufferHeight];
            for (var tileY = 0; tileY < rows; ++tileY)
            {
                for (var y = 0; y < tileHeight; ++y)
                {
                    var rowBuffer = new byte[bufferWidth];
                    for (var tileX = 0; tileX < columns; ++tileX)
                    {
                        var tile = images[tileY, tileX];
                        if (tile != null)
                        {
                            images[tileY, tileX] = null;
                            var tileRowBuffer = tile.GetRow(y).ToBytes();
                            Array.Copy(tileRowBuffer, 0, rowBuffer, tileX * tileWidth * components, tileRowBuffer.Length);
                            tile.Dispose();
                            GC.Collect();
                        }
                    }

                    combined[(tileY * tileHeight) + y] = new SampleRow(rowBuffer, bufferWidth, 8, (byte)components);
                }
            }

            return new JpegImage(combined, Colorspace.RGB);
        }
    }
}