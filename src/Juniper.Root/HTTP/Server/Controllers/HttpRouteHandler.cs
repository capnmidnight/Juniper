using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Juniper.HTTP.Server.Controllers
{
    internal class HttpRouteHandler : AbstractRegexRouteHandler
    {
        internal HttpRouteHandler(string name, object source, MethodInfo method, RouteAttribute route)
            : base(name, source, method, route)
        { }

        public override bool IsMatch(HttpListenerRequest request)
        {
            return base.IsMatch(request)
                && !request.IsWebSocketRequest;
        }

        public override Task InvokeAsync(HttpListenerContext context)
        {
            return InvokeAsync(context, context);
        }
    }
}