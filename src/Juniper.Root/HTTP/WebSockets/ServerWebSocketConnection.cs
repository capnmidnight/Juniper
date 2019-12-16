using System.Net;
using System.Net.WebSockets;

using Juniper.HTTP.WebSockets;

namespace Juniper.HTTP.WebSockets
{
    public class ServerWebSocketConnection : WebSocketConnection
    {
        public readonly HttpListenerContext httpContext;

        public ServerWebSocketConnection(HttpListenerContext httpContext, WebSocket socket)
            : base(socket)
        {
            this.httpContext = httpContext;
        }

        private bool disposedValue;

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposedValue)
            {
                if (disposing)
                {
                    httpContext.Response.Close();
                }

                disposedValue = true;
            }
        }
    }
}
