using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Juniper.HTTP
{
    public class ClientWebSocketConnection : WebSocketConnection<ClientWebSocket>
    {
        private readonly Uri uri;

        public ClientWebSocketConnection(Uri uri, string token, int rxBufferSize = DEFAULT_RX_BUFFER_SIZE, int dataBufferSize = DEFAULT_DATA_BUFFER_SIZE)
            : base(rxBufferSize, dataBufferSize)
        {
            try
            {
                Socket = new ClientWebSocket();
                Socket.Options.AddSubProtocol(token);
                this.uri = uri;
            }
            catch
            {
                Socket.Dispose();
                throw;
            }
        }

        public Task ConnectAsync()
        {
            return Socket.ConnectAsync(uri, CancellationToken.None);
        }
    }
}
