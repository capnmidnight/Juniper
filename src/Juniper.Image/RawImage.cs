using System;
using System.Threading.Tasks;

namespace Juniper.Image
{
    /// <summary>
    /// The raw bytes and dimensions of an image that has been loaded either off disk or across the 'net.
    /// </summary>
    public class RawImage : ICloneable
    {
        public enum ImageSource
        {
            None,
            File,
            Network
        }

        public const int BytesPerComponent = sizeof(byte);
        public const int BitsPerComponent = 8 * BytesPerComponent;

        public readonly ImageSource source;
        public readonly byte[] data;
        public readonly Size dimensions;
        public readonly int stride;
        public readonly int components;
        public readonly int bytesPerSample;
        public readonly int bitsPerSample;

        public RawImage(ImageSource source, Size dimensions, byte[] data)
        {
            this.source = source;
            this.dimensions = dimensions;
            this.data = data;
            stride = data.Length / dimensions.height;
            components = stride / dimensions.width;
            bytesPerSample = BytesPerComponent * components;
            bitsPerSample = 8 * bytesPerSample;
        }

        public RawImage(ImageSource source, int width, int height, byte[] data)
            : this(source, new Size(width, height), data)
        {
        }

        public object Clone()
        {
            return new RawImage(source, dimensions, (byte[])data.Clone());
        }

        private static Task<RawImage> CombineTilesAsync(int columns, int rows, params RawImage[] images)
        {
            return Task.Run(() => CombineTiles(columns, rows, images));
        }

        private static RawImage CombineTiles(int columns, int rows, params RawImage[] images)
        {
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

            bool anyNotNull = false;
            RawImage firstImage = default;
            for (int i = 0; i < images.Length; ++i)
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

            var imageStride = firstImage.stride;
            var imageHeight = firstImage.dimensions.height;
            var bufferStride = columns * imageStride;
            var bufferHeight = rows * imageHeight;
            var bufferLength = bufferStride * bufferHeight;
            var buffer = new byte[bufferLength];
            for(
                int bufferI = 0,
                    tileX = 0;
                bufferI < bufferLength;
                bufferI += imageStride,
                    tileX = (tileX + 1) % columns)
            {
                var bufferY = bufferI / bufferStride;
                var tileY = bufferY / imageHeight;
                var tileI = tileY * columns + tileX;
                var tile = images[tileI];
                if (tile != null)
                {
                    var imageY = bufferY / columns;
                    var imageI = imageY * imageStride;
                    Array.Copy(tile.data, imageI, buffer, bufferI, imageStride);
                }
            }

            return new RawImage(
                ImageSource.None,
                columns * firstImage.dimensions.width,
                rows * firstImage.dimensions.height,
                buffer);
        }

        public static Task<RawImage> Combine6Squares(RawImage north, RawImage east, RawImage west, RawImage south, RawImage up, RawImage down)
        {
            return CombineTilesAsync(
                3, 2,
                west, south, east,
                down, up, north);
        }

        public static Task<RawImage> CombineCross(RawImage north, RawImage east, RawImage west, RawImage south, RawImage up, RawImage down)
        {
            return CombineTilesAsync(
                4, 3,
                null, up, null, null,
                west, north, east, south,
                null, down, null, null);
        }
    }
}