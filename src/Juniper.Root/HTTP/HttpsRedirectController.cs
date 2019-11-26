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
        public Task ServeFile(HttpListenerContext context)
        {
            var secureUrl = new UriBuilder(context.Request.Url)
            {
                Scheme = "https",
                Port = -1
            };

            context.Response.Redirect(secureUrl.ToString());

            return Task.CompletedTask;
        }
    }
}