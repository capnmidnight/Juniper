using Juniper.HTTP;

using System.Net.WebSockets;
using System.Text;

namespace Juniper.IO;

public static class ISerializerExt
{
    public static void Serialize<T, M>(this ISerializer<T, M> serializer, HttpRequestMessage request, MediaType type, T value)
        where M : MediaType
    {
        if (serializer is null)
        {
            throw new ArgumentNullException(nameof(serializer));
        }

        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var stream = serializer.GetStream(value);
        request.Body(new StreamContent(stream), type);
    }

    public static Task SerializeAsync<T, M, U>(this ISerializer<T, M> serializer, WebSocketConnection<U> socket, T value)
        where M : MediaType
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

    public static Task SerializeAsync<T, M, U>(this ISerializer<T, M> serializer, WebSocketConnection<U> socket, string message, T value)
        where M : MediaType
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
}
