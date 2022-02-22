using System.Net;

namespace Juniper.Services
{
    public static class IPBanListMiddlewareExt
    {
        public static IApplicationBuilder UseIPBanList(this IApplicationBuilder app, string bannedIPs)
        {
            return app.UseMiddleware<IPBanListMiddleware>(bannedIPs);
        }
    }

    public class IPBanListMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IPAddress[] bannedIPs;

        public IPBanListMiddleware(RequestDelegate next, string bannedIPs)
        {
            this.next = next;
            this.bannedIPs = bannedIPs.SplitX(';')
                .Select(IPAddress.Parse)
                .ToArray();
        }

        public async Task Invoke(HttpContext context)
        {
            var remoteIp = context.Connection.RemoteIpAddress;
            if (remoteIp is null
                || bannedIPs.Any(remoteIp.Equals))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return;
            }

            await next.Invoke(context);
        }
    }
}
