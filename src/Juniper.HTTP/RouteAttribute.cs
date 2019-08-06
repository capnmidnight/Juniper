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

        public int Priority = 50;
        public string Method = "GET";
        public bool Continue = false;

        internal object source;
        internal MethodInfo method;

        public RouteAttribute(Regex pattern)
        {
            this.pattern = pattern;
        }

        public RouteAttribute(string pattern)
            : this(new Regex(pattern, RegexOptions.Compiled)) { }

        public string[] GetParams(HttpListenerContext context)
        {
            if (context.Request.HttpMethod != Method)
            {
                return default;
            }
            else
            {
                var path = context.Request.Url.PathAndQuery;
                var match = pattern.Match(path);
                if (!match.Success)
                {
                    return default;
                }
                else
                {
                    return match
                        .Groups
                        .Cast<Group>()
                        .Select(g => g.Value)
                        .ToArray();
                }
            }
        }

        private object[] args = new object[2];

        public void Invoke(HttpListenerContext context, string[] argValues)
        {
            args[0] = context;
            args[1] = argValues;
            method.Invoke(source, args);
        }
    }
}