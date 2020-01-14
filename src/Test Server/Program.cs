using System;
using System.Globalization;
using System.Threading.Tasks;

using Juniper.HTTP.Server;
using Juniper.HTTP.Server.Controllers;

using static System.Console;
using static Juniper.AnsiColor;

namespace Juniper.HTTP
{
    public static class Program
    {
        public static async Task Main()
        {
            using var server = new HttpServer(
                new HttpToHttpsRedirect(),
                new BanHammer("testBans.txt"),
                typeof(Program),
                new StaticFileServer("content"),
                new NCSALogger("logs.txt"))
            {
                HttpPort = 8080,
                HttpsPort = 8081,
                ListenerCount = 10
            };

            server.Info += Server_Info;
            server.Warning += Server_Warning;
            server.Err += Server_Error;
            server.Log += Server_Log;
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

        private static void Socket_Message(object sender, StringEventArgs e)
        {
            var socket = (WebSocketConnection)sender;
            WriteLine($"{Green}[SOCKET]  {e.Value}{Reset}");
            var msg = e.Value + " from server";
            _ = Task.Run(() => socket.SendAsync(msg));
        }

        private static void Socket_Error(object sender, ErrorEventArgs e)
        {
            Error.WriteLine($"{Red}[SOCKET ERROR] {e.Value.Unroll()}{Reset}");
        }

        private static void Server_Log(object sender, StringEventArgs e)
        {
            WriteLine($"{Cyan}[LOG]     {e.Value}{Reset}");
        }

        private static void Server_Info(object sender, StringEventArgs e)
        {
            WriteLine($"{White}[INFO]    {e.Value}{Reset}");
        }

        private static void Server_Warning(object sender, StringEventArgs e)
        {
            WriteLine($"{Yellow}[WARNING] {e.Value}{Reset}");
        }

        private static void Server_Error(object sender, ErrorEventArgs e)
        {
            Error.WriteLine($"{Red}[ERROR]   {e.Value.Unroll()}{Reset}");
        }
    }
}
