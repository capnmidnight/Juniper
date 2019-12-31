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
            server.Error += Server_Error;

            server.AddRoutesFrom(new DefaultFileController("content"));
            server.AddRoutesFrom(new IPBanController("testBans.txt"));
            server.AddRoutesFrom(typeof(Program));

            server.Start();

#if DEBUG
            server.StartBrowser("index.html");
#endif

            while (server.IsRunning)
            {
                await Task.Yield();
            }
        }

        [Route("connect/")]
        public static Task AcceptWebSocketAsync(WebSocketConnection socket)
        {
            if (socket != null)
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

        private static void Socket_Error(object sender, System.Exception e)
        {
            Error.WriteLine($"[SOCKET ERROR] {e}");
        }

        private static void Socket_Message(object sender, string msg)
        {
            var socket = (WebSocketConnection)sender;
            WriteLine($"[SOCKET] {msg}");
            msg += " from server";
            Task.Run(() => socket.SendAsync(msg));
        }

        private static void Server_Info(object sender, string e)
        {
            WriteLine(e);
        }

        private static void Server_Warning(object sender, string e)
        {
            WriteLine($"[WARNING] {e}");
        }

        private static void Server_Error(object sender, Exception e)
        {
            Error.WriteLine(e.Unroll());
        }
    }
}
