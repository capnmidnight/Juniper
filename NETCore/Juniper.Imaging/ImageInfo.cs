using System;

using static System.Math;

namespace Juniper.Imaging
{
    public sealed class ImageInfo
    {
        public static bool IsPNG(byte[] data)
        {
            return data.Length >= 8
                && data[0] == 0x89
                && data[0] == 0x50
                && data[0] == 0x4E
                && data[0] == 0x47
                && data[0] == 0x0D
                && data[0] == 0x0A
                && data[0] == 0x1A
                && data[0] == 0x0A;
        }

        public static ImageInfo ReadPNG(byte[] data)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            var width = 0;
            var height = 0;

            var i = Units.Bits.PER_BYTE; // skip the PNG signature

            while (i < data.Length)
            {
                var len = 0;
                len = (len << Units.Bits.PER_BYTE) | data[i++];
                len = (len << Units.Bits.PER_BYTE) | data[i++];
                len = (len << Units.Bits.PER_BYTE) | data[i++];
                len = (len << Units.Bits.PER_BYTE) | data[i++];

                var chunk = System.Text.Encoding.UTF8.GetString(data, i, 4);
                i += 4;

                if (chunk == "IHDR")
                {
                    width = (width << Units.Bits.PER_BYTE) | data[i++];
                    width = (width << Units.Bits.PER_BYTE) | data[i++];
                    width = (width << Units.Bits.PER_BYTE) | data[i++];
                    width = (width << Units.Bits.PER_BYTE) | data[i++];

                    height = (height << Units.Bits.PER_BYTE) | data[i++];
                    height = (height << Units.Bits.PER_BYTE) | data[i++];
                    height = (height << Units.Bits.PER_BYTE) | data[i++];
                    height = (height << Units.Bits.PER_BYTE) | data[i++];

                    var bitDepth = data[i + 9];
                    var colorType = data[i + 10];

                    var components = 0;
                    switch (colorType)
                    {
                        case 0:
                        components = (int)Ceiling(Units.Bits.Bytes(bitDepth));
                        break;

                        case 2:
                        components = 3;
                        break;

                        case 3:
                        components = 1;
                        break;

                        case 4:
                        components = (int)Ceiling(Units.Bits.Bytes(bitDepth)) + 1;
                        break;

                        case 6:
                        components = 4;
                        break;
                    }

                    return new ImageInfo(height, width, components);
                }

                i += len;
                i += 4;
            }

            throw new ArgumentException("Could not parse PNG data", nameof(data));
        }

        public static ImageInfo ReadJPEG(byte[] data)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            for (var i = 0; i < data.Length - 1; ++i)
            {
                var a = data[i];
                var b = data[i + 1];
                if (a == byte.MaxValue
                    && b == 0xc0)
                {
                    var heightHi = data[i + 5];
                    var heightLo = data[i + 6];
                    var widthHi = data[i + 7];
                    var widthLo = data[i + 8];

                    var width = (widthHi << Units.Bits.PER_BYTE) | widthLo;
                    var height = (heightHi << Units.Bits.PER_BYTE) | heightLo;

                    return new ImageInfo(width, height, 3);
                }
            }

            throw new ArgumentException("Could not parse JPEG data", nameof(data));
        }

        public Size Dimensions { get; }

        public int Stride { get; }

        public int Components { get; }

        public int BytesPerSample { get; }

        public int BitsPerSample { get; }

        public ImageInfo(Size size, int components)
        {
            Components = components;
            Dimensions = size ?? throw new ArgumentNullException(nameof(size));
            Stride = size.Width * components;
            BytesPerSample = components;
            BitsPerSample = Units.Bits.PER_BYTE * BytesPerSample;
        }

        public ImageInfo(int width, int height, int components)
            : this(new Size(width, height), components) { }
    }
}
