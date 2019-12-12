using System.Collections.Generic;
using System.Threading.Tasks;

using static System.Console;

namespace Juniper.HTTP
{
    public class Program
    {
        private static readonly List<WebSocketConnection> sockets = new List<WebSocketConnection>();

        public static void Main()
        {
            var server = new HttpServer
            {
                HttpPort = 8080,
                ListenerCount = 10
            };

            server.Info += Server_Info;
            server.Warning += Server_Warning;
            server.Error += Server_Error;

            server.AddRoutesFrom(new DefaultFileController("content"));
            server.AddRoutesFrom<Program>();

            server.Start();

#if DEBUG
            server.StartBrowser("index.html");
#endif
        }

        [Route("connect/")]
        public static Task AcceptWebSocket(WebSocketConnection socket)
        {
            sockets.Add(socket);
            socket.Message += Socket_Message;
            socket.Error += Socket_Error;
            WriteLine("Got socket");
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
            socket.Send(msg);
        }

        private static void Server_Info(object sender, string e)
        {
            WriteLine(e);
        }

        private static void Server_Warning(object sender, string e)
        {
            WriteLine($"[WARNING] {e}");
        }

        private static void Server_Error(object sender, string e)
        {
            Error.WriteLine(e);
        }
    }
}
