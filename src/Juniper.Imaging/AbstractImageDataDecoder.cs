using System;
using System.IO;

using Juniper.Progress;
using Juniper.Serialization;

namespace Juniper.Imaging
{
    public abstract class AbstractImageDataDecoder : IImageDecoder<ImageData>
    {
        public ImageData Concatenate(int columns, int rows, IProgress prog, params ImageData[] images)
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
            ImageData firstImage = default;
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
                    if (img?.dimensions.width != firstImage.dimensions.width || img?.dimensions.height != firstImage.dimensions.height)
                    {
                        throw new ArgumentException($"All elements of {nameof(images)} must be the same width and height. Image {i} did not match image 0.");
                    }
                }
            }

            if (!anyNotNull)
            {
                throw new ArgumentNullException($"Expected at least one image in {nameof(images)} to be not null");
            }

            var combined = new ImageData(
                firstImage.source,
                columns * firstImage.dimensions.width,
                rows * firstImage.dimensions.height,
                firstImage.components);

            for (var i = 0; i < combined.data.Length; i += firstImage.stride)
            {
                var bufferX = i % combined.stride;
                var bufferY = i / combined.stride;
                var tileX = bufferX / firstImage.stride;
                var tileY = bufferY / firstImage.dimensions.height;
                var tileI = tileY * columns + tileX;
                var tile = images[tileI];
                if (tile != null)
                {
                    var imageY = bufferY % firstImage.dimensions.height;
                    var imageI = imageY * firstImage.stride;
                    Array.Copy(tile.data, imageI, combined.data, i, firstImage.stride);
                }

                prog?.Report(i, combined.data.Length);
            }

            return combined;
        }

        public abstract ImageData Read(byte[] data, DataSource source = DataSource.None);

        public abstract void Serialize(Stream stream, ImageData value, IProgress prog = null);

        public abstract ImageData Deserialize(Stream stream);
    }
}
