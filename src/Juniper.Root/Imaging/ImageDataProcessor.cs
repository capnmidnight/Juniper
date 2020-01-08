using System;

using Juniper.Progress;

namespace Juniper.Imaging
{
    public class ImageDataProcessor : IImageProcessor<ImageData>
    {
        public ImageData Concatenate(ImageData[,] images, IProgress prog)
        {
            var rows = images.GetLength(0);
            var columns = images.GetLength(1);
            var len = rows * columns;
            var progs = prog.Split(len);
            var firstImage = images[0, 0];
            var tileWidth = firstImage.Info.Dimensions.Width;
            var tileHeight = firstImage.Info.Dimensions.Height;
            var bufferSize = new Size(
                    tileWidth * columns,
                    tileHeight * rows);
            var info = new ImageInfo(bufferSize, firstImage.Info.Components);
            var bufferData = new byte[len];

            for (var tileY = 0; tileY < rows; ++tileY)
            {
                for (var tileX = 0; tileX < columns; ++tileX)
                {
                    var tileI = (tileY * columns) + tileX;
                    var p = progs[tileI];
                    p.Report(0);
                    var tile = images[tileY, tileX];
                    for (var y = 0; y < tileHeight; ++y)
                    {
                        var tileBufferI = y * tileWidth;
                        var bufferI = (bufferSize.Width * ((tileHeight * tileY) + y)) + (tileX * tileWidth);
                        Array.Copy(tile.Data, tileBufferI, bufferData, bufferI, tileWidth);
                    }

                    p.Report(1);
                }
            }

            return new ImageData(info, bufferData);
        }

        public int GetComponents(ImageData img)
        {
            return img.Info.Components;
        }

        public int GetWidth(ImageData img)
        {
            return img.Info.Dimensions.Width;
        }

        public int GetHeight(ImageData img)
        {
            return img.Info.Dimensions.Height;
        }
    }
}