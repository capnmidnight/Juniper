using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Juniper.Image
{
    public static class IEncoderExt
    {
        public static Task EncodeAsync(this IEncoder encoder, RawImage image, Stream outputStream, bool flipImage)
        {
            return Task.Run(() => encoder.Encode(image, outputStream, flipImage));
        }

        public static void Encode(this IEncoder encoder, RawImage image, FileInfo file, bool flipImage)
        {
            using (var stream = file.OpenWrite())
            {
                encoder.Encode(image, stream, flipImage);
            }
        }

        public static Task EncodeAsync(this IEncoder encoder, RawImage image, FileInfo file, bool flipImage)
        {
            return Task.Run(() => encoder.Encode(image, file, flipImage));
        }

        public static void Encode(this IEncoder encoder, RawImage image, HttpWebRequest request, bool flipImage)
        {
            using (var stream = request.GetRequestStream())
            {
                encoder.Encode(image, stream, flipImage);
            }
        }

        public static Task EncodeAsync(this IEncoder encoder, RawImage image, HttpWebRequest request, bool flipImage)
        {
            return Task.Run(() => encoder.Encode(image, request, flipImage));
        }

        public static void Encode(this IEncoder encoder, RawImage image, string fileName, bool flipImage)
        {
            encoder.Encode(image, new FileInfo(fileName), flipImage);
        }

        public static Task EncodeAsync(this IEncoder encoder, RawImage image, string fileName, bool flipImage)
        {
            return encoder.EncodeAsync(image, new FileInfo(fileName), flipImage);
        }

        public static byte[] Encode(this IEncoder encoder, RawImage image, bool flipImage)
        {
            using (var mem = new MemoryStream())
            {
                encoder.Encode(image, mem, flipImage);
                return mem.ToArray();
            }
        }

        public static Task<byte[]> EncodeAsync(this IEncoder encoder, RawImage image, bool flipImage)
        {
            return Task.Run(() => encoder.Encode(image, flipImage));
        }
    }
}