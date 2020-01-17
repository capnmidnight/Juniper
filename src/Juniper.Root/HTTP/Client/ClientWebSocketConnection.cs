using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Juniper.HTTP.Client
{
    public class ClientWebSocketConnection : WebSocketConnection<ClientWebSocket>
    {
        private readonly Uri uri;

        public ClientWebSocketConnection(Uri uri, string token, int rxBufferSize = DEFAULT_RX_BUFFER_SIZE, int dataBufferSize = DEFAULT_DATA_BUFFER_SIZE)
            : base(ValidateSocket(token), rxBufferSize, dataBufferSize)
        {
            this.uri = uri;
        }

        private static ClientWebSocket ValidateSocket(string token)
        {
            var socket = new ClientWebSocket();

            socket.Options.AddSubProtocol(token);

            return socket;
        }

        public async Task ConnectAsync()
        {
            var clientSocket = Socket;
            await clientSocket
                .ConnectAsync(uri, CancellationToken.None)
                .ConfigureAwait(false);
        }
    }
}
