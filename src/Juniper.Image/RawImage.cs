using System;
using System.IO;
using System.Threading.Tasks;

using Juniper.Progress;

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

        public RawImage(int width, int height, int components)
            : this(ImageSource.None, width, height, new byte[height * width * components])
        {}

        public object Clone()
        {
            return new RawImage(source, dimensions, (byte[])data.Clone());
        }

        public static ImageSource DetermineSource(Stream imageStream)
        {
            var source = ImageSource.None;
            if (imageStream is FileStream)
            {
                source = ImageSource.File;
            }
            else if (imageStream is CachingStream)
            {
                source = ImageSource.Network;
            }

            return source;
        }

        public static string GetContentType(ImageFormat format)
        {
            switch (format)
            {
                case ImageFormat.JPEG: return "image/jpeg";
                case ImageFormat.PNG: return "image/png";
                default: return "application/unknown";
            }
        }

        public static string GetExtension(ImageFormat format)
        {
            switch (format)
            {
                case ImageFormat.JPEG: return "jpeg";
                case ImageFormat.PNG: return "png";
                default: return "raw";
            }
        }

        public static int GetRowIndex(int numRows, int i, bool flipImage)
        {
            int rowIndex = i;
            if (flipImage)
            {
                rowIndex = numRows - i - 1;
            }

            return rowIndex;
        }

        private static void RGB2HSV(RawImage image, int index, out float h, out float s, out float v)
        {
            float R = image.data[index] / 255f;
            float G = image.data[index + 1] / 255f;
            float B = image.data[index + 2] / 255f;
            float max = R;
            float min = R;

            if (G > max) max = B;
            if (G < min) min = B;
            if (B > max) max = B;
            if (B < min) min = B;

            float delta = max - min;

            h = 0;
            if (delta > 0)
            {
                if (max == R) h = (G - B) / delta;
                if (max == G) h = 2 + (B - R) / delta;
                if (max == B) h = 4 + (R - G) / delta;
            }

            h *= 60;
            if (h < 0) h += 360;
            if (h >= 360) h -= 360;

            s = 0;
            if (max > 0) s = (max - min) / max;

            v = max;
        }

        private static void HSV2RGB(float h, float s, float v, RawImage image, int index)
        {
            float delta = v * s;
            h /= 60;
            float x = delta * (1 - Math.Abs((h % 2) - 1));
            float r = 0;
            float g = 0;
            float b = 0;
            if (h <= 1)
            {
                r = delta;
                g = x;
            }
            else if (h <= 2)
            {
                r = x;
                g = delta;
            }
            else if (h <= 3)
            {
                g = delta;
                b = x;
            }
            else if (h <= 4)
            {
                g = x;
                b = delta;
            }
            else if (h <= 5)
            {
                r = x;
                b = delta;
            }
            else
            {
                r = delta;
                b = x;
            }

            float m = v - delta;
            image.data[index] = (byte)((r + m) * 255f);
            image.data[index + 1] = (byte)((g + m) * 255f);
            image.data[index + 2] = (byte)((b + m) * 255f);
        }

        private static void HorizontalLerp(RawImage input, RawImage output, int outputX, int outputY)
        {
            float inputX = (float)outputX * input.dimensions.width / output.dimensions.width;
            int inputXA = (int)inputX;
            int inputXB = (inputXA + 1) % input.dimensions.width;
            int inputIA = outputY * input.stride + inputXA * input.components;
            int inputIB = outputY * input.stride + inputXB * input.components;
            RGB2HSV(input, inputIA, out var h1, out var s1, out var v1);
            RGB2HSV(input, inputIB, out var h2, out var s2, out var v2);
            float p = 1 - inputX + inputXA;
            float q = 1 - inputXB + inputX;
            float h = h1 * p + h2 * q;
            float s = s1 * p + s2 * q;
            float v = v1 * p + v2 * q;

            int outputIndex = outputY * output.stride + outputX * output.components;
            HSV2RGB(h, s, v, output, outputIndex);
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

            var combined = new RawImage(
                columns * firstImage.dimensions.width,
                rows * firstImage.dimensions.height,
                firstImage.components);

            for (int i = 0; i < combined.data.Length; i += firstImage.stride)
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
            }

            return combined; // Squarify(combined);
        }

        private static RawImage Squarify(RawImage combined)
        {
            var resized = new RawImage(
                            combined.dimensions.height,
                            combined.dimensions.height,
                            combined.components);

            for (int y = 0; y < resized.dimensions.height; ++y)
            {
                for (int x = 0; x < resized.dimensions.width; ++x)
                {
                    HorizontalLerp(combined, resized, x, y);
                }
            }

            return resized;
        }

        public static Task<RawImage> Combine6Squares(RawImage north, RawImage east, RawImage west, RawImage south, RawImage up, RawImage down)
        {
            return CombineTilesAsync(
                1, 6,
                west, south, east,
                down, up, north);
        }

        public static Task<RawImage> CombineCross(RawImage north, RawImage east, RawImage west, RawImage south, RawImage down, RawImage up)
        {
            return CombineTilesAsync(
                4, 3,
                null, up, null, null,
                west, north, east, south,
                null, down, null, null);
        }
    }
}