using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Juniper.HTTP
{
    public class WebSocketRouteHandler : AbstractRouteHandler
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

        internal override async Task Invoke(HttpListenerContext httpContext)
        {
            var wsContext = await httpContext.AcceptWebSocketAsync(null)
                .ConfigureAwait(false);

            var ws = new WebSocketConnection(httpContext, wsContext);
            SocketConnected?.Invoke(ws);

            await Invoke(GetStringArguments(httpContext)
                .Cast<object>()
                .Prepend(ws)
                .ToArray())
                .ConfigureAwait(false);
        }
    }
}
