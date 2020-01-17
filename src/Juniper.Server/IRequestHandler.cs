using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using Juniper.Logic;

namespace Juniper.HTTP.Server
{
    public interface IRequestHandler
    {
        int Priority { get; }
        HttpProtocols Protocols { get; }
        HttpMethods Methods { get; }
        Regex Pattern { get; }
        AuthenticationSchemes Authentication { get; }
        IExpression<MediaType> Accept { get; }
        IExpression<HttpStatusCode> StatusCodes { get; }
        IExpression<(string Key, string Value)> Headers { get; }
    }
}