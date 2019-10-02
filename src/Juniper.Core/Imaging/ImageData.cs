using System;

using Juniper.HTTP;
using Juniper.Progress;

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

        public const int BytesPerComponent = sizeof(byte);
        public const int BitsPerComponent = 8 * BytesPerComponent;

        public readonly ImageInfo info;
        public readonly MediaType.Image contentType;
        public readonly byte[] data;

        public ImageData(ImageInfo info, MediaType.Image contentType, byte[] data)
        {
            this.info = info;
            this.contentType = contentType;
            this.data = data;
        }

        public static ImageData Concatenate(ImageData[,] tiles, IProgress prog)
        {
            var rows = tiles.GetLength(0);
            var columns = tiles.GetLength(1);
            var len = rows * columns;
            var progs = prog.Split(len);
            var firstImage = tiles[0, 0];
            var tileWidth = firstImage.info.dimensions.width;
            var tileHeight = firstImage.info.dimensions.height;
            var bufferSize = new Size(
                    tileWidth * columns,
                    tileHeight * rows);
            var info = new ImageInfo(bufferSize, firstImage.info.components);
            var bufferData = new byte[len];

            for (var tileY = 0; tileY < rows; ++tileY)
            {
                for (var tileX = 0; tileX < columns; ++tileX)
                {
                    var tileI = tileY * columns + tileX;
                    var p = progs[tileI];
                    p.Report(0);
                    var tile = tiles[tileY, tileX];
                    for(var y = 0; y < tileHeight; ++y)
                    {
                        var tileBufferI = y * tileWidth;
                        var bufferI = bufferSize.width * (tileHeight * tileY + y) + tileX * tileWidth;
                        Array.Copy(tile.data, tileBufferI, bufferData, bufferI, tileWidth);
                    }
                    p.Report(1);
                }
            }

            return new ImageData(info, firstImage.contentType, bufferData);
        }

        public ImageData(Size size, int components, MediaType.Image contentType, byte[] data)
            : this(new ImageInfo(size, components), contentType, data) { }

        public ImageData(int width, int height, int components, MediaType.Image contentType, byte[] data)
            : this(new Size(width, height), components, contentType, data)
        {
        }

        public ImageData(Size size, int components)
            : this(size, components, MediaType.Image.Raw, new byte[size.height * size.width * components])
        {
        }

        public ImageData(int width, int height, int components)
            : this(new Size(width, height), components)
        {
        }

        public object Clone()
        {
            return new ImageData(info.dimensions, info.components, contentType, (byte[])data.Clone());
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
                info.dimensions.height,
                info.dimensions.height,
                info.components);

            for (var y = 0; y < resized.info.dimensions.height; ++y)
            {
                for (var x = 0; x < resized.info.dimensions.width; ++x)
                {
                    HorizontalLerp(resized, x, y);
                }
            }

            return resized;
        }

        private void HorizontalLerp(ImageData output, int outputX, int outputY)
        {
            var inputX = (float)outputX * info.dimensions.width / output.info.dimensions.width;
            var inputY = outputY;

            var inputXA = (int)inputX;
            var inputIA = inputY * info.stride + inputXA * info.components;
            RGB2HSV(inputIA, out var h1, out var s1, out var v1);

            var inputXB = (int)(inputX + 1) % info.dimensions.width;
            var inputIB = inputY * info.stride + inputXB * info.components;
            RGB2HSV(inputIB, out var h2, out var s2, out var v2);

            var p = 1 - inputX + inputXA;
            var q = 1 - inputXB + inputX;
            var h = h1 * p + h2 * q;
            var s = s1 * p + s2 * q;
            var v = v1 * p + v2 * q;

            var outputIndex = outputY * output.info.stride + outputX * output.info.components;
            output.HSV2RGB(h, s, v, outputIndex);
        }

        private ImageData VerticalSqueeze()
        {
            var resized = new ImageData(
                info.dimensions.width,
                info.dimensions.width,
                info.components);

            for (var y = 0; y < resized.info.dimensions.height; ++y)
            {
                for (var x = 0; x < resized.info.dimensions.width; ++x)
                {
                    VerticalLerp(resized, x, y);
                }
            }

            return resized;
        }

        private void VerticalLerp(ImageData output, int outputX, int outputY)
        {
            var inputX = outputX;
            var inputY = (float)outputY * info.dimensions.height / output.info.dimensions.height;

            var inputYA = (int)inputY;
            var inputIA = inputYA * info.stride + inputX * info.components;
            RGB2HSV(inputIA, out var h1, out var s1, out var v1);

            var inputYB = (int)(inputY + 1) % info.dimensions.height;
            var inputIB = inputYB * info.stride + inputX * info.components;
            RGB2HSV(inputIB, out var h2, out var s2, out var v2);

            var p = 1 - inputY + inputYA;
            var q = 1 - inputYB + inputY;
            var h = h1 * p + h2 * q;
            var s = s1 * p + s2 * q;
            var v = v1 * p + v2 * q;

            var outputIndex = outputY * output.info.stride + outputX * output.info.components;
            output.HSV2RGB(h, s, v, outputIndex);
        }

        public ImageData Squarify()
        {
            if (info.dimensions.width < info.dimensions.height)
            {
                return VerticalSqueeze();
            }
            else if (info.dimensions.width > info.dimensions.height)
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