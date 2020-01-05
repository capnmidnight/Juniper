using System.Net;
using System.Threading.Tasks;

namespace Juniper.HTTP.Server.Controllers
{
    public class UnhandledRequestController : AbstractRequestHandler
    {
        public UnhandledRequestController()
            : base(int.MaxValue)
        {
            Protocol = HttpProtocols.All;
            Verb = HttpMethods.All;
        }

        public override Task InvokeAsync(HttpListenerContext context)
        {
            if (context is null)
            {
                throw new System.ArgumentNullException(nameof(context));
            }

            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            return Task.CompletedTask;
        }
    }
}
