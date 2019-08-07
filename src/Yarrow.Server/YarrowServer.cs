using System;

using Juniper.HTTP;

namespace Yarrow.Server
{
    public class YarrowServer
    {
        private readonly HttpServer server;

        public YarrowServer(string[] args, Action<string> info, Action<string> warning, Action<string> error)
        {
            server = HttpServerUtil.Start(
                args, info, warning, error,
                new YarrowController());
        }

        public void Stop()
        {
            server.Stop();
        }
    }
}