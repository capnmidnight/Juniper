using System;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Juniper.HTTP
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class RouteAttribute : Attribute

    {
        public readonly Regex pattern;
        public readonly string regexSource;
        public readonly int parameterCount;

        public int Priority;
        public HttpProtocol Protocol = HttpProtocol.All;
        public HttpMethod Method = HttpMethod.GET;
        public bool Continue = false;
        public AuthenticationSchemes Authentication = AuthenticationSchemes.Anonymous;

        public RouteAttribute(Regex pattern)
        {
            this.pattern = pattern;

            regexSource = pattern.ToString();
            parameterCount = pattern.GetGroupNames().Length;
        }

        public RouteAttribute(string pattern)
            : this(new Regex(pattern, RegexOptions.Compiled))
        { }
    }
}