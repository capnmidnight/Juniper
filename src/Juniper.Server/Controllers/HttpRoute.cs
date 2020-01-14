using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace Juniper.HTTP.Server.Controllers
{
    public class HttpRoute : AbstractRoute
    {
        public HttpRoute(object source, MethodInfo method, RouteAttribute route)
            : base(source, method, route)
        { }

        public override bool IsMatch(HttpListenerRequest request)
        {
            if (request is null)
            {
                throw new System.ArgumentNullException(nameof(request));
            }

            return base.IsMatch(request)
                && !request.IsWebSocketRequest;
        }

        public override Task InvokeAsync(HttpListenerContext context)
        {
            return InvokeAsync(context, context);
        }
    }
}