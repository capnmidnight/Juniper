using Juniper.Progress;

using System;
using System.Globalization;
using System.Threading.Tasks;

namespace Juniper.Imaging
{

    public static class IImageProcessorExt
    {
        public static void ValidateImages<T>(this IImageProcessor<T> factory, T[,] images, IProgress prog, out int rows, out int columns, out int components, out int tileWidth, out int tileHeight)
        {
            if (factory is null)
            {
                throw new ArgumentNullException(nameof(factory));
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

            IProgressExt.Report(prog, 0);

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
                            tileWidth = factory.GetWidth(img);
                            tileHeight = factory.GetHeight(img);
                            components = factory.GetComponents(img);
                        }

                        anyNotNull = true;
                        if (img is object
                            && (factory.GetWidth(img) != tileWidth
                                || factory.GetHeight(img) != tileHeight))
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
                throw new ArgumentNullException(nameof(images), $"Expected at least one image in {nameof(images)} to be not null");
            }
        }

        public static Task<T> ConcatenateAsync<T>(this IImageProcessor<T> factory, T[,] images, IProgress prog = null)
        {
            if (factory is null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            if (images is null)
            {
                throw new ArgumentNullException(nameof(images));
            }

            return Task.Run(() => factory.Concatenate(images, prog));
        }
    }
}