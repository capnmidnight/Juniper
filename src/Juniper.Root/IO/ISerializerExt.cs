using System;
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
            if (serializer is null)
            {
                throw new ArgumentNullException(nameof(serializer));
            }

            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            using var stream = request.GetRequestStream();
            serializer.Serialize(stream, value, prog);
        }

        public static void Serialize<T>(this ISerializer<T> serializer, HttpListenerResponse response, T value, IProgress prog = null)
        {
            if (serializer is null)
            {
                throw new ArgumentNullException(nameof(serializer));
            }

            if (response is null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            serializer.Serialize(response.OutputStream, value, prog);
        }

        public static Task SerializeAsync<T>(this ISerializer<T> serializer, WebSocketConnection socket, T value, IProgress prog = null)
        {
            if (serializer is null)
            {
                throw new ArgumentNullException(nameof(serializer));
            }

            if (socket is null)
            {
                throw new ArgumentNullException(nameof(socket));
            }

            return socket.SendAsync(serializer.Serialize(value, prog));
        }

        public static Task SerializeAsync<T>(this ISerializer<T> serializer, WebSocketConnection socket, string message, T value)
        {
            if (serializer is null)
            {
                throw new ArgumentNullException(nameof(serializer));
            }

            if (socket is null)
            {
                throw new ArgumentNullException(nameof(socket));
            }

            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            return socket.SendAsync(message, value, serializer);
        }

        public static byte[] Serialize<T>(this ISerializer<T> serializer, T value, IProgress prog = null)
        {
            if (serializer is null)
            {
                throw new ArgumentNullException(nameof(serializer));
            }

            using var mem = new MemoryStream();
            serializer.Serialize(mem, value, prog);
            mem.Flush();

            return mem.ToArray();
        }

        public static void Serialize<T>(this ISerializer<T> serializer, FileInfo file, T value, IProgress prog = null)
        {
            if (serializer is null)
            {
                throw new ArgumentNullException(nameof(serializer));
            }

            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            using var stream = file.Create();
            serializer.Serialize(stream, value, prog);
        }

        public static void Serialize<T>(this ISerializer<T> serializer, string fileName, T value, IProgress prog = null)
        {
            if (serializer is null)
            {
                throw new ArgumentNullException(nameof(serializer));
            }

            serializer.Serialize(new FileInfo(fileName.ValidateFileName()), value, prog);
        }

        public static string ToString<T>(this ISerializer<T> serializer, T value, IProgress prog = null)
        {
            if (serializer is null)
            {
                throw new ArgumentNullException(nameof(serializer));
            }

            return Encoding.UTF8.GetString(serializer.Serialize(value, prog));
        }
    }
}
