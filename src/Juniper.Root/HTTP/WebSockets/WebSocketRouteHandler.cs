using System;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace Juniper.HTTP.WebSockets
{
    internal class WebSocketRouteHandler : AbstractRegexRouteHandler
    {
        internal WebSocketRouteHandler(string name, object source, MethodInfo method, RouteAttribute route)
            : base(name, source, method, route)
        { }

        internal event Action<WebSocketConnection> SocketConnected;

        public override bool IsMatch(HttpListenerRequest request)
        {
            return base.IsMatch(request)
                && request.IsWebSocketRequest;
        }

        public override async Task InvokeAsync(HttpListenerContext httpContext)
        {
            var wsContext = await httpContext.AcceptWebSocketAsync(null)
                .ConfigureAwait(false);

            var ws = new ServerWebSocketConnection(httpContext, wsContext.WebSocket);
            SocketConnected?.Invoke(ws);

            await InvokeAsync(httpContext, ws)
                .ConfigureAwait(false);
        }
    }
}
