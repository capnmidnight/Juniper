using System;

namespace Juniper.Imaging
{
    public sealed class ImageInfo
    {
        public static ImageInfo ReadPNG(byte[] data)
        {
            int width = 0, height = 0;

            var i = 8; // skip the PNG signature

            while (i < data.Length)
            {
                var len = 0;
                len = len << 8 | data[i++];
                len = len << 8 | data[i++];
                len = len << 8 | data[i++];
                len = len << 8 | data[i++];

                var chunk = System.Text.Encoding.UTF8.GetString(data, i, 4);
                i += 4;

                if (chunk == "IHDR")
                {
                    width = width << 8 | data[i++];
                    width = width << 8 | data[i++];
                    width = width << 8 | data[i++];
                    width = width << 8 | data[i++];

                    height = height << 8 | data[i++];
                    height = height << 8 | data[i++];
                    height = height << 8 | data[i++];
                    height = height << 8 | data[i++];

                    var bitDepth = data[i + 9];
                    var colorType = data[i + 10];

                    var components = 0;
                    switch (colorType)
                    {
                        case 0: components = (int)Math.Ceiling((float)bitDepth / 8); break;
                        case 2: components = 3; break;
                        case 3: components = 1; break;
                        case 4: components = (int)Math.Ceiling((float)bitDepth / 8) + 1; break;
                        case 6: components = 4; break;
                    }

                    return new ImageInfo(height, width, components);
                }

                i += len;
                i += 4;
            }

            return default;
        }

        public static ImageInfo ReadJPEG(byte[] data)
        {
            for (var i = 0; i < data.Length - 1; ++i)
            {
                var a = data[i];
                var b = data[i + 1];
                if (a == 0xff && b == 0xc0)
                {
                    var heightHi = data[i + 5];
                    var heightLo = data[i + 6];
                    var widthHi = data[i + 7];
                    var widthLo = data[i + 8];

                    var width = widthHi << 8 | widthLo;
                    var height = heightHi << 8 | heightLo;

                    return new ImageInfo(width, height, 3);
                }
            }

            return default;
        }

        public readonly Size dimensions;
        public readonly int stride;
        public readonly int components;
        public readonly int bytesPerSample;
        public readonly int bitsPerSample;

        public ImageInfo(Size size, int components)
        {
            this.components = components;
            dimensions = size;
            stride = size.width * components;
            bytesPerSample = ImageData.BytesPerComponent * components;
            bitsPerSample = 8 * bytesPerSample;
        }

        public ImageInfo(int width, int height, int components)
            : this(new Size(width, height), components) { }
    }
}
