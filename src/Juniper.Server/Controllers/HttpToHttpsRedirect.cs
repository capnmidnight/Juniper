using System;
using System.Net;
using System.Threading.Tasks;

namespace Juniper.HTTP.Server.Controllers
{
    public sealed class HttpToHttpsRedirect : AbstractResponse
    {
        public HttpToHttpsRedirect()
            : base(int.MinValue + 1,
                  HttpProtocols.HTTP,
                  HttpMethods.GET,
                  HttpStatusCode.OK,
                  AnyAuth,
                  MediaType.All)
        { }

        public override bool IsMatch(HttpListenerRequest request)
        {
            if(Server.HttpsPort is null)
            {
                OnWarning("The server isn't listening for HTTPS requests!");
            }

            return Server.HttpsPort is object
                && request.Headers["Upgrade-Insecure-Requests"] == "1"
                && base.IsMatch(request);
        }

        public override Task InvokeAsync(HttpListenerContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (Server.HttpsPort is null)
            {
                throw new InvalidOperationException("Cannot redirect to HTTPS when the server isn't listening for HTTPS requests");
            }

            var secureUrl = new UriBuilder(context.Request.Url)
            {
                Scheme = "https"
            };

            if(Server.HttpsPort != 443)
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