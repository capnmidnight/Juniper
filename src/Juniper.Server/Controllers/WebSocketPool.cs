using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Juniper.HTTP.Server.Controllers
{
    public class WebSocketPool
    {
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
                var wsContext = await context.AcceptWebSocketAsync(null)
                    .ConfigureAwait(false);

                var socket = new ServerWebSocketConnection(context, wsContext.WebSocket);

                socket.Closed += Socket_Closed;
                sockets.Add(id, socket);
            }

            return sockets[id];
        }
    }
}
