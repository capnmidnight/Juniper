using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Juniper.HTTP
{
    public class HttpsRedirectController
    {
        public HttpsRedirectController()
        { }

        [Route(".*",
            Priority = int.MinValue,
            Protocol = HttpProtocol.HTTP,
            Method = HttpMethod.All)]
        public Task Redirect(HttpListenerContext context)
        {
            var secureUrl = new UriBuilder(context.Request.Url)
            {
                Scheme = "https",
                Port = -1
            };

            if (Enum.TryParse<HttpMethod>(context.Request.HttpMethod, true, out var method))
            {
                context.Response.RedirectLocation = secureUrl.ToString();
                context.Response.StatusCode = (int)(method == HttpMethod.GET
                    ? HttpStatusCode.Redirect
                    : HttpStatusCode.RedirectKeepVerb);
            }

            return Task.CompletedTask;
        }
    }
}