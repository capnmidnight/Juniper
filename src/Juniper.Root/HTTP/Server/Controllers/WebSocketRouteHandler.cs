using System;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace Juniper.HTTP.Server.Controllers
{
    internal class WebSocketRouteHandler : AbstractRouteHandler
    {
        internal WebSocketRouteHandler(string name, object source, MethodInfo method, RouteAttribute route)
            : base(name, source, method, route)
        { }

        internal event Action<WebSocketConnection> SocketConnected;

        private WebSocketManager wsMgr;

        public override HttpServer Server
        {
            get { return base.Server; }

            set
            {
                base.Server = value;

                wsMgr = Server.GetController<WebSocketManager>();
                if (wsMgr is null)
                {
                    wsMgr = new WebSocketManager();
                    Server.AddController(wsMgr);
                }
            }
        }

        public override bool IsMatch(HttpListenerRequest request)
        {
            return base.IsMatch(request)
                && request.IsWebSocketRequest;
        }

        public override async Task InvokeAsync(HttpListenerContext httpContext)
        {
            if (wsMgr is null)
            {
                OnError(new NullReferenceException("No web socket manager"));
                httpContext.Response.Error(HttpStatusCode.InternalServerError, "Server error");
            }
            else
            {
                var wsContext = await httpContext.AcceptWebSocketAsync(null)
                    .ConfigureAwait(false);

                var ws = new ServerWebSocketConnection(httpContext, wsContext.WebSocket);
                wsMgr.Add(ws);

                SocketConnected?.Invoke(ws);

                await InvokeAsync(httpContext, ws)
                    .ConfigureAwait(false);
            }
        }
    }
}
