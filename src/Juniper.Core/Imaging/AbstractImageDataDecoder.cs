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
            return img.dimensions.width;
        }

        public int GetHeight(ImageData img)
        {
            return img.dimensions.height;
        }

        public ImageData Concatenate(ImageData[,] images, IProgress prog = null)
        {
            this.ValidateImages(images, prog,
                out var rows, out var columns,
                out var firstImage,
                out var tileWidth, out var tileHeight);

            var combined = new ImageData(
                firstImage.source,
                columns * tileWidth,
                rows * tileHeight,
                firstImage.components);

            for (var i = 0; i < combined.data.Length; i += firstImage.stride)
            {
                var bufferX = i % combined.stride;
                var bufferY = i / combined.stride;
                var tileX = bufferX / firstImage.stride;
                var tileY = bufferY / tileHeight;
                var tile = images[tileY, tileX];
                if (tile != null)
                {
                    var imageY = bufferY % tileHeight;
                    var imageI = imageY * firstImage.stride;
                    Array.Copy(tile.data, imageI, combined.data, i, firstImage.stride);
                }

                prog?.Report(i, combined.data.Length);
            }

            return combined;
        }

        public abstract HTTP.MediaType.Image Format { get; }

        public abstract ImageData Read(byte[] data, DataSource source = DataSource.None);

        public abstract void Serialize(Stream stream, ImageData value, IProgress prog = null);

        public abstract ImageData Deserialize(Stream stream);
    }
}