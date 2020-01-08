using System.Net;
using System.Threading.Tasks;

namespace Juniper.HTTP.Server.Controllers
{
    public class UnhandledRequestTrap : AbstractResponse
    {
        public UnhandledRequestTrap()
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

            context.Response.SetStatus(HttpStatusCode.NotFound);
            return Task.CompletedTask;
        }
    }
}
