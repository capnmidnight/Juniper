using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Juniper.HTTP
{
    public abstract class AbstractRouteHandler :
        IEquatable<AbstractRouteHandler>,
        IComparable,
        IComparable<AbstractRouteHandler>
    {
        public readonly HttpProtocols Protocol;

        protected readonly Regex pattern;

        private readonly string regexSource;
        private readonly int parameterCount;
        private readonly string name;

        private readonly int priority;
        private readonly HttpMethods verb;

        private readonly object source;
        private readonly MethodInfo method;

        public AuthenticationSchemes Authentication { get; }

        public bool Continue { get; }

        protected AbstractRouteHandler()
        { }

        protected AbstractRouteHandler(string name, RouteAttribute route, object source, MethodInfo method)
        {
            if (route is null)
            {
                throw new ArgumentNullException(nameof(route));
            }

            this.name = name;
            pattern = route.Pattern;
            regexSource = pattern.ToString();
            parameterCount = pattern.GetGroupNames().Length;
            priority = route.Priority;
            Protocol = route.Protocol;
            verb = route.Method;
            Continue = route.Continue;
            Authentication = route.Authentication;
            this.source = source;
            this.method = method;
        }

        public virtual bool IsMatch(HttpListenerRequest request)
        {
            var urlMatch = pattern.Match(request.Url.PathAndQuery);
            return urlMatch.Success
                && urlMatch.Groups.Count == parameterCount
                && Enum.TryParse<HttpProtocols>(request.Url.Scheme, true, out var protocol)
                && Enum.TryParse<HttpMethods>(request.HttpMethod, true, out var verb)
                && (Protocol & protocol) != 0
                && (this.verb & verb) != 0;
        }

        internal abstract Task InvokeAsync(HttpListenerContext context);

        protected Task InvokeAsync(object[] args)
        {
            return (Task)method.Invoke(source, args);
        }

        protected IEnumerable<string> GetStringArguments(HttpListenerContext context)
        {
            var path = context.Request.Url.PathAndQuery;
            var match = pattern.Match(path);
            return match
                .Groups
                .Cast<Group>()
                .Skip(1)
                .Select(g => Uri.UnescapeDataString(g.Value));
        }

        public override bool Equals(object obj)
        {
            return obj is AbstractRouteHandler other
                && Equals(other);
        }

        public bool Equals(AbstractRouteHandler other)
        {
            return CompareTo(other) == 0;
        }

        public override string ToString()
        {
            return $"[{priority.ToString(CultureInfo.CurrentCulture)}] {name}({regexSource})";
        }

        public override int GetHashCode()
        {
            return priority.GetHashCode()
                ^ regexSource.GetHashCode();
        }

        public int CompareTo(object obj)
        {
            return CompareTo(obj as AbstractRouteHandler);
        }

        public int CompareTo(AbstractRouteHandler other)
        {
            if (other is null)
            {
                return -1;
            }
            else if (priority == other.priority)
            {
                // longer routes before shorter routes
                return string.CompareOrdinal(regexSource, other.regexSource);
            }
            else
            {
                // smaller Priority numbers before larger Priority numbers (i.e. 0 being the "Highest Priority")
                return priority.CompareTo(other.priority);
            }
        }

        public static bool operator ==(AbstractRouteHandler left, AbstractRouteHandler right)
        {
            return (left is null && right is null)
                || (left is object && left.CompareTo(right) == 0)
                || (right is object && right.CompareTo(left) == 0);
        }

        public static bool operator !=(AbstractRouteHandler left, AbstractRouteHandler right)
        {
            return !(left == right);
        }

        public static bool operator <(AbstractRouteHandler left, AbstractRouteHandler right)
        {
            return (left is object && left.CompareTo(right) == -1)
                || (right is object && right.CompareTo(left) == 1);
        }

        public static bool operator >(AbstractRouteHandler left, AbstractRouteHandler right)
        {
            return (left is object && left.CompareTo(right) == 1)
                || (right is object && right.CompareTo(left) == -1);
        }

        public static bool operator <=(AbstractRouteHandler left, AbstractRouteHandler right)
        {
            return left < right || left == right;
        }

        public static bool operator >=(AbstractRouteHandler left, AbstractRouteHandler right)
        {
            return left > right || left == right;
        }
    }
}