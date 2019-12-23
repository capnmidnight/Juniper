using System;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace Juniper.HTTP.WebSockets
{
    public class WebSocketRouteHandler :
        AbstractRouteHandler
    {
        public WebSocketRouteHandler(string name, RouteAttribute route, object source, MethodInfo method)
            : base(name, route, source, method)
        { }

        internal event Action<WebSocketConnection> SocketConnected;

        public override bool IsMatch(HttpListenerRequest request)
        {
            return base.IsMatch(request)
                && request.IsWebSocketRequest;
        }

        internal override async Task InvokeAsync(HttpListenerContext httpContext)
        {
            var wsContext = await httpContext.AcceptWebSocketAsync(null)
                .ConfigureAwait(false);

            var ws = new ServerWebSocketConnection(httpContext, wsContext.WebSocket);
            SocketConnected?.Invoke(ws);

            await InvokeAsync(GetStringArguments(httpContext)
                .Cast<object>()
                .Prepend(ws)
                .ToArray())
                .ConfigureAwait(false);
        }
    }
}
