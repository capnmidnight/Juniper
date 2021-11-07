using Juniper.HTTP;

using System;
using System.IO;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace Juniper.IO
{
    public static class ISerializerExt
    {
        public static byte[] Serialize<T>(this ISerializer<T> serializer, T value)
        {
            if (serializer is null)
            {
                throw new ArgumentNullException(nameof(serializer));
            }

            using var mem = new MemoryStream();
            serializer.Serialize(mem, value);
            mem.Flush();

            return mem.ToArray();
        }

        public static void Serialize<T>(this ISerializer<T> serializer, FileInfo file, T value)
        {
            if (serializer is null)
            {
                throw new ArgumentNullException(nameof(serializer));
            }

            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            using var stream = file.Open(FileMode.Create, FileAccess.Write, FileShare.Read);
            serializer.Serialize(stream, value);
        }

        public static void Serialize<T>(this ISerializer<T> serializer, string fileName, T value)
        {
            if (fileName is null)
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            if (fileName.Length == 0)
            {
                throw new ArgumentException("path must not be empty string", nameof(fileName));
            }

            serializer.Serialize(new FileInfo(fileName), value);
        }

        public static void Serialize<T>(this ISerializer<T> serializer, HttpWebRequest request, MediaType type, T value)
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
            request.ContentType = type;
            request.ContentLength = serializer.Serialize(stream, value);
        }

        public static void Serialize<T>(this ISerializer<T> serializer, HttpListenerResponse response, MediaType type, T value)
        {
            if (serializer is null)
            {
                throw new ArgumentNullException(nameof(serializer));
            }

            if (response is null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            response.ContentType = type;
            response.ContentLength64 = serializer.Serialize(response.OutputStream, value);
        }

        public static Task SerializeAsync<T, U>(this ISerializer<T> serializer, WebSocketConnection<U> socket, T value)
            where U : WebSocket
        {
            if (serializer is null)
            {
                throw new ArgumentNullException(nameof(serializer));
            }

            if (socket is null)
            {
                throw new ArgumentNullException(nameof(socket));
            }

            var data = serializer.Serialize(value);
            return socket.SendAsync(data);
        }

        public static Task SerializeAsync<T, U>(this ISerializer<T> serializer, WebSocketConnection<U> socket, string message, T value)
            where U : WebSocket
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

        public static string ToString<T>(this ISerializer<T> serializer, T value)
        {
            if (serializer is null)
            {
                throw new ArgumentNullException(nameof(serializer));
            }

            return Encoding.UTF8.GetString(serializer.Serialize(value));
        }
    }
}
