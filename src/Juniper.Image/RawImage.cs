using System;
using System.Threading.Tasks;

namespace Juniper.Image
{
    /// <summary>
    /// The raw bytes and dimensions of an image that has been loaded either off disk or across the 'net.
    /// </summary>
    public struct RawImage
    {
        public enum ImageSource
        {
            None,
            File,
            Network
        }

        public ImageSource source;
        public byte[] data;
        public int width, height;

        public int stride { get { return data.Length / height; } }
        public int components { get { return stride / width; } }

        public void Mirror()
        {
            var output = new byte[data.Length];
            for(int p = 0, c = components; p < data.Length; p += c)
            {
                Array.Copy(data, p, output, data.Length - p - c, c);
            }
            data = output;
        }


        private static Task<RawImage> CombineTilesAsync(int columns, int rows, params RawImage?[] images)
        {
            return Task.Run(() => CombineTiles(columns, rows, images));
        }

        private static RawImage CombineTiles(int columns, int rows, params RawImage?[] images)
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
                        firstImage = images[i].Value;
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
                        var img = images[i].Value;
                        int destinationIndex = bufferRow * bufferStride + bufferColumn * imageStride;
                        for (int imageRow = 0; imageRow < img.data.Length; imageRow += imageStride)
                        {
                            Array.Copy(img.data, imageRow, destinationBuffer, destinationIndex, imageStride);
                        }
                    }
                }
            }

            return new RawImage
            {
                source = ImageSource.None,
                width = columns * firstImage.width,
                height = rows * firstImage.height,
                data = destinationBuffer
            };
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
