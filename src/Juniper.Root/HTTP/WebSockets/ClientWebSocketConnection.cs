using System;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Juniper.HTTP.WebSockets;

namespace Juniper.HTTP.WebSockets
{
    public class ClientWebSocketConnection : WebSocketConnection
    {
        private readonly Uri uri;

        public ClientWebSocketConnection(Uri uri)
            : base(new ClientWebSocket())
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
