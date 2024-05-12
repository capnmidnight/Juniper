using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

using System.Net;

namespace Juniper.HTTP;

public class WebSocketPool
{
    protected static readonly Dictionary<string, string?> userNames = new
();
    public static void SetUserToken(string userName, string token)
    {
        lock (userNames)
        {
            userNames[token] = userName;
        }
    }

    private readonly Dictionary<int, ServerWebSocketConnection> sockets = new();

    public IReadOnlyCollection<ServerWebSocketConnection> Sockets => sockets.Values;

    protected void Socket_Closed(object? sender, EventArgs e)
    {
        if (sender is ServerWebSocketConnection socket)
        {
            socket.Closed -= Socket_Closed;
            socket.Dispose();

            var keys = (from s in sockets
                        where socket == s.Value
                        select s.Key);

            foreach (var key in keys)
            {
                _ = sockets.Remove(key);
            }
        }
    }

    public async Task<ServerWebSocketConnection> GetAsync(HttpListenerContext context)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        var id = context.GetHashCode();
        if (!sockets.ContainsKey(id))
        {
            var token = context.Request.Headers[HeaderNames.SecWebSocketProtocol];
            if (token is not null && userNames.ContainsKey(token))
            {
                var wsContext = await context.AcceptWebSocketAsync(token)
                    .ConfigureAwait(false);

                if (userNames.TryGetValue(token, out var userName)
                    && userName is not null)
                {
                    var socket = new ServerWebSocketConnection(context, wsContext.WebSocket, userName);
                    socket.Closed += Socket_Closed;
                    sockets.Add(id, socket);
                }
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            }
        }

        return sockets[id];
    }

    public async Task<ServerWebSocketConnection> GetAsync(HttpContext context)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        var id = context.GetHashCode();
        if (!sockets.ContainsKey(id))
        {
            var tokens = context.Request.Headers[HeaderNames.SecWebSocketProtocol];
            if (tokens.Count != 1
                || tokens[0] is not string token
                || !userNames.ContainsKey(token))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            }
            else
            {
                var webSocket = await context
                    .WebSockets
                    .AcceptWebSocketAsync(token)
                    .ConfigureAwait(false);

                if (userNames.TryGetValue(token, out var userName)
                    && userName is not null)
                {
                    var socket = new ServerWebSocketConnection(webSocket, userName);

                    socket.Closed += Socket_Closed;
                    sockets.Add(id, socket);
                }
            }
        }

        return sockets[id];
    }
}
