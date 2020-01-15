using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Juniper.HTTP.Server.Controllers
{
    public class WebSocketPool
    {
        private static readonly Dictionary<string, string> userNames = new Dictionary<string, string>();
        public static void SetUserToken(string user, string token)
        {
            userNames[token] = user;
        }

        private readonly Dictionary<int, WebSocketConnection> sockets = new Dictionary<int, WebSocketConnection>();

        private void Socket_Closed(object sender, EventArgs e)
        {
            if (sender is WebSocketConnection socket)
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

        public async Task<WebSocketConnection> GetAsync(HttpListenerContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var id = context.GetHashCode();
            if (!sockets.ContainsKey(id))
            {
                var token = context.Request.Headers["Sec-WebSocket-Protocol"];
                if (token is null || !userNames.ContainsKey(token))
                {
                    context.Response.SetStatus(HttpStatusCode.Unauthorized);
                }
                else
                {
                    var wsContext = await context.AcceptWebSocketAsync(token)
                        .ConfigureAwait(false);

                    var socket = new ServerWebSocketConnection(context, wsContext.WebSocket, userNames[token]);

                    socket.Closed += Socket_Closed;
                    sockets.Add(id, socket);

                    return sockets[id];
                }
            }

            return null;
        }
    }
}
