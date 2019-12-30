using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Juniper.HTTP
{
    public sealed class IPBanController : AbstractRouteHandler
    {
        private readonly CIDRBlock[] blocks;

        public IPBanController(IEnumerable<CIDRBlock> blocks)
            : base(null, int.MinValue, HttpProtocols.All, HttpMethods.All)
        {
            this.blocks = blocks.ToArray();
        }

        public override bool IsMatch(HttpListenerRequest request)
        {
            return blocks.Any(block =>
                block.Contains(request.RemoteEndPoint.Address));
        }

        public override Task InvokeAsync(HttpListenerContext context)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return Task.CompletedTask;
        }
    }
}
