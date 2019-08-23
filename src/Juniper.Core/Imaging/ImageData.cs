using System;
using Juniper.HTTP;
using Juniper.Serialization;

namespace Juniper.Imaging
{
    /// <summary>
    /// The raw bytes and dimensions of an image that has been loaded either off disk or across the 'net.
    /// </summary>
    public partial class ImageData : ICloneable
    {
        public static T[,] CubeCross<T>(T[] images)
        {
            return new T[,]
            {
                { default, images[0], default, default },
                { images[1], images[2], images[3], images[4] },
                { default, images[5], default, default }
            };
        }

        public static int GetRowIndex(int numRows, int i, bool flipImage)
        {
            var rowIndex = i;
            if (flipImage)
            {
                rowIndex = numRows - i - 1;
            }

            return rowIndex;
        }

        public const int BytesPerComponent = sizeof(byte);
        public const int BitsPerComponent = 8 * BytesPerComponent;

        public readonly DataSource source;
        public readonly MediaType.Image contentType;
        public readonly string extension;
        public readonly byte[] data;
        public readonly Size dimensions;
        public readonly int stride;
        public readonly int components;
        public readonly int bytesPerSample;
        public readonly int bitsPerSample;

        public ImageData(DataSource source, Size size, int components, MediaType.Image contentType, byte[] data)
        {
            this.source = source;
            this.data = data;
            this.components = components;
            dimensions = size;
            this.contentType = contentType;
            stride = size.width * components;
            bytesPerSample = BytesPerComponent * components;
            bitsPerSample = 8 * bytesPerSample;
        }

        public ImageData(DataSource source, int width, int height, int components, MediaType.Image contentType, byte[] data)
            : this(source, new Size(width, height), components, contentType, data)
        {
        }

        public ImageData(DataSource source, Size size, int components)
            : this(source, size, components, MediaType.Image.Raw, new byte[size.height * size.width * components])
        {
        }

        public ImageData(DataSource source, int width, int height, int components)
            : this(source, new Size(width, height), components)
        {
        }

        public object Clone()
        {
            return new ImageData(source, dimensions, components, contentType, (byte[])data.Clone());
        }

        private void RGB2HSV(int index, out float h, out float s, out float v)
        {
            var R = data[index] / 255f;
            var G = data[index + 1] / 255f;
            var B = data[index + 2] / 255f;
            var max = R;
            var min = R;
            if (G > max)
            {
                max = B;
            }

            if (G < min)
            {
                min = B;
            }

            if (B > max)
            {
                max = B;
            }

            if (B < min)
            {
                min = B;
            }

            var delta = max - min;

            h = 0;
            if (delta > 0)
            {
                if (max == R)
                {
                    h = (G - B) / delta;
                }

                if (max == G)
                {
                    h = 2 + (B - R) / delta;
                }

                if (max == B)
                {
                    h = 4 + (R - G) / delta;
                }
            }

            h *= 60;
            if (h < 0)
            {
                h += 360;
            }

            if (h >= 360)
            {
                h -= 360;
            }

            s = 0;
            if (max > 0)
            {
                s = (max - min) / max;
            }

            v = max;
        }

        private void HSV2RGB(float h, float s, float v, int index)
        {
            var delta = v * s;
            h /= 60;
            var x = delta * (1 - Math.Abs((h % 2) - 1));
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

            var m = v - delta;
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

            for (var y = 0; y < resized.dimensions.height; ++y)
            {
                for (var x = 0; x < resized.dimensions.width; ++x)
                {
                    HorizontalLerp(resized, x, y);
                }
            }

            return resized;
        }

        private void HorizontalLerp(ImageData output, int outputX, int outputY)
        {
            var inputX = (float)outputX * dimensions.width / output.dimensions.width;
            var inputY = outputY;

            var inputXA = (int)inputX;
            var inputIA = inputY * stride + inputXA * components;
            RGB2HSV(inputIA, out var h1, out var s1, out var v1);

            var inputXB = (int)(inputX + 1) % dimensions.width;
            var inputIB = inputY * stride + inputXB * components;
            RGB2HSV(inputIB, out var h2, out var s2, out var v2);

            var p = 1 - inputX + inputXA;
            var q = 1 - inputXB + inputX;
            var h = h1 * p + h2 * q;
            var s = s1 * p + s2 * q;
            var v = v1 * p + v2 * q;

            var outputIndex = outputY * output.stride + outputX * output.components;
            output.HSV2RGB(h, s, v, outputIndex);
        }

        private ImageData VerticalSqueeze()
        {
            var resized = new ImageData(
                source,
                dimensions.width,
                dimensions.width,
                components);

            for (var y = 0; y < resized.dimensions.height; ++y)
            {
                for (var x = 0; x < resized.dimensions.width; ++x)
                {
                    VerticalLerp(resized, x, y);
                }
            }

            return resized;
        }

        private void VerticalLerp(ImageData output, int outputX, int outputY)
        {
            var inputX = outputX;
            var inputY = (float)outputY * dimensions.height / output.dimensions.height;

            var inputYA = (int)inputY;
            var inputIA = inputYA * stride + inputX * components;
            RGB2HSV(inputIA, out var h1, out var s1, out var v1);

            var inputYB = (int)(inputY + 1) % dimensions.height;
            var inputIB = inputYB * stride + inputX * components;
            RGB2HSV(inputIB, out var h2, out var s2, out var v2);

            var p = 1 - inputY + inputYA;
            var q = 1 - inputYB + inputY;
            var h = h1 * p + h2 * q;
            var s = s1 * p + s2 * q;
            var v = v1 * p + v2 * q;

            var outputIndex = outputY * output.stride + outputX * output.components;
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