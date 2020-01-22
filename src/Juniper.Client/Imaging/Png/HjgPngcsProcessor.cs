using System;

using Hjg.Pngcs;
using Juniper.IO;

namespace Juniper.Imaging
{
    public class HjgPngcsProcessor : IImageProcessor<ImageLines>
    {
        public int GetWidth(ImageLines img)
        {
            if (img is null)
            {
                throw new ArgumentNullException(nameof(img));
            }

            return img.ImgInfo.BytesPerRow / img.ImgInfo.BytesPixel;
        }

        public int GetHeight(ImageLines img)
        {
            if (img is null)
            {
                throw new ArgumentNullException(nameof(img));
            }

            return img.Nrows;
        }

        public int GetComponents(ImageLines img)
        {
            if (img is null)
            {
                throw new ArgumentNullException(nameof(img));
            }

            return img.ImgInfo.BytesPerRow / img.ImgInfo.BytesPixel;
        }

        public ImageLines Concatenate(ImageLines[,] images, IProgress prog = null)
        {
            if (images is null)
            {
                throw new ArgumentNullException(nameof(images));
            }

            this.ValidateImages(images, prog,
                out var rows, out var columns, out var components,
                out var tileWidth,
                out var tileHeight);

            var combinedInfo = new Hjg.Pngcs.ImageInfo(
                columns * tileWidth,
                rows * tileHeight,
                Units.Bits.PER_BYTE,
                components == 4);

            var combinedLines = new ImageLines(
                combinedInfo,
                ImageLine.ESampleType.BYTE,
                true,
                0,
                rows * tileHeight,
                rows * tileHeight * components);

            for (var y = 0; y < rows; ++y)
            {
                for (var x = 0; x < columns; ++x)
                {
                    var tile = images[y, x];
                    if (tile is object)
                    {
                        for (var i = 0; i < tileHeight; ++i)
                        {
                            var bufferY = (y * tileHeight) + i;
                            var bufferX = x * tileWidth;
                            var bufferLine = combinedLines.ScanlinesB[bufferY];
                            var tileLine = tile.ScanlinesB[i];
                            Array.Copy(tileLine, 0, bufferLine, bufferX, tileLine.Length);
                        }
                    }
                }
            }

            return combinedLines;
        }
    }
}