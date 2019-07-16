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

        public readonly ImageSource source;
        public readonly byte[] data;
        public readonly int width, height;

        public RawImage(ImageSource source, int width, int height, byte[] data)
        {
            this.source = source;
            this.width = width;
            this.height = height;
            this.data = data;
        }

        public object Clone()
        {
            return new RawImage(source, width, height, (byte[])data.Clone());
        }

        public RawImage CreateCopy()
        {
            return (RawImage)Clone();
        }

        public int stride { get { return data.Length / height; } }
        public int components { get { return stride / width; } }

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

                    if (img?.width != firstImage.width || img?.height != firstImage.height)
                    {
                        throw new ArgumentException($"All elements of {nameof(images)} must be the same width and height. Image {i} did not match image 0.");
                    }
                }
            }

            if (!anyNotNull)
            {
                throw new ArgumentNullException($"Expected at least one image in {nameof(images)} to be not null");
            }

            var destinationBuffer = new byte[numTiles * firstImage.data.Length];
            var imageStride = firstImage.data.Length / firstImage.height;
            var bufferStride = imageStride * columns;
            for (int bufferRow = 0; bufferRow < rows; ++bufferRow)
            {
                for (int bufferColumn = 0; bufferColumn < columns; ++bufferColumn)
                {
                    int i = bufferRow * columns + bufferColumn;
                    if (images[i] != null)
                    {
                        var img = images[i];
                        int destinationIndex = bufferRow * bufferStride + bufferColumn * imageStride;
                        for (int imageRow = 0; imageRow < img.data.Length; imageRow += imageStride)
                        {
                            Array.Copy(img.data, imageRow, destinationBuffer, destinationIndex, imageStride);
                        }
                    }
                }
            }

            return new RawImage(
                ImageSource.None,
                columns * firstImage.width,
                rows * firstImage.height,
                destinationBuffer);
        }

        public static Task<RawImage> Combine6Squares(RawImage north, RawImage east, RawImage west, RawImage south, RawImage up, RawImage down)
        {
            return CombineTilesAsync(
                3, 2,
                north, east, west,
                south, up, down);
        }

        public static Task<RawImage> CombineCubemap(RawImage north, RawImage east, RawImage west, RawImage south, RawImage up, RawImage down)
        {
            return CombineTilesAsync(
                4, 3,
                null, up, null, null,
                west, north, east, south,
                null, down, null, null);
        }
    }
}
