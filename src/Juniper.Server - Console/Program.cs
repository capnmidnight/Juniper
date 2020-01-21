using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using Juniper.Console;
using Juniper.HTTP;
using Juniper.HTTP.Server;
using Juniper.HTTP.Server.Controllers;
using Juniper.Processes;

using static System.Console;
using static Juniper.AnsiColor;

namespace Juniper
{
    public static class Program
    {
        private static HttpServer server;

        private static readonly ConsoleCommandProcessor cons = new ConsoleCommandProcessor
        {
            (ConsoleKey.UpArrow, "increase log level", () => cons.SetLogLevel(1)),
            (ConsoleKey.DownArrow, "decrease log level", () => cons.SetLogLevel(-1)),
            (ConsoleKey.O, "open root page in browser", () => server.StartBrowser()),
            (ConsoleKey.Q, "quit server", () => server.Stop())
        };

        private static async Task Main(string[] args)
        {
            cons.OnInfo("Starting server");

            // Read options
            var options = new Dictionary<string, string>();

#if DEBUG
            //Set default options
            options.SetValues(
                ("path", Path.Combine("..", "..", "..", "content")),
                ("domain", "localhost"),
                ("http", HttpServer.IsAdministrator ? "80" : "8080"),
                ("https", HttpServer.IsAdministrator ? "443" : "8081"));
#endif

            options.SetValues(args);

            using var s = server = new HttpServer
            {
                ListenerCount = 10,
                AutoAssignCertificate = HttpServer.IsAdministrator
            };

            server.Add(cons);

            if (server.SetOptions(options))
            {
                server.Add(typeof(Program));
                server.Start();
                cons.PrintUsage();

                while (server.IsRunning)
                {
                    await Task.Yield();
                    cons.Pump();
                }
            }
        }

        [Route("auth/", Methods = HttpMethods.POST, Authentication = AuthenticationSchemes.Basic)]
        public static async Task AuthenticateUserAsync(HttpListenerContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var response = context.Response;

            if (context.User.Identity is HttpListenerBasicIdentity user
                && user.Name == "sean"
                && user.Password == "ppyptky7")
            {
                var token = Guid.NewGuid().ToString();
                WebSocketPool.SetUserToken(user.Name, token);
                response.SetStatus(HttpStatusCode.OK);
                await response.SendTextAsync(token)
                    .ConfigureAwait(false);
            }
            else
            {
                response.SetStatus(HttpStatusCode.Unauthorized);
            }
        }

        [Route("connect/")]
        public static Task AcceptWebSocketAsync(ServerWebSocketConnection socket)
        {
            if (socket is object)
            {
                socket.Closing += Socket_Closing;
                socket.Message += Socket_Message;
                socket.Error += Socket_Error;
                var code = socket
                    .GetHashCode()
                    .ToString(CultureInfo.InvariantCulture);
                WriteLine($"Got socket {code}");
            }

            return Task.CompletedTask;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Socket_Message(object sender, StringEventArgs e)
        {
            var socket = (ServerWebSocketConnection)sender;
            cons.OnInfo(Green, e.Value);
            var msg = e.Value + " from server";
            _ = Task.Run(() => socket.SendAsync(msg));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Socket_Error(object sender, ErrorEventArgs e)
        {
            cons.OnError(e);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Socket_Closing(object sender, EventArgs e)
        {
            var socket = (ServerWebSocketConnection)sender;
            socket.Error -= Socket_Error;
            socket.Message -= Socket_Message;
        }
    }
}
