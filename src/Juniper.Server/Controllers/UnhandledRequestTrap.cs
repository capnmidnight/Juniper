using System.Net;
using System.Threading.Tasks;

namespace Juniper.HTTP.Server.Controllers
{
    public class UnhandledRequestTrap : AbstractResponse
    {
        public UnhandledRequestTrap()
            : base(int.MaxValue, HttpProtocols.All, HttpMethods.All, AnyAuth, MediaType.All)
        { }

        public override Task InvokeAsync(HttpListenerContext context)
        {
            if (context is null)
            {
                throw new System.ArgumentNullException(nameof(context));
            }

            var response = context.Response;
            response.SetStatus(HttpStatusCode.NotFound);
            OnWarning(NCSALogger.FormatLogMessage(context));
            return response.SendTextAsync(MediaType.Text.Plain, "Not found");
        }
    }
}
