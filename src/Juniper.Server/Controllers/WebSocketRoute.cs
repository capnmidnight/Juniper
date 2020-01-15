using System;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace Juniper.HTTP.Server.Controllers
{
    internal class WebSocketRoute : AbstractRoute
    {
        internal WebSocketRoute(object source, MethodInfo method, RouteAttribute route)
            : base(source, method, route)
        { }

        private static readonly WebSocketPool socketPool = new WebSocketPool();

        public override bool IsMatch(HttpListenerContext context)
        {
            return base.IsMatch(context)
                && context.Request.IsWebSocketRequest;
        }

        public override async Task InvokeAsync(HttpListenerContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var response = context.Response;
            if (socketPool is null)
            {
                OnError(new NullReferenceException("No web socket manager"));
                response.SetStatus(HttpStatusCode.InternalServerError);
            }
            else
            {
                var socket = await socketPool.GetAsync(context)
                    .ConfigureAwait(false);

                await InvokeAsync(context, socket)
                    .ConfigureAwait(false);

                response.SetStatus(HttpStatusCode.Continue);
            }
        }
    }
}
