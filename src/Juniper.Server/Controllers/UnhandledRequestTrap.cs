using System.Net;
using System.Threading.Tasks;

namespace Juniper.HTTP.Server.Controllers
{
    public class UnhandledRequestTrap : AbstractResponse
    {
        public UnhandledRequestTrap()
            : base(int.MaxValue,
                AllProtocols,
                AllMethods,
                AllRoutes,
                AllAuthSchemes,
                AnyMediaTypes,
                HttpStatusCode.Continue,
                NoHeaders)
        { }

        protected override Task InvokeAsync(HttpListenerContext context)
        {
            if (context is null)
            {
                throw new System.ArgumentNullException(nameof(context));
            }

            var response = context.Response;
            response.SetStatus(HttpStatusCode.NotImplemented);

            return Task.CompletedTask;
        }
    }
}
