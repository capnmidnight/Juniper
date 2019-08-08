using System.IO;
using System.Threading.Tasks;

using Juniper.Progress;
using Juniper.Serialization;

namespace Juniper.Imaging
{
    public interface IImageDecoder<T> : IFactory<T>
    {
        T Read(byte[] data, DataSource source = DataSource.None);

        T Concatenate(int columns, int rows, IProgress prog, params T[] images);
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

        public static T Read<T>(this IImageDecoder<T> decoder, string fileName)
        {
            return decoder.Read(File.ReadAllBytes(fileName), DataSource.File);
        }

        public static T Read<T>(this IImageDecoder<T> decoder, FileInfo file)
        {
            return decoder.Read(file.FullName);
        }

        public static Task<T> Combine6Squares<T>(this IImageDecoder<T> concator, T north, T east, T west, T south, T up, T down, IProgress prog = null)
        {
            return Task.Run(() => concator.Concatenate(
                1, 6, prog,
                west, south, east,
                down, up, north));
        }

        public static Task<T> CombineCross<T>(this IImageDecoder<T> concator, T north, T east, T west, T south, T down, T up, IProgress prog = null)
        {
            return Task.Run(() => concator.Concatenate(
                4, 3, prog,
                default, up, default, default,
                west, north, east, south,
                default, down, default, default));
        }
    }
}