using System;
using System.Collections.Generic;

namespace Juniper.HTTP.Server.Controllers
{
    internal class WebSocketManager
    {
        private readonly List<WebSocketConnection> sockets = new List<WebSocketConnection>();

        public void Add(WebSocketConnection socket)
        {
            socket.Closed += Socket_Closed;
            sockets.Add(socket);
        }

        private void Socket_Closed(object sender, EventArgs e)
        {
            if (sender is WebSocketConnection socket)
            {
                _ = sockets.Remove(socket);
                socket.Closed -= Socket_Closed;
                socket.Dispose();
            }
        }
    }
}
