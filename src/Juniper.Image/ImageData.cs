using System;
using System.Threading.Tasks;

using Juniper.Progress;
using Juniper.Serialization;

namespace Juniper.Image
{
    /// <summary>
    /// The raw bytes and dimensions of an image that has been loaded either off disk or across the 'net.
    /// </summary>
    public partial class ImageData : ICloneable
    {
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

        public const int BytesPerComponent = sizeof(byte);
        public const int BitsPerComponent = 8 * BytesPerComponent;

        public readonly DataSource source;
        public readonly ImageFormat format;
        public readonly string contentType;
        public readonly string extension;
        public readonly byte[] data;
        public readonly Size dimensions;
        public readonly int stride;
        public readonly int components;
        public readonly int bytesPerSample;
        public readonly int bitsPerSample;

        public ImageData(DataSource source, Size size, int components, ImageFormat format, byte[] data)
        {
            this.source = source;
            this.format = format;
            this.data = data;
            this.components = components;
            dimensions = size;
            contentType = GetContentType(format);
            extension = GetExtension(format);
            stride = size.width * components;
            bytesPerSample = BytesPerComponent * components;
            bitsPerSample = 8 * bytesPerSample;
        }

        public ImageData(DataSource source, int width, int height, int components, ImageFormat format, byte[] data)
            : this(source, new Size(width, height), components, format, data)
        {
        }

        public ImageData(DataSource source, Size size, int components)
            : this(source, size, components, ImageFormat.None, new byte[size.height * size.width * components])
        {
        }

        public ImageData(DataSource source, int width, int height, int components)
            : this(source, new Size(width, height), components)
        {
        }

        private static ImageData Concatenate(int columns, int rows, IProgress prog, params ImageData[] images)
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

            bool anyNotNull = false;
            ImageData firstImage = default;
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

            var combined = new ImageData(
                firstImage.source,
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

                prog?.Report(i, combined.data.Length);
            }

            return combined;
        }

        public static Task<ImageData> Combine6Squares(ImageData north, ImageData east, ImageData west, ImageData south, ImageData up, ImageData down)
        {
            return Task.Run(() => Concatenate(
                1, 6, null,
                west, south, east,
                down, up, north));
        }

        public static Task<ImageData> CombineCross(ImageData north, ImageData east, ImageData west, ImageData south, ImageData down, ImageData up, IProgress prog = null)
        {
            return Task.Run(() => Concatenate(
                4, 3, prog,
                null, up, null, null,
                west, north, east, south,
                null, down, null, null));
        }

        public object Clone()
        {
            return new ImageData(source, dimensions, components, format, (byte[])data.Clone());
        }

        private void RGB2HSV(int index, out float h, out float s, out float v)
        {
            float R = data[index] / 255f;
            float G = data[index + 1] / 255f;
            float B = data[index + 2] / 255f;
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

        private void HSV2RGB(float h, float s, float v, int index)
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
            data[index] = (byte)((r + m) * 255f);
            data[index + 1] = (byte)((g + m) * 255f);
            data[index + 2] = (byte)((b + m) * 255f);
        }

        private ImageData HorizontalSqueeze()
        {
            var resized = new ImageData(
                source,
                dimensions.height,
                dimensions.height,
                components);

            for (int y = 0; y < resized.dimensions.height; ++y)
            {
                for (int x = 0; x < resized.dimensions.width; ++x)
                {
                    HorizontalLerp(resized, x, y);
                }
            }

            return resized;
        }

        private void HorizontalLerp(ImageData output, int outputX, int outputY)
        {
            float inputX = (float)outputX * dimensions.width / output.dimensions.width;
            int inputY = outputY;

            int inputXA = (int)inputX;
            int inputIA = inputY * stride + inputXA * components;
            RGB2HSV(inputIA, out var h1, out var s1, out var v1);

            int inputXB = (int)(inputX + 1) % dimensions.width;
            int inputIB = inputY * stride + inputXB * components;
            RGB2HSV(inputIB, out var h2, out var s2, out var v2);

            float p = 1 - inputX + inputXA;
            float q = 1 - inputXB + inputX;
            float h = h1 * p + h2 * q;
            float s = s1 * p + s2 * q;
            float v = v1 * p + v2 * q;

            int outputIndex = outputY * output.stride + outputX * output.components;
            output.HSV2RGB(h, s, v, outputIndex);
        }

        private ImageData VerticalSqueeze()
        {
            var resized = new ImageData(
                source,
                dimensions.width,
                dimensions.width,
                components);

            for (int y = 0; y < resized.dimensions.height; ++y)
            {
                for (int x = 0; x < resized.dimensions.width; ++x)
                {
                    VerticalLerp(resized, x, y);
                }
            }

            return resized;
        }

        private void VerticalLerp(ImageData output, int outputX, int outputY)
        {
            int inputX = outputX;
            float inputY = (float)outputY * dimensions.height / output.dimensions.height;

            int inputYA = (int)inputY;
            int inputIA = inputYA * stride + inputX * components;
            RGB2HSV(inputIA, out var h1, out var s1, out var v1);

            int inputYB = (int)(inputY + 1) % dimensions.height;
            int inputIB = inputYB * stride + inputX * components;
            RGB2HSV(inputIB, out var h2, out var s2, out var v2);

            float p = 1 - inputY + inputYA;
            float q = 1 - inputYB + inputY;
            float h = h1 * p + h2 * q;
            float s = s1 * p + s2 * q;
            float v = v1 * p + v2 * q;

            int outputIndex = outputY * output.stride + outputX * output.components;
            output.HSV2RGB(h, s, v, outputIndex);
        }

        public ImageData Squarify()
        {
            if (dimensions.width < dimensions.height)
            {
                return VerticalSqueeze();
            }
            else if (dimensions.width > dimensions.height)
            {
                return HorizontalSqueeze();
            }
            else
            {
                return this.Copy();
            }
        }
    }
}