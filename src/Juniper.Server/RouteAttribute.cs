using System;
using System.Net;
using System.Text.RegularExpressions;

namespace Juniper.HTTP.Server
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class RouteAttribute : Attribute
    {
        public Regex Pattern { get; }

        public string RegexSource { get; }

        public int ParameterCount { get; }

        public int Priority;
        public bool Continue;

#if DEBUG
        public HttpProtocols Protocol = HttpProtocols.All;
#else
        public HttpProtocols Protocol = HttpProtocols.HTTPS;
#endif

        public HttpMethods Method = HttpMethods.GET;
        public AuthenticationSchemes Authentication = AuthenticationSchemes.Anonymous;

        public RouteAttribute(Regex pattern)
        {
            Pattern = pattern
                ?? throw new ArgumentNullException(nameof(pattern));
            RegexSource = pattern.ToString();
            ParameterCount = pattern.GetGroupNames().Length;
        }

        public RouteAttribute(string pattern)
            : this(new Regex(pattern, RegexOptions.Compiled))
        { }
    }
}