using System;
using System.Globalization;
using System.Threading.Tasks;

using Juniper.HTTP.Server;
using Juniper.HTTP.Server.Controllers;

using static System.Console;

namespace Juniper.HTTP
{
    public static class Program
    {
        public static async Task Main()
        {
            using var server = new HttpServer
            {
                HttpPort = 8080,
                ListenerCount = 10
            };

            server.Info += Server_Info;
            server.Warning += Server_Warning;
            server.Err += Server_Error;

            server.Add(
                new HttpToHttpsRedirect(),
                typeof(Program),
                new StaticFileServer("..\\..\\..\\content"),
                new BanHammer("testBans.txt"),
                new NCSALogger("logs.txt"));

            server.Start();

#if DEBUG
            using var browserProc = server.StartBrowser("index.html");
#endif

            while (server.IsRunning)
            {
                await Task.Yield();
            }
        }

        [Route("connect/")]
        public static Task AcceptWebSocketAsync(WebSocketConnection socket)
        {
            if (socket is object)
            {
                socket.Message += Socket_Message;
                socket.Error += Socket_Error;
                var code = socket
                    .GetHashCode()
                    .ToString(CultureInfo.InvariantCulture);
                WriteLine($"Got socket {code}");
            }

            return Task.CompletedTask;
        }

        private static void Socket_Error(object sender, ErrorEventArgs e)
        {
            Error.WriteLine($"[SOCKET ERROR] {e.Value.Unroll()}");
        }

        private static void Socket_Message(object sender, StringEventArgs e)
        {
            var socket = (WebSocketConnection)sender;
            WriteLine($"[SOCKET] {e.Value}");
            var msg = e.Value + " from server";
            _ = Task.Run(() => socket.SendAsync(msg));
        }

        private static void Server_Info(object sender, StringEventArgs e)
        {
            WriteLine(e.Value);
        }

        private static void Server_Warning(object sender, StringEventArgs e)
        {
            WriteLine($"[WARNING] {e.Value}");
        }

        private static void Server_Error(object sender, ErrorEventArgs e)
        {
            Error.WriteLine(e.Value.Unroll());
        }
    }
}
