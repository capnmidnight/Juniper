using System;
using System.IO;

using Juniper.HTTP;
using Juniper.Progress;
using Juniper.Serialization;
using Juniper.Streams;

namespace Juniper.Imaging
{
    public interface IImageCodec<T> : IFactory<T>
    {
        int GetWidth(T img);

        int GetHeight(T img);

        int GetComponents(T img);

        MediaType.Image ReadImageType { get; }

        MediaType.Image WriteImageType { get; }

        ImageInfo GetImageInfo(byte[] data);

        T Concatenate(T[,] images, IProgress prog);
    }

    public static class IImageDecoderExt
    {
        public static ImageInfo GetImageInfo<T>(this IImageCodec<T> codec, Stream stream)
        {
            using (var mem = new MemoryStream())
            {
                stream.CopyTo(mem);
                mem.Flush();
                var buffer = mem.ToArray();
                return codec.GetImageInfo(buffer);
            }
        }

        public static ImageInfo GetImageInfo<T>(this IImageCodec<T> codec, FileInfo file)
        {
            using (var stream = file.OpenRead())
            {
                return codec.GetImageInfo(stream);
            }
        }

        public static ImageInfo GetImageInfo<T>(this IImageCodec<T> codec, string fileName)
        {
            return codec.GetImageInfo(new FileInfo(fileName));
        }

        public static ImageData GetUndecodedImage<T>(this IImageCodec<T> codec, Stream stream)
        {
            using (var mem = new MemoryStream())
            {
                stream.CopyTo(mem);
                mem.Flush();
                var buffer = mem.ToArray();
                var info = codec.GetImageInfo(buffer);
                return new ImageData(info, codec.ReadImageType, buffer);
            }
        }

        public static ImageData GetUndecodedImage<T>(this IImageCodec<T> codec, Stream stream, long length, IProgress prog)
        {
            var progStream = new ProgressStream(stream, length, prog);
            return codec.GetUndecodedImage(progStream);
        }

        public static ImageData GetUndecodedImage<T>(this IImageCodec<T> codec, FileInfo file, IProgress prog)
        {
            return codec.GetUndecodedImage(file.OpenRead(), file.Length, prog);
        }

        public static ImageData GetUndecodedImage<T>(this IImageCodec<T> codec, FileInfo file)
        {
            return codec.GetUndecodedImage(file, null);
        }

        public static ImageData GetUndecodedImage<T>(this IImageCodec<T> codec, string fileName, IProgress prog)
        {
            return codec.GetUndecodedImage(new FileInfo(fileName), prog);
        }

        public static ImageData GetUndecodedImage<T>(this IImageCodec<T> codec, string fileName)
        {
            return codec.GetUndecodedImage(fileName, null);
        }

        public static void ValidateImages<T>(this IImageCodec<T> codec, T[,] images, IProgress prog, out int rows, out int columns, out int components, out int tileWidth, out int tileHeight)
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
                            throw new ArgumentException($"All elements of {nameof(images)} must be the same width and height. Image [{y.ToString()},{x.ToString()}] did not match image 0.");
                        }
                    }
                }
            }

            if (!anyNotNull)
            {
                throw new ArgumentNullException($"Expected at least one image in {nameof(images)} to be not null");
            }
        }

        public static T Concatenat<T>(this IImageCodec<T> codec, T[,] images)
        {
            return codec.Concatenate(images, null);
        }
    }
}