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

        int GetComponents(T img);

        ImageInfo GetImageInfo(byte[] data);

        T Concatenate(T[,] images, IProgress prog = null);

        HTTP.MediaType.Image Format { get; }
    }

    public static class IImageDecoderExt
    {
        public static ImageInfo GetImageInfo<T>(this IImageDecoder<T> decoder, Stream stream)
        {
            using (var mem = new MemoryStream())
            {
                stream.CopyTo(mem);
                mem.Flush();
                var buffer = mem.ToArray();
                return decoder.GetImageInfo(buffer);
            }
        }

        public static ImageInfo GetImageInfo<T>(this IImageDecoder<T> decoder, FileInfo file)
        {
            using (var stream = file.OpenRead())
            {
                return decoder.GetImageInfo(stream);
            }
        }

        public static ImageInfo GetImageInfo<T>(this IImageDecoder<T> decoder, string fileName)
        {
            return decoder.GetImageInfo(new FileInfo(fileName));
        }

        public static ImageData ReadRaw<T>(this IImageDecoder<T> decoder, Stream stream)
        {
            using (var mem = new MemoryStream())
            {
                stream.CopyTo(mem);
                mem.Flush();
                var buffer = mem.ToArray();
                var info = decoder.GetImageInfo(buffer);
                return new ImageData(info, buffer);
            }
        }

        public static ImageData ReadRaw<T>(this IImageDecoder<T> decoder, Stream stream, long length, IProgress prog)
        {
            var progStream = new ProgressStream(stream, length, prog);
            return decoder.ReadRaw(progStream);
        }

        public static ImageData ReadRaw<T>(this IImageDecoder<T> decoder, FileInfo file, IProgress prog = null)
        {
            return decoder.ReadRaw(file.OpenRead(), file.Length, prog);
        }

        public static ImageData Read<T>(this IImageDecoder<T> decoder, string fileName, IProgress prog = null)
        {
            return decoder.ReadRaw(new FileInfo(fileName), prog);
        }

        public static void ValidateImages<T>(this IImageDecoder<T> decoder, T[,] images, IProgress prog, out int rows, out int columns, out int components, out int tileWidth, out int tileHeight)
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
                            tileWidth = decoder.GetWidth(img);
                            tileHeight = decoder.GetHeight(img);
                            components = decoder.GetComponents(img);
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