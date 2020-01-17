using System.Net;
using System.Reflection;
using System.Threading.Tasks;

using static Juniper.Logic.LogicConstructor;

namespace Juniper.HTTP.Server.Controllers
{
    public class HttpRoute : AbstractRoute
    {
        private static RouteAttribute ValidateRoute(RouteAttribute route)
        {
            if (route is null)
            {
                throw new System.ArgumentNullException(nameof(route));
            }

            var newRoute = new RouteAttribute(route.Pattern)
            {
                Accept = route.Accept,
                Authentication = route.Authentication,
                Headers = And(Not(("Connection", "Upgrade")), route.Headers),
                Methods = route.Methods,
                Name = route.Name,
                Priority = route.Priority,
                Protocols = route.Protocols,
                StatusCodes = route.StatusCodes
            };
            return newRoute;
        }

        public HttpRoute(object source, MethodInfo method, RouteAttribute route)
            : base(source, method, ValidateRoute(route))
        { }

        protected override Task InvokeAsync(HttpListenerContext context)
        {
            return InvokeAsync(context, context);
        }
    }
}