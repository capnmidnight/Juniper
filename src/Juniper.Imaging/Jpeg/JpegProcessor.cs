using BitMiracle.LibJpeg;

using Juniper.Progress;

using System;

namespace Juniper.Imaging
{
    public class JpegProcessor : IImageProcessor<JpegImage>
    {
        public int GetWidth(JpegImage img)
        {
            if (img is null)
            {
                throw new ArgumentNullException(nameof(img));
            }

            return img.Width;
        }

        public int GetHeight(JpegImage img)
        {
            if (img is null)
            {
                throw new ArgumentNullException(nameof(img));
            }

            return img.Height;
        }

        public int GetComponents(JpegImage img)
        {
            if (img is null)
            {
                throw new ArgumentNullException(nameof(img));
            }

            return img.ComponentsPerSample;
        }

        public JpegImage Concatenate(JpegImage[,] images, IProgress prog = null)
        {
            if (images is null)
            {
                throw new ArgumentNullException(nameof(images));
            }

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
                        if (tile is object)
                        {
                            images[tileY, tileX] = null;
                            var tileRowBuffer = tile.GetRow(y).ToBytes();
                            Array.Copy(tileRowBuffer, 0, rowBuffer, tileX * tileWidth * components, tileRowBuffer.Length);
                            tile.Dispose();
                            GC.Collect();
                        }
                    }

                    combined[(tileY * tileHeight) + y] = new SampleRow(rowBuffer, bufferWidth, Units.Bits.PER_BYTE, (byte)components);
                }
            }

            return new JpegImage(combined, Colorspace.RGB);
        }
    }
}