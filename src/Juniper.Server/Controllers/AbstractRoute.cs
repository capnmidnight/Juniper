using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Juniper.HTTP.Server.Controllers
{
    public abstract class AbstractRoute : AbstractResponse
    {
        private readonly object source;
        private readonly MethodInfo action;

        private static string MakeName(MethodInfo action, RouteAttribute route)
        {
            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            if (route is null)
            {
                throw new ArgumentNullException(nameof(route));
            }

            if (route.Name is object)
            {
                return route.Name;
            }
            else
            {
                return $"{action.DeclaringType.Name}::{action.Name}";
            }
        }

        protected AbstractRoute(object source, MethodInfo action, RouteAttribute route)
            : base(route, MakeName(action, route))
        {
            this.source = source;
            this.action = action;
        }

        protected Task InvokeAsync(HttpListenerContext context, object firstParam)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var path = context.Request.Url.PathAndQuery;
            var args = Pattern
                .Match(path)
                .Groups
                .Cast<Group>()
                .Skip(1)
                .Select(g => Uri.UnescapeDataString(g.Value))
                .Cast<object>()
                .Prepend(firstParam)
                .ToArray();
            return (Task)action.Invoke(source, args);
        }

        public override bool Equals(object obj)
        {
            return obj is AbstractRoute handler
                && base.Equals(obj)
                && EqualityComparer<Regex>.Default.Equals(Pattern, handler.Pattern);
        }

        public override int GetHashCode()
        {
            var hashCode = -1402022977;
            hashCode = (hashCode * -1521134295) + base.GetHashCode();
            hashCode = (hashCode * -1521134295) + EqualityComparer<object>.Default.GetHashCode(source);
            hashCode = (hashCode * -1521134295) + EqualityComparer<MethodInfo>.Default.GetHashCode(action);
            return hashCode;
        }

        public override string ToString()
        {
            return $"{base.ToString()}({RegexSource})";
        }

        public override int CompareTo(AbstractResponse other)
        {
            var compare = base.CompareTo(other);
            if (compare == 0 && other is AbstractRoute handler)
            {
                // longer routes before shorter routes
                return -string.CompareOrdinal(RegexSource, handler.RegexSource);
            }
            else
            {
                return compare;
            }
        }
    }
}