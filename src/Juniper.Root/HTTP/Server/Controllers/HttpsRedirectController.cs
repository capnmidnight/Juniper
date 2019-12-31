using System;
using System.Net;
using System.Threading.Tasks;

namespace Juniper.HTTP.Server.Controllers
{
    public sealed class HttpsRedirectController : AbstractRouteHandler
    {
        private HttpsRedirectController()
            : base(null, int.MinValue + 1, HttpProtocols.HTTP, HttpMethods.All)
        { }

        public override Task InvokeAsync(HttpListenerContext context)
        {
            var secureUrl = new UriBuilder(context.Request.Url)
            {
                Scheme = "https",
                Port = -1
            };

            if (Enum.TryParse<HttpMethods>(context.Request.HttpMethod, true, out var method))
            {
                context.Response.RedirectLocation = secureUrl.ToString();
                context.Response.StatusCode = (int)(method == HttpMethods.GET
                    ? HttpStatusCode.Redirect
                    : HttpStatusCode.RedirectKeepVerb);
            }

            return Task.CompletedTask;
        }
    }
}