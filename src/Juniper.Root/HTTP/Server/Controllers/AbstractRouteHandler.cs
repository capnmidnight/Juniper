using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Juniper.HTTP.Server.Controllers
{
    internal abstract class AbstractRouteHandler : AbstractRequestHandler
    {
        private readonly Regex pattern;
        private readonly string regexSource;
        private readonly int parameterCount;

        private readonly object source;
        private readonly MethodInfo method;

        protected AbstractRouteHandler(string name, object source, MethodInfo method, RouteAttribute route)
            : base(name, route.Priority, route.Protocol, route.Method, route.Continue, route.Authentication)
        {
            if (route is null)
            {
                throw new ArgumentNullException(nameof(route));
            }

            pattern = route.Pattern;
            regexSource = pattern.ToString();
            parameterCount = pattern.GetGroupNames().Length;

            this.source = source;
            this.method = method;
        }

        public override bool IsMatch(HttpListenerRequest request)
        {
            var urlMatch = pattern.Match(request.Url.PathAndQuery);
            return urlMatch.Success
                && urlMatch.Groups.Count == parameterCount
                && base.IsMatch(request);
        }

        protected Task InvokeAsync(HttpListenerContext context, object firstParam)
        {
            var path = context.Request.Url.PathAndQuery;
            var args = pattern
                .Match(path)
                .Groups
                .Cast<Group>()
                .Skip(1)
                .Select(g => Uri.UnescapeDataString(g.Value))
                .Cast<object>()
                .Prepend(firstParam)
                .ToArray();
            return (Task)method.Invoke(source, args);
        }

        public override bool Equals(object obj)
        {
            return obj is AbstractRouteHandler handler
                && base.Equals(obj)
                && EqualityComparer<Regex>.Default.Equals(pattern, handler.pattern);
        }

        public override int GetHashCode()
        {
            var hashCode = -1402022977;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<Regex>.Default.GetHashCode(pattern);
            hashCode = hashCode * -1521134295 + EqualityComparer<object>.Default.GetHashCode(source);
            hashCode = hashCode * -1521134295 + EqualityComparer<MethodInfo>.Default.GetHashCode(method);
            return hashCode;
        }

        public override string ToString()
        {
            return $"{base.ToString()}({regexSource})";
        }

        public override int CompareTo(AbstractRequestHandler other)
        {
            var compare = base.CompareTo(other);
            if (compare == 0 && other is AbstractRouteHandler handler)
            {
                // longer routes before shorter routes
                return -string.CompareOrdinal(regexSource, handler.regexSource);
            }
            else
            {
                return compare;
            }
        }
    }
}