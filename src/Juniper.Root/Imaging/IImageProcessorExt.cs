using System;
using System.Globalization;
using System.Threading.Tasks;

using Juniper.Progress;

namespace Juniper.Imaging
{

    public static class IImageProcessorExt
    {
        public static void ValidateImages<T>(this IImageProcessor<T> codec, T[,] images, IProgress prog, out int rows, out int columns, out int components, out int tileWidth, out int tileHeight)
        {
            prog.Report(0);

            if (images == null)
            {
                throw new ArgumentNullException($"Parameter {nameof(images)} must not be null.");
            }

            if (images.Length == 0)
            {
                throw new ArgumentException($"Parameter {nameof(images)} must have at least one image.");
            }

            rows = images.GetLength(0);
            columns = images.GetLength(1);
            components = 0;
            tileWidth = 0;
            tileHeight = 0;

            var anyNotNull = false;
            for (var y = 0; y < rows; ++y)
            {
                for (var x = 0; x < columns; ++x)
                {
                    var img = images[y, x];
                    if (img != null)
                    {
                        if (!anyNotNull)
                        {
                            tileWidth = codec.GetWidth(img);
                            tileHeight = codec.GetHeight(img);
                            components = codec.GetComponents(img);
                        }

                        anyNotNull = true;
                        if (img != null
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

        public static T Concatenate<T>(this IImageProcessor<T> codec, T[,] images)
        {
            return codec.Concatenate(images, null);
        }

        public static Task<T> ConcatenateAsync<T>(this IImageProcessor<T> codec, T[,] images, IProgress prog)
        {
            return Task.Run(() => codec.Concatenate(images, prog));
        }

        public static Task<T> ConcatenateAsync<T>(this IImageProcessor<T> codec, T[,] images)
        {
            return codec.ConcatenateAsync(images, null);
        }
    }
}