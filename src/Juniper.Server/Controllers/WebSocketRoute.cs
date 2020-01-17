using System;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

using static Juniper.Logic.LogicConstructor;

namespace Juniper.HTTP.Server.Controllers
{
    internal class WebSocketRoute : AbstractRoute
    {
        private static RouteAttribute ValidateRoute(RouteAttribute route)
        {
            if (route is null)
            {
                throw new ArgumentNullException(nameof(route));
            }

            var newRoute = new RouteAttribute(route.Pattern)
            {
                Accept = route.Accept,
                Authentication = route.Authentication,
                Headers = And(route.Headers, And(
                    ("Connection", "Upgrade"),
                    ("Upgrade", "Websocket")))
            };
            return newRoute;
        }

        public WebSocketRoute(object source, MethodInfo method, RouteAttribute route)
            : base(source, method, ValidateRoute(route))
        { }

        private static readonly WebSocketPool socketPool = new WebSocketPool();

        protected override async Task InvokeAsync(HttpListenerContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var response = context.Response;
            if (socketPool is null)
            {
                response.SetStatus(HttpStatusCode.InternalServerError);
#if DEBUG
                response.StatusDescription = "No web socket manager";
#endif
            }
            else
            {
                var socket = await socketPool.GetAsync(context)
                    .ConfigureAwait(false);

                await InvokeAsync(context, socket)
                    .ConfigureAwait(false);

                response.SetStatus(HttpStatusCode.OK);
            }
        }
    }
}
