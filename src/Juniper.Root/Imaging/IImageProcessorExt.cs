using System;
using System.Globalization;
using System.Threading.Tasks;

using Juniper.IO;

namespace Juniper.Imaging
{

    public static class IImageProcessorExt
    {
        public static void ValidateImages<T>(this IImageProcessor<T> codec, T[,] images, IProgress prog, out int rows, out int columns, out int components, out int tileWidth, out int tileHeight)
        {
            if (codec is null)
            {
                throw new ArgumentNullException(nameof(codec));
            }

            if (images is null)
            {
                throw new ArgumentNullException(nameof(images));
            }

            if (images.Length == 0)
            {
            }

            var r = images.GetLength(0);
            var c = images.GetLength(1);

            if (r * c == 0)
            {
                throw new ArgumentException("Must have at least one image.", nameof(images));
            }

            rows = r;
            columns = c;

            prog.Report(0);

            components = 0;
            tileWidth = 0;
            tileHeight = 0;

            var anyNotNull = false;
            for (var y = 0; y < rows; ++y)
            {
                for (var x = 0; x < columns; ++x)
                {
                    var img = images[y, x];
                    if (img is object)
                    {
                        if (!anyNotNull)
                        {
                            tileWidth = codec.GetWidth(img);
                            tileHeight = codec.GetHeight(img);
                            components = codec.GetComponents(img);
                        }

                        anyNotNull = true;
                        if (img is object
                            && (codec.GetWidth(img) != tileWidth
                                || codec.GetHeight(img) != tileHeight))
                        {
                            var yStr = y.ToString(CultureInfo.InvariantCulture);
                            var xStr = x.ToString(CultureInfo.InvariantCulture);
                            throw new ArgumentException($"All elements of {nameof(images)} must be the same width and height. Image [{yStr},{xStr}] did not match image 0.");
                        }
                    }
                }
            }

            if (!anyNotNull)
            {
                throw new ArgumentNullException($"Expected at least one image in {nameof(images)} to be not null");
            }
        }

        public static Task<T> ConcatenateAsync<T>(this IImageProcessor<T> codec, T[,] images, IProgress prog = null)
        {
            if (codec is null)
            {
                throw new ArgumentNullException(nameof(codec));
            }

            if (images is null)
            {
                throw new ArgumentNullException(nameof(images));
            }

            return Task.Run(() => codec.Concatenate(images, prog));
        }
    }
}