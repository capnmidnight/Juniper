
using System;
using Juniper.Progress;

namespace Juniper.Imaging
{
    public class ImageDataProcessor : IImageProcessor<ImageData>
    {
        public ImageData Concatenate(ImageData[,] tiles, IProgress prog)
        {
            var rows = tiles.GetLength(0);
            var columns = tiles.GetLength(1);
            var len = rows * columns;
            var progs = prog.Split(len);
            var firstImage = tiles[0, 0];
            var tileWidth = firstImage.info.dimensions.width;
            var tileHeight = firstImage.info.dimensions.height;
            var bufferSize = new Size(
                    tileWidth * columns,
                    tileHeight * rows);
            var info = new ImageInfo(bufferSize, firstImage.info.components);
            var bufferData = new byte[len];

            for (var tileY = 0; tileY < rows; ++tileY)
            {
                for (var tileX = 0; tileX < columns; ++tileX)
                {
                    var tileI = tileY * columns + tileX;
                    var p = progs[tileI];
                    p.Report(0);
                    var tile = tiles[tileY, tileX];
                    for (var y = 0; y < tileHeight; ++y)
                    {
                        var tileBufferI = y * tileWidth;
                        var bufferI = bufferSize.width * (tileHeight * tileY + y) + tileX * tileWidth;
                        Array.Copy(tile.data, tileBufferI, bufferData, bufferI, tileWidth);
                    }
                    p.Report(1);
                }
            }

            return new ImageData(info, bufferData);
        }

        public int GetComponents(ImageData img)
        {
            return img.info.components;
        }

        public int GetWidth(ImageData img)
        {
            return img.info.dimensions.width;
        }

        public int GetHeight(ImageData img)
        {
            return img.info.dimensions.height;
        }
    }
}