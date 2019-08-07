using System;
using System.Collections.Generic;
using System.IO;

namespace Juniper.HTTP
{
    public static class HttpServerUtil
    {
        private static readonly Dictionary<string, string> COMMAND_ALIASES = new Dictionary<string, string>()
        {
            {"--port", "port" },
            {"-p", "port" },
            {"/P", "port" },

            {"--directory", "path" },
            {"-d", "path" },
            {"/D", "path" },

            {"--url", "url" },
            {"-u", "url" },
            {"/U", "url" }
        };

        private static readonly Dictionary<string, KeyValuePair<string, string>> COMMAND_SHORTCUTS = new Dictionary<string, KeyValuePair<string, string>>()
        {
            {"--kiosk", new KeyValuePair<string, string>("mode", "kiosk" ) },
            {"-k", new KeyValuePair<string, string>("mode", "kiosk" ) },
            {"/K", new KeyValuePair<string, string>("mode", "kiosk" ) },

            {"--quiet", new KeyValuePair<string, string>("mode", "quiet" ) },
            {"-q", new KeyValuePair<string, string>("mode", "quiet" ) },
            {"/Q", new KeyValuePair<string, string>("mode", "quiet" ) },

            {"--help", new KeyValuePair<string, string>("help", "help") },
            {"/?", new KeyValuePair<string, string>("help", "help") }
        };

        private static readonly Dictionary<string, string> DEFAULT_ARGUMENTS = new Dictionary<string, string>()
        {
            {"port", "8000" },
            {"path", Environment.CurrentDirectory },
            {"url", "" }
        };

        public static void SetDefaults(Dictionary<string, string> args)
        {
            foreach (var pair in DEFAULT_ARGUMENTS)
            {
                args[pair.Key] = pair.Value;
            }
        }

        public static void ReadCommandLine(string[] args, Dictionary<string, string> arguments)
        {
            for (var i = 0; i < args.Length; ++i)
            {
                if ((args[i].Length > 0 && (args[i][0] == '"' || args[i][0] == '\'') && args[i][0] == args[i][args[i].Length - 1]))
                {
                    args[i] = args[i].Substring(1, args[i].Length - 2);
                }

                if (COMMAND_ALIASES.ContainsKey(args[i]))
                {
                    if (i < args.Length - 1)
                    {
                        arguments[COMMAND_ALIASES[args[i]]] = args[i + 1];
                        ++i;
                    }
                    else
                    {
                        Console.Error.WriteLine("Unknown command switch: {0}.", args[i]);
                        return;
                    }
                }
                else if (COMMAND_SHORTCUTS.ContainsKey(args[i]))
                {
                    var pair = COMMAND_SHORTCUTS[args[i]];
                    arguments[pair.Key] = pair.Value;
                }
                else if (Directory.Exists(args[i]))
                {
                    arguments["path"] = args[i];
                }
                else
                {
                    Console.Error.WriteLine("Missing value for {0} switch.", args[i]);
                    return;
                }
            }
        }

        public static HttpServer Start(string[] args, Action<string> Info, Action<string> Warning, Action<string> Error, params object[] controllers)
        {
            HttpServer server = null;
            var arguments = new Dictionary<string, string>();
            SetDefaults(arguments);
            ReadCommandLine(args, arguments);
            var path = arguments["path"];
            var portDef = arguments["port"];

            int port;
            if (!int.TryParse(portDef, out port))
            {
                Error($"Invalid Port specification. Was {portDef}, expecting an integer like 80, 81, 8080, 8383, etc.");
            }
            else if (!Directory.Exists(path))
            {
                Error($"No directory from which to serve found at {path}.");
            }
            else
            {
                Info($"Serving path '{path}'");
                for (var p = port; p < 0xffff; ++p)
                {
                    Info($"Trying port '{p}'");
                    try
                    {
                        server = new HttpServer(path, p, Info, Warning, Error, controllers);
                        port = p;
                        arguments["port"] = p.ToString();
                        Info($"Listening on port {port}");
                        break;
                    }
                    catch
                    {
                        Warning($"Port {p} is already in use. Trying another one.");
                    }
                }

                if (server == null)
                {
                    Error("Couldn't find an open port.");
                }
            }

            return server;
        }
    }
}