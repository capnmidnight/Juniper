using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Juniper.Progress;

namespace Juniper.IO
{
    public interface ISerializer
    {
        void Serialize<T>(Stream stream, T value, IProgress prog);
    }

    public interface ISerializer<in T>
    {
        void Serialize(Stream stream, T value, IProgress prog);
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

        public static void Serialize<T>(this ISerializer<T> serializer, HttpWebRequest request, T value, IProgress prog)
        {
            using (var stream = request.GetRequestStream())
            {
                serializer.Serialize(stream, value, prog);
            }
        }

        public static Task SerializeAsync<T>(this ISerializer<T> serializer, HttpWebRequest request, T value, IProgress prog)
        {
            return Task.Run(() => serializer.Serialize(request, value, prog));
        }

        public static void Serialize<T>(this ISerializer<T> serializer, HttpWebRequest request, T value)
        {
            serializer.Serialize(request, value, null);
        }

        public static Task SerializeAsync<T>(this ISerializer<T> serializer, HttpWebRequest request, T value)
        {
            return serializer.SerializeAsync(request, value, null);
        }

        public static void Serialize<T>(this ISerializer serializer, HttpListenerResponse response, T value, IProgress prog)
        {
            serializer.Serialize(response.OutputStream, value, prog);
        }

        public static Task SerializeAsync<T>(this ISerializer serializer, HttpListenerResponse response, T value, IProgress prog)
        {
            return Task.Run(() => serializer.Serialize(response, value, prog));
        }

        public static void Serialize<T>(this ISerializer serializer, HttpListenerResponse response, T value)
        {
            serializer.Serialize(response.OutputStream, value);
        }

        public static Task SerializeAsync<T>(this ISerializer serializer, HttpListenerResponse response, T value)
        {
            return serializer.SerializeAsync(response, value, null);
        }

        public static byte[] Serialize<T>(this ISerializer serializer, T value)
        {
            return serializer.Serialize(value, null);
        }

        public static byte[] Serialize<T>(this ISerializer serializer, T value, IProgress prog)
        {
            using (var mem = new MemoryStream())
            {
                serializer.Serialize(mem, value, prog);
                mem.Flush();

                return mem.ToArray();
            }
        }
        public static Task<byte[]> SerializeAsync<T>(this ISerializer serializer, T value, IProgress prog)
        {
            return Task.Run(() => serializer.Serialize(value, prog));
        }

        public static Task<byte[]> SerializeAsync<T>(this ISerializer serializer, T value)
        {
            return serializer.SerializeAsync(value, null);
        }

        public static void Serialize<T>(this ISerializer serializer, FileInfo file, T value, IProgress prog)
        {
            using (var stream = file.Create())
            {
                serializer.Serialize(stream, value, prog);
            }
        }

        public static Task SerializeAsync<T>(this ISerializer serializer, FileInfo file, T value, IProgress prog)
        {
            return Task.Run(() => serializer.Serialize(file, value, prog));
        }

        public static void Serialize<T>(this ISerializer serializer, FileInfo file, T value)
        {
            serializer.Serialize(file, value, null);
        }

        public static Task SerializeAsync<T>(this ISerializer serializer, FileInfo file, T value)
        {
            return serializer.SerializeAsync(file, value, null);
        }

        public static void Serialize<T>(this ISerializer serializer, string fileName, T value, IProgress prog)
        {
            serializer.Serialize(new FileInfo(fileName), value, prog);
        }

        public static Task SerializeAsync<T>(this ISerializer serializer, string fileName, T value, IProgress prog)
        {
            return Task.Run(() => serializer.Serialize(fileName, value, prog));
        }

        public static void Serialize<T>(this ISerializer serializer, string fileName, T value)
        {
            serializer.Serialize(fileName, value, null);
        }

        public static Task SerializeAsync<T>(this ISerializer serializer, string fileName, T value)
        {
            return serializer.SerializeAsync(fileName, value, null);
        }

        public static string ToString<T>(this ISerializer serializer, T value, IProgress prog)
        {
            return Encoding.UTF8.GetString(serializer.Serialize(value, prog));
        }

        public static Task<string> ToStringAsync<T>(this ISerializer serializer, T value, IProgress prog)
        {
            return Task.Run(() => Encoding.UTF8.GetString(serializer.Serialize(value, prog)));
        }

        public static string ToString<T>(this ISerializer serializer, T value)
        {
            return serializer.ToString(value, null);
        }

        public static Task<string> ToStringAsync<T>(this ISerializer serializer, T value)
        {
            return serializer.ToStringAsync(value, null);
        }

        public static Task SerializeAsync<T>(this ISerializer<T> serializer, Stream stream, T value, IProgress prog)
        {
            return Task.Run(() => serializer.Serialize(stream, value, prog));
        }

        public static void Serialize<T>(this ISerializer<T> serializer, Stream stream, T value)
        {
            serializer.Serialize(stream, value, null);
        }

        public static Task SerializeAsync<T>(this ISerializer<T> serializer, Stream stream, T value)
        {
            return serializer.SerializeAsync(stream, value, null);
        }

        public static void Serialize<T>(this ISerializer<T> serializer, HttpListenerResponse response, T value, IProgress prog)
        {
            serializer.Serialize(response.OutputStream, value, prog);
        }

        public static Task SerializeAsync<T>(this ISerializer<T> serializer, HttpListenerResponse response, T value, IProgress prog)
        {
            return Task.Run(() => serializer.Serialize(response, value, prog));
        }

        public static void Serialize<T>(this ISerializer<T> serializer, HttpListenerResponse response, T value)
        {
            serializer.Serialize(response.OutputStream, value);
        }

        public static Task SerializeAsync<T>(this ISerializer<T> serializer, HttpListenerResponse response, T value)
        {
            return serializer.SerializeAsync(response, value, null);
        }

        public static byte[] Serialize<T>(this ISerializer<T> serializer, T value, IProgress prog)
        {
            using (var mem = new MemoryStream())
            {
                serializer.Serialize(mem, value, prog);
                mem.Flush();

                return mem.ToArray();
            }
        }

        public static Task<byte[]> SerializeAsync<T>(this ISerializer<T> serializer, T value, IProgress prog)
        {
            return Task.Run(() => serializer.Serialize(value, prog));
        }

        public static byte[] Serialize<T>(this ISerializer<T> serializer, T value)
        {
            return serializer.Serialize(value, null);
        }

        public static Task<byte[]> SerializeAsync<T>(this ISerializer<T> serializer, T value)
        {
            return serializer.SerializeAsync(value, null);
        }

        public static void Serialize<T>(this ISerializer<T> serializer, FileInfo file, T value, IProgress prog)
        {
            using (var stream = file.Create())
            {
                serializer.Serialize(stream, value, prog);
            }
        }

        public static Task SerializeAsync<T>(this ISerializer<T> serializer, FileInfo file, T value, IProgress prog)
        {
            return Task.Run(() => serializer.Serialize(file, value, prog));
        }

        public static void Serialize<T>(this ISerializer<T> serializer, FileInfo file, T value)
        {
            serializer.Serialize(file, value, null);
        }

        public static Task SerializeAsync<T>(this ISerializer<T> serializer, FileInfo file, T value)
        {
            return serializer.SerializeAsync(file, value, null);
        }

        public static void Serialize<T>(this ISerializer<T> serializer, string fileName, T value, IProgress prog)
        {
            serializer.Serialize(new FileInfo(fileName), value, prog);
        }

        public static Task SerializeAsync<T>(this ISerializer<T> serializer, string fileName, T value, IProgress prog)
        {
            return Task.Run(() => serializer.Serialize(fileName, value, prog));
        }

        public static void Serialize<T>(this ISerializer<T> serializer, string fileName, T value)
        {
            serializer.Serialize(fileName, value, null);
        }

        public static Task SerializeAsync<T>(this ISerializer<T> serializer, string fileName, T value)
        {
            return serializer.SerializeAsync(fileName, value, null);
        }

        public static string ToString<T>(this ISerializer<T> serializer, T value, IProgress prog)
        {
            return Encoding.UTF8.GetString(serializer.Serialize(value, prog));
        }

        public static Task<string> ToStringAsync<T>(this ISerializer<T> serializer, T value, IProgress prog)
        {
            return Task.Run(() => Encoding.UTF8.GetString(serializer.Serialize(value, prog)));
        }

        public static string ToString<T>(this ISerializer<T> serializer, T value)
        {
            return serializer.ToString(value, null);
        }

        public static Task<string> ToStringAsync<T>(this ISerializer<T> serializer, T value)
        {
            return serializer.ToStringAsync(value, null);
        }
    }
}