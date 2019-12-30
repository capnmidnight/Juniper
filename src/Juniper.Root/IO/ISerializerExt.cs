using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Juniper.HTTP;
using Juniper.Progress;

namespace Juniper.IO
{
    public static class ISerializerExt
    {
        public static void Serialize<T>(this ISerializer<T> serializer, HttpWebRequest request, T value, IProgress prog = null)
        {
            using (var stream = request.GetRequestStream())
            {
                serializer.Serialize(stream, value, prog);
            }
        }

        public static void Serialize<T>(this ISerializer<T> serializer, HttpListenerResponse response, T value, IProgress prog = null)
        {
            serializer.Serialize(response.OutputStream, value, prog);
        }

        public static Task SerializeAsync<T>(this ISerializer<T> serializer, WebSocketConnection socket, T value, IProgress prog = null)
        {
            return socket.SendAsync(serializer.Serialize(value, prog));
        }

        public static Task SerializeAsync<T>(this ISerializer<T> serializer, WebSocketConnection socket, string message, T value)
        {
            return socket.SendAsync(message, value, serializer);
        }

        public static byte[] Serialize<T>(this ISerializer<T> serializer, T value, IProgress prog = null)
        {
            using (var mem = new MemoryStream())
            {
                serializer.Serialize(mem, value, prog);
                mem.Flush();

                return mem.ToArray();
            }
        }

        public static void Serialize<T>(this ISerializer<T> serializer, FileInfo file, T value, IProgress prog = null)
        {
            using (var stream = file.Create())
            {
                serializer.Serialize(stream, value, prog);
            }
        }

        public static void Serialize<T>(this ISerializer<T> serializer, string fileName, T value, IProgress prog = null)
        {
            serializer.Serialize(new FileInfo(fileName), value, prog);
        }

        public static string ToString<T>(this ISerializer<T> serializer, T value, IProgress prog = null)
        {
            return Encoding.UTF8.GetString(serializer.Serialize(value, prog));
        }
    }
}
