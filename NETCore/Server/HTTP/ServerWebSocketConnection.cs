using System.Net;
using System.Net.WebSockets;

namespace Juniper.HTTP;

public class ServerWebSocketConnection : WebSocketConnection<WebSocket>
{
    private HttpListenerContext? context;

    public string UserName { get; set; }

    public string? Token => Socket?.SubProtocol;

    public ServerWebSocketConnection(HttpListenerContext httpContext, WebSocket socket, string userName, int rxBufferSize = DEFAULT_RX_BUFFER_SIZE, int dataBufferSize = DEFAULT_DATA_BUFFER_SIZE)
        : base(socket, rxBufferSize, dataBufferSize)
    {
        context = httpContext;
        UserName = userName;
    }

    public ServerWebSocketConnection(WebSocket socket, string userName, int rxBufferSize = DEFAULT_RX_BUFFER_SIZE, int dataBufferSize = DEFAULT_DATA_BUFFER_SIZE)
        : base(socket, rxBufferSize, dataBufferSize)
    {
        context = null;
        disposedValue = true;
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
                context?.Response?.Close();
                context = null;
            }

            disposedValue = true;
        }
    }
}
