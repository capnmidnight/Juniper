using System;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Juniper.HTTP
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class RouteAttribute :
        Attribute,
        IEquatable<RouteAttribute>,
        IComparable,
        IComparable<RouteAttribute>

    {
        private readonly Regex pattern;
        private readonly string regexSource;
        public readonly int parameterCount;

        public int Priority = 50;
        public string Method = "GET";
        public bool Continue = false;
        public AuthenticationSchemes Authentication = AuthenticationSchemes.Anonymous;

        internal object source;
        internal MethodInfo method;

        public RouteAttribute(Regex pattern)
        {
            this.pattern = pattern;

            regexSource = pattern.ToString();
            parameterCount = pattern.GetGroupNames().Length;
        }

        public RouteAttribute(string pattern)
            : this(new Regex(pattern, RegexOptions.Compiled))
        { }

        public bool IsMatch(HttpListenerRequest request)
        {
            return request.HttpMethod == Method
                && pattern.IsMatch(request.Url.PathAndQuery);
        }

        public Task Invoke(HttpListenerContext context)
        {
            var path = context.Request.Url.PathAndQuery;
            var match = pattern.Match(path);
            var args = match
                .Groups
                .Cast<Group>()
                .Skip(1)
                .Select(g => Uri.UnescapeDataString(g.Value))
                .Cast<object>()
                .Prepend(context)
                .ToArray();
            return (Task)method.Invoke(source, args);
        }

        public override bool Equals(object obj)
        {
            return obj is RouteAttribute other
                && Equals(other);
        }

        public bool Equals(RouteAttribute other)
        {
            return CompareTo(other) == 0;
        }

        public override string ToString()
        {
            return $"[{Priority} {regexSource}";
        }

        public override int GetHashCode()
        {
            return Priority.GetHashCode()
                ^ regexSource.GetHashCode();
        }

        public int CompareTo(object obj)
        {
            return CompareTo(obj as RouteAttribute);
        }

        public int CompareTo(RouteAttribute other)
        {
            if(other is null)
            {
                return -1;
            }
            else if(Priority == other.Priority)
            {
                // longer routes before shorter routes
                return -regexSource.CompareTo(other.regexSource);
            }
            else
            {
                // smaller Priority numbers before larger Priority numbers (i.e. 0 being the "Highest Priority")
                return Priority.CompareTo(other.Priority);
            }
        }

        public static bool operator ==(RouteAttribute left, RouteAttribute right)
        {
            return left is null && right is null
                || left is object && left.CompareTo(right) == 0
                || right is object && right.CompareTo(left) == 0;
        }

        public static bool operator !=(RouteAttribute left, RouteAttribute right)
        {
            return !(left == right);
        }

        public static bool operator <(RouteAttribute left, RouteAttribute right)
        {
            return left is object && left.CompareTo(right) == -1
                || right is object && right.CompareTo(left) == 1;
        }

        public static bool operator >(RouteAttribute left, RouteAttribute right)
        {
            return left is object && left.CompareTo(right) == 1
                || right is object && right.CompareTo(left) == -1;
        }

        public static bool operator <=(RouteAttribute left, RouteAttribute right)
        {
            return left < right || left == right;
        }

        public static bool operator >=(RouteAttribute left, RouteAttribute right)
        {
            return left > right || left == right;
        }
    }
}