using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using Juniper.Logic;

using static Juniper.Logic.LogicConstructor;

namespace Juniper.HTTP.Server
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class RouteAttribute :
        Attribute,
        IRequestHandler
    {
        public Regex Pattern { get; }

        public string RegexSource { get; }

        public int ParameterCount { get; }

        public int Priority { get; set; } = 0;

        public string Name { get; set; } = null;

        public HttpMethods Methods { get; set; } = HttpMethods.GET;

        public AuthenticationSchemes Authentication { get; set; } = AuthenticationSchemes.Anonymous;

        public HttpProtocols Protocols { get; set; } = HttpProtocols.Default;

        public IExpression<HttpStatusCode> StatusCodes { get; set; } = Expr(HttpStatusCode.Continue);

        public IExpression<MediaType> Accept { get; set; } = Expr(MediaType.Any);

        public IExpression<(string Key, string Value)> Headers { get; set; } = Empty<(string Key, string Value)>();

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