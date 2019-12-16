using System;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Juniper.HTTP.WebSockets;

namespace Juniper.HTTP.WebSockets
{
    public class ClientWebSocketConnection : WebSocketConnection
    {
        private readonly Uri uri;

        public ClientWebSocketConnection(Uri uri, int rxBufferSize = DEFAULT_RX_BUFFER_SIZE, int dataBufferSize = DEFAULT_DATA_BUFFER_SIZE)
            : base(new ClientWebSocket(), rxBufferSize, dataBufferSize)
        {
            this.uri = uri;
        }

        public async Task ConnectAsync()
        {
            var clientSocket = (ClientWebSocket)socket;
            await clientSocket
                .ConnectAsync(uri, canceller.Token)
                .ConfigureAwait(false);
        }
    }
}
