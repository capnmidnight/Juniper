using System;
using System.Collections.Generic;
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

        public int Priority { get; set; } = 0;

        public string Name { get; set; } = null;

        public HttpStatusCode ExpectedStatus { get; set; } = HttpStatusCode.OK;

        public HttpMethods Method { get; set; } = HttpMethods.GET;

        public AuthenticationSchemes Authentication { get; set; } = AuthenticationSchemes.Anonymous;

        public HttpProtocols Protocol { get; set; } = HttpProtocols.Default;

        public IReadOnlyList<MediaType> Accept { get; set; } = MediaType.All;

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