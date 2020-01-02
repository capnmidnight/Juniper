using System.Net;
using System.Net.WebSockets;

namespace Juniper.HTTP.Server
{
    public class ServerWebSocketConnection : WebSocketConnection
    {
        public readonly HttpListenerContext httpContext;

        public ServerWebSocketConnection(HttpListenerContext httpContext, WebSocket socket, int rxBufferSize = DEFAULT_RX_BUFFER_SIZE, int dataBufferSize = DEFAULT_DATA_BUFFER_SIZE)
            : base(socket, rxBufferSize, dataBufferSize)
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
