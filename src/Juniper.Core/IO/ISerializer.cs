using System.IO;
using System.Net;
using System.Threading.Tasks;

using Juniper.Progress;

namespace Juniper.IO
{
    public interface ISerializer
    {
        void Serialize<T>(Stream stream, T value, IProgress prog);
    }

    public static class ISerializerExt
    {
        public static Task SerializeAsync<T>(this ISerializer serializer, Stream stream, T value, IProgress prog)
        {
            return Task.Run(() => serializer.Serialize(stream, value, prog));
        }

        public static void Serialize<T>(this ISerializer serializer, Stream stream, T value)
        {
            serializer.Serialize(stream, value, null);
        }

        public static Task SerializeAsync<T>(this ISerializer serializer, Stream stream, T value)
        {
            return serializer.SerializeAsync(stream, value, null);
        }

        public static void Serialize<T>(this ISerializer serializer, HttpWebRequest request, T value, IProgress prog)
        {
            using (var stream = request.GetRequestStream())
            {
                serializer.Serialize(stream, value, prog);
            }
        }

        public static async Task SerializeAsync<T>(this ISerializer serializer, HttpWebRequest request, T value, IProgress prog)
        {
            using (var stream = await request.GetRequestStreamAsync())
            {
                await serializer.SerializeAsync(stream, value, prog);
            }
        }

        public static void Serialize<T>(this ISerializer serializer, HttpWebRequest request, T value)
        {
            serializer.Serialize(request, value, null);
        }

        public static Task SerializeAsync<T>(this ISerializer serializer, HttpWebRequest request, T value)
        {
            return serializer.SerializeAsync(request, value, null);
        }
    }
}