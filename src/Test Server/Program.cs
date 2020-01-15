using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using Juniper.HTTP.Server;
using Juniper.HTTP.Server.Controllers;

using static System.Console;
using static Juniper.AnsiColor;

namespace Juniper.HTTP
{
    public static class Program
    {
        private static HttpServer server;

        private static uint logLevel = 0;
        private static readonly Dictionary<ConsoleKey, NamedAction> actions = new Dictionary<ConsoleKey, NamedAction>
        {
            [ConsoleKey.UpArrow] = ("increase log level", () => SetLogLevel(1)),
            [ConsoleKey.DownArrow] = ("decrease log level", () => SetLogLevel(-1)),
            [ConsoleKey.Q] = ("quit server", () => server.Stop())
        };

        private static async Task Main(string[] args)
        {
            Log(0, WriteLine, Green, "Starting server");

            // Read options
            var options = new Dictionary<string, string>();
            for (var i = 0; i < args.Length - 1; ++i)
            {
                if (args[i].StartsWith("--", false, CultureInfo.InvariantCulture))
                {
                    options[args[i]] = args[++i];
                }
            }

#if DEBUG
            //Set default options
            options.Default("--path", Path.Combine("..", "..", "..", "content"));
            options.Default("--domain", "localhost");
            if (HttpServer.IsAdministrator)
            {
                options.Default("--https", "443");
                options.Default("--http", "80");
            }
            else
            {
                options.Default("--http", "8080");
            }
#endif

            using var s = server = new HttpServer(
                typeof(Program))
            {
                ListenerCount = 10,
                AutoAssignCertificate = HttpServer.IsAdministrator
            };

            server.Info += Server_Info;
            server.Warning += Server_Warning;
            server.Err += Server_Error;
            server.Log += Server_Log;

            // Parse options
            var hasHttpsPort = options.TryGetValue(
                "--https",
                out var httpsPortString);

            var hasHttpPort = options.TryGetValue(
                "--http",
                out var httpPortString);

            var hasPort = Check(
                hasHttpsPort || hasHttpPort,
                "Must specify at least one of --https or --http");

            var isValidHttpsPort = Check(
                ushort.TryParse(httpsPortString, out var httpsPort) || !hasHttpsPort,
                $"--https value `{httpsPortString}` must be an unsigned 16-bit integer");

            var isValidHttpPort = Check(
                ushort.TryParse(httpPortString, out var httpPort) || !hasHttpPort,
                $"--http value `{httpPortString}` must be an unsigned 16-bit integer");

            var hasDomain = Check(
                options.TryGetValue("--domain", out var domain),
                "No domain specified");

            var hasLogPath = Check(
                options.TryGetValue("--log", out var logPath),
                "No logging path");

            var hasContentPath = options.TryGetValue(
                "--path",
                out var contentPath);

            var isValidContentPath = Check(
                !hasContentPath || Directory.Exists(contentPath),
                "Path to static content directory does not exist");

            if (hasContentPath && !isValidContentPath)
            {
                Log(2, WriteLine, Yellow, $"Content directory was attempted from: {new DirectoryInfo(contentPath).FullName}");
            }

            if (hasDomain
                && hasPort
                && isValidHttpsPort
                && isValidHttpPort)
            {
                // Set options on server
                server.Domain = domain;

                if (isValidHttpsPort
                    && hasHttpsPort)
                {
                    server.HttpsPort = httpsPort;
                }

                if (isValidHttpPort
                    && hasHttpPort)
                {
                    server.HttpPort = httpPort;
                    server.Add(new HttpToHttpsRedirect());
                }

                if (hasLogPath)
                {
                    server.Add(new NCSALogger(logPath));
                }

                if (isValidContentPath
                    && hasContentPath)
                {
                    server.Add(new StaticFileServer(contentPath));
                }

                server.Start();

#if DEBUG
                using var browserProc = server.StartBrowser("index.html");
#endif

                while (server.IsRunning)
                {
                    await Task.Yield();

                    if (KeyAvailable)
                    {
                        var keyInfo = ReadKey(true);
                        if (actions.ContainsKey(keyInfo.Key))
                        {
                            actions[keyInfo.Key].Invoke();
                        }
                        else
                        {
                            PrintUsage();
                        }
                    }
                }

                server.Info -= Server_Info;
                server.Warning -= Server_Warning;
                server.Err -= Server_Error;
                server.Log -= Server_Log;
            }
        }

        private static bool Check(bool isValid, string message)
        {
            if (!isValid)
            {
                Log(3, Error.WriteLine, Red, new ErrorEventArgs(new ArgumentException(message)));
            }

            return isValid;
        }

        private static void SetLogLevel(int direction)
        {
            var nextLogLevel = (uint)(logLevel + direction);
            if (nextLogLevel < 4)
            {
                logLevel = nextLogLevel;
                WriteLine($"Logging level is now {GetName(logLevel)}");
            }
        }

        private static string GetName(uint logLevel)
        {
            return logLevel switch
            {
                0 => "Verbose",
                1 => "Log",
                2 => "Warning",
                3 => "Error",
                _ => "N/A"
            };
        }

        private static void PrintUsage()
        {
            WriteLine("Usage:");
            var maxKeyLen = (from key in actions.Keys
                             let keyName = key.ToString()
                             select keyName.Length)
                        .Max();

            var format = $"\t{{0,-{maxKeyLen}}} : {{1}}";
            foreach (var command in actions)
            {
                WriteLine(format, command.Key, command.Value.Name);
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Log<T>(uint level, Action<string> logger, string color, T e)
        {
            if (level >= logLevel)
            {
                logger($"{color}[{GetName(level)}] {e.ToString()}{Reset}");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Server_Info(object sender, StringEventArgs e)
        {
            Log(0, WriteLine, White, e);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Socket_Message(object sender, StringEventArgs e)
        {
            var socket = (WebSocketConnection)sender;
            Log(0, WriteLine, Green, e);
            var msg = e.Value + " from server";
            _ = Task.Run(() => socket.SendAsync(msg));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Server_Log(object sender, StringEventArgs e)
        {
            Log(1, WriteLine, Cyan, e);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Server_Warning(object sender, StringEventArgs e)
        {
            Log(2, WriteLine, Yellow, e);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Socket_Error(object sender, ErrorEventArgs e)
        {
            Log(3, Error.WriteLine, Red, e);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Server_Error(object sender, ErrorEventArgs e)
        {
            Log(3, Error.WriteLine, Red, e);
        }
    }
}
