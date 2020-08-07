using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Juniper.HTTP.Server
{
    public class WebSocketPool
    {
        protected static readonly Dictionary<string, string> userNames = new Dictionary<string, string>();
        public static void SetUserToken(string userName, string token)
        {
            lock (userNames)
            {
                userNames[token] = userName;
            }
        }

        protected readonly Dictionary<int, ServerWebSocketConnection> sockets = new Dictionary<int, ServerWebSocketConnection>();

        public IReadOnlyCollection<ServerWebSocketConnection> Sockets => sockets.Values;

        protected void Socket_Closed(object sender, EventArgs e)
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
                var token = context.Request.Headers["Sec-WebSocket-Protocol"];
                if (token is object && !userNames.ContainsKey(token))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                }
                else
                {
                    var wsContext = await context.AcceptWebSocketAsync(token)
                        .ConfigureAwait(false);

                    var socket = new ServerWebSocketConnection(context, wsContext.WebSocket, userNames.Get(token));

                    socket.Closed += Socket_Closed;
                    sockets.Add(id, socket);
                }
            }

            return sockets[id];
        }

#if NETCOREAPP
        public async Task<ServerWebSocketConnection> GetAsync(Microsoft.AspNetCore.Http.HttpContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var id = context.GetHashCode();
            if (!sockets.ContainsKey(id))
            {
                var token = context.Request.Headers["Sec-WebSocket-Protocol"];
                if (token.Count() == 1 && !userNames.ContainsKey(token[0]))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                }
                else
                {
                    var webSocket = await context
                        .WebSockets
                        .AcceptWebSocketAsync(token)
                        .ConfigureAwait(false);

                    var socket = new ServerWebSocketConnection(webSocket, userNames.Get(token));

                    socket.Closed += Socket_Closed;
                    sockets.Add(id, socket);
                }
            }

            return sockets[id];
        }
#endif
    }
}
