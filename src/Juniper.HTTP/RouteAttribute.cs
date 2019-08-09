using System;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Juniper.HTTP
{
    public sealed class RouteAttribute : Attribute
    {
        private readonly Regex pattern;
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
            parameterCount = pattern.GetGroupNames().Length;
        }

        public RouteAttribute(string pattern)
            : this(new Regex(pattern, RegexOptions.Compiled)) { }

        public bool IsMatch(HttpListenerRequest request)
        {
            return request.HttpMethod == Method
                && pattern.IsMatch(request.Url.PathAndQuery);
        }

        public void Invoke(HttpListenerContext context)
        {
            var path = context.Request.Url.PathAndQuery;
            var match = pattern.Match(path);
            var args = match
                .Groups
                .Cast<Group>()
                .Skip(1)
                .Select(g => g.Value)
                .Cast<object>()
                .Prepend(context)
                .ToArray();
            method.Invoke(source, args);
        }
    }
}