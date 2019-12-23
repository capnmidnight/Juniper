using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Juniper.HTTP
{
    internal class HttpRouteHandler : AbstractRouteHandler
    {
        internal HttpRouteHandler(string name, RouteAttribute route, object source, MethodInfo method)
            : base(name, route, source, method)
        { }

        public override bool IsMatch(HttpListenerRequest request)
        {
            return base.IsMatch(request)
                && !request.IsWebSocketRequest;
        }

        internal override Task InvokeAsync(HttpListenerContext context)
        {
            return InvokeAsync(GetStringArguments(context)
                .Cast<object>()
                .Prepend(context)
                .ToArray());
        }
    }
}