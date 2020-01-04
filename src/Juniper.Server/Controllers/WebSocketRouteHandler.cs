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

        public override bool IsMatch(HttpListenerContext context)
        {
            return base.IsMatch(context)
                && context.Request.IsWebSocketRequest;
        }

        public override async Task InvokeAsync(HttpListenerContext context)
        {
            if (wsMgr is null)
            {
                OnError(new NullReferenceException("No web socket manager"));
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            else
            {
                var wsContext = await context.AcceptWebSocketAsync(null)
                    .ConfigureAwait(false);

                var ws = new ServerWebSocketConnection(context, wsContext.WebSocket);
                wsMgr.Add(ws);

                SocketConnected?.Invoke(ws);

                await InvokeAsync(context, ws)
                    .ConfigureAwait(false);
            }
        }
    }
}
