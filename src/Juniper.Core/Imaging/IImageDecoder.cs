using System;
using System.IO;

using Juniper.Progress;
using Juniper.Serialization;
using Juniper.Streams;

namespace Juniper.Imaging
{
    public interface IImageDecoder<T> : IFactory<T>
    {
        int GetWidth(T img);

        int GetHeight(T img);

        T Read(byte[] data, DataSource source = DataSource.None);

        T Concatenate(T[,] images, IProgress prog = null);

        ImageFormat Format { get; }
    }

    public static class IImageDecoderExt
    {
        public static T Read<T>(this IImageDecoder<T> decoder, Stream stream)
        {
            var source = stream.DetermineSource();
            using (var mem = new MemoryStream())
            {
                stream.CopyTo(mem);
                mem.Flush();
                return decoder.Read(mem.ToArray(), source);
            }
        }

        public static T Read<T>(this IImageDecoder<T> decoder, Stream stream, long length, IProgress prog)
        {
            var progStream = new ProgressStream(stream, length, prog);
            return decoder.Read(progStream);
        }

        public static T Read<T>(this IImageDecoder<T> decoder, string fileName, IProgress prog = null)
        {
            return decoder.Read(new FileInfo(fileName), prog);
        }

        public static T Read<T>(this IImageDecoder<T> decoder, FileInfo file, IProgress prog = null)
        {
            return decoder.Read(file.OpenRead(), file.Length, prog);
        }

        public static void ValidateImages<T>(this IImageDecoder<T> decoder, T[,] images, IProgress prog, out int rows, out int columns, out T firstImage, out int tileWidth, out int tileHeight)
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

            rows = images.GetLength(0);
            columns = images.GetLength(1);
            var anyNotNull = false;
            firstImage = default;
            tileWidth = 0;
            tileHeight = 0;
            for (var y = 0; y < rows; ++y)
            {
                for (var x = 0; x < columns; ++x)
                {
                    var img = images[y, x];
                    if (img != null)
                    {
                        if (!anyNotNull)
                        {
                            firstImage = img;
                            tileWidth = decoder.GetWidth(img);
                            tileHeight = decoder.GetHeight(img);
                        }

                        anyNotNull = true;
                        if (img != null
                            && (decoder.GetWidth(img) != tileWidth
                                || decoder.GetHeight(img) != tileHeight))
                        {
                            throw new ArgumentException($"All elements of {nameof(images)} must be the same width and height. Image [{y},{x}] did not match image 0.");
                        }
                    }
                }
            }

            if (!anyNotNull)
            {
                throw new ArgumentNullException($"Expected at least one image in {nameof(images)} to be not null");
            }
        }
    }
}