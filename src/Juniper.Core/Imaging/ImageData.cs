using System;

using static System.Math;

namespace Juniper.Imaging
{
    /// <summary>
    /// The raw bytes and dimensions of an image that has been loaded either off disk or across the 'net.
    /// </summary>
    public partial class ImageData
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
        public readonly byte[] data;

        public ImageData(ImageInfo info, byte[] data)
        {
            this.info = info;
            this.data = data;
        }

        public ImageData(Size size, int components, byte[] data)
            : this(new ImageInfo(size, components), data) { }

        public ImageData(int width, int height, int components, byte[] data)
            : this(new Size(width, height), components, data)
        {
        }

        public ImageData(Size size, int components)
            : this(size, components, new byte[size.height * size.width * components])
        {
        }

        public ImageData(int width, int height, int components)
            : this(new Size(width, height), components)
        {
        }
    }
}