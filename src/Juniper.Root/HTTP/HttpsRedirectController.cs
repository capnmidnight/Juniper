using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Juniper.HTTP
{
    public sealed class HttpsRedirectController
    {
        private HttpsRedirectController() { }

        [Route(".*",
            Priority = int.MinValue,
            Protocol = HttpProtocols.HTTP)]
        public static Task Redirect(HttpListenerContext context)
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