using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Juniper.Image
{
    public static class IDecoderExt
    {
        public static Task<RawImage> DecodeAsync(this IDecoder decoder, Stream imageStream, bool flipImage)
        {
            return Task.Run(() => decoder.Decode(imageStream, flipImage));
        }

        public static RawImage Decode(this IDecoder decoder, FileInfo file, bool flipImage)
        {
            if (!file.Exists)
            {
                throw new FileNotFoundException("File not found!", file.FullName);
            }
            else
            {
                using (var stream = file.OpenRead())
                {
                    return decoder.Decode(stream, flipImage);
                }
            }
        }

        public static Task<RawImage> DecodeAsync(this IDecoder decoder, FileInfo file, bool flipImage)
        {
            return Task.Run(() => decoder.Decode(file, flipImage));
        }

        public static RawImage Decode(this IDecoder decoder, HttpWebResponse response, bool flipImage)
        {
            using (var stream = response.GetResponseStream())
            {
                return decoder.Decode(stream, flipImage);
            }
        }

        public static Task<RawImage> DecodeAsync(this IDecoder decoder, HttpWebResponse response, bool flipImage)
        {
            return Task.Run(() => decoder.Decode(response, flipImage));
        }

        public static RawImage Decode(this IDecoder decoder, string fileName, bool flipImage)
        {
            return decoder.Decode(new FileInfo(fileName), flipImage);
        }

        public static Task<RawImage> DecodeAsync(this IDecoder decoder, string fileName, bool flipImage)
        {
            return decoder.DecodeAsync(new FileInfo(fileName), flipImage);
        }

        public static RawImage Decode(this IDecoder decoder, byte[] bytes, bool flipImage)
        {
            using (var mem = new MemoryStream(bytes))
            {
                return decoder.Decode(mem, flipImage);
            }
        }

        public static Task<RawImage> DecodeAsync(this IDecoder decoder, byte[] bytes, bool flipImage)
        {
            return Task.Run(() => decoder.Decode(bytes, flipImage));
        }
    }
}