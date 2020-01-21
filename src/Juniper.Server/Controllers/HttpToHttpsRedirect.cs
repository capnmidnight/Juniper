using System;
using System.Net;
using System.Threading.Tasks;

using static Juniper.Logic.LogicConstructor;

namespace Juniper.HTTP.Server.Controllers
{
    public sealed class HttpToHttpsRedirect :
        AbstractResponse
    {
        private readonly int redirectPort;

        public HttpToHttpsRedirect(int redirectPort)
            : base(int.MinValue + 1,
                HttpProtocols.HTTP,
                HttpMethods.GET,
                AllRoutes,
                AllAuthSchemes,
                AnyMediaTypes,
                HttpStatusCode.Continue,
                Expr(("Upgrade-Insecure-Requests", "1"))
            )
        {
            this.redirectPort = redirectPort;
        }

        protected override Task InvokeAsync(HttpListenerContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var secureUrl = new UriBuilder(context.Request.Url)
            {
                Scheme = "https"
            };

            if (redirectPort != 443)
            {
                secureUrl.Port = redirectPort;
            }
            else
            {
                secureUrl.Port = -1;
            }

            context.Response.Redirect(secureUrl.ToString());
            return Task.CompletedTask;
        }
    }
}