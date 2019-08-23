using System;
using System.IO;

using Juniper.Progress;
using Juniper.Serialization;

namespace Juniper.Imaging
{
    public abstract class AbstractImageDataDecoder : IImageDecoder<ImageData>
    {
        public int GetWidth(ImageData img)
        {
            return img.info.dimensions.width;
        }

        public int GetHeight(ImageData img)
        {
            return img.info.dimensions.height;
        }

        public ImageData Concatenate(ImageData[,] images, IProgress prog = null)
        {
            this.ValidateImages(images, prog,
                out var rows, out var columns,
                out var firstImage,
                out var tileWidth, out var tileHeight);

            var combined = new ImageData(
                firstImage.info.source,
                columns * tileWidth,
                rows * tileHeight,
                firstImage.info.components);

            for (var i = 0; i < combined.data.Length; i += firstImage.info.stride)
            {
                var bufferX = i % combined.info.stride;
                var bufferY = i / combined.info.stride;
                var tileX = bufferX / firstImage.info.stride;
                var tileY = bufferY / tileHeight;
                var tile = images[tileY, tileX];
                if (tile != null)
                {
                    var imageY = bufferY % tileHeight;
                    var imageI = imageY * firstImage.info.stride;
                    Array.Copy(tile.data, imageI, combined.data, i, firstImage.info.stride);
                }

                prog?.Report(i, combined.data.Length);
            }

            return combined;
        }

        public abstract HTTP.MediaType.Image Format { get; }

        public abstract ImageInfo GetImageInfo(byte[] data, DataSource source = DataSource.None);

        public abstract void Serialize(Stream stream, ImageData value, IProgress prog = null);

        public abstract ImageData Deserialize(Stream stream);
    }
}