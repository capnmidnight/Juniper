using System.Net;
using System.Net.WebSockets;

namespace Juniper.HTTP.Server
{
    public class ServerWebSocketConnection : WebSocketConnection<WebSocket>
    {
        private readonly HttpListenerContext context;

        public string UserName { get; set; }

        public string Token
        {
            get
            {
                return Socket.SubProtocol;
            }
        }

        public ServerWebSocketConnection(HttpListenerContext httpContext, WebSocket socket, string userName, int rxBufferSize = DEFAULT_RX_BUFFER_SIZE, int dataBufferSize = DEFAULT_DATA_BUFFER_SIZE)
            : base(socket, rxBufferSize, dataBufferSize)
        {
            context = httpContext;
            UserName = userName;
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
