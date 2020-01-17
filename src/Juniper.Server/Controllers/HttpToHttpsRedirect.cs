using System;
using System.Net;
using System.Threading.Tasks;

using static Juniper.Logic.LogicConstructor;

namespace Juniper.HTTP.Server.Controllers
{
    public sealed class HttpToHttpsRedirect :
        AbstractResponse
    {
        public HttpToHttpsRedirect()
            : base(int.MinValue + 1,
#if DEBUG
                // Don't attempt the redirect if we're running in DEBUG mode
                HttpProtocols.None,
#else
                HttpProtocols.HTTP,
#endif
                HttpMethods.GET,
                AllRoutes,
                AllAuthSchemes,
                AnyMediaTypes,
                HttpStatusCode.Continue,
                Expr(("Upgrade-Insecure-Requests", "1"))
            )
        { }

        protected override Task InvokeAsync(HttpListenerContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (Server.HttpsPort is null)
            {
#if DEBUG
                OnWarning("Server is not listening for the HTTPS protocol");
#else
                throw new InvalidOperationException("Cannot redirect to HTTPS when the server isn't listening for HTTPS requests");
#endif
            }

            var secureUrl = new UriBuilder(context.Request.Url)
            {
                Scheme = "https"
            };

            if (Server.HttpsPort != 443)
            {
                secureUrl.Port = Server.HttpsPort.Value;
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