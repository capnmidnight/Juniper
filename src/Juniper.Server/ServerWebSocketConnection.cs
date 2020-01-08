using System.Net;
using System.Net.WebSockets;

namespace Juniper.HTTP.Server
{
    public class ServerWebSocketConnection : WebSocketConnection
    {
        private readonly HttpListenerContext context;

        public ServerWebSocketConnection(HttpListenerContext httpContext, WebSocket socket, int rxBufferSize = DEFAULT_RX_BUFFER_SIZE, int dataBufferSize = DEFAULT_DATA_BUFFER_SIZE)
            : base(socket, rxBufferSize, dataBufferSize)
        {
            context = httpContext;
        }

        private bool disposedValue;

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposedValue)
            {
                if (disposing)
                {
                    context.Response.Close();
                }

                disposedValue = true;
            }
        }
    }
}
