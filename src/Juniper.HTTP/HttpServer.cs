using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;

namespace Juniper.HTTP
{
    public class HttpServer
    {
        private static readonly string[] INDEX_FILES = {
            "index.html",
            "index.htm",
            "default.html",
            "default.htm"
        };

        private readonly string rootDirectory;

        private readonly Thread serverThread;
        private readonly HttpListener listener;
        private readonly List<RouteAttribute> routes = new List<RouteAttribute>();

        private readonly Action<string> error;
        private readonly Action<string> info;
        private readonly Action<string> warning;

        public int Port
        {
            get;
            private set;
        }

        public bool Done
        {
            get;
            private set;
        }

        /// <summary>
        /// Construct server with given port.
        /// </summary>
        /// <param name="path">Directory path to serve.</param>
        /// <param name="port">Port of the server.</param>
        public HttpServer(string path, int port, Action<string> info, Action<string> warning, Action<string> error, params object[] controllers)
        {
            this.info = info;
            this.warning = warning;
            this.error = error;

            rootDirectory = path;
            Port = port;

            AddRoutesFrom(controllers);
            AddRoutesFrom(this);

            listener = new HttpListener();
            listener.Prefixes.Add(string.Format("http://*:{0}/", Port));
            listener.Start();
            serverThread = new Thread(Listen);
            serverThread.Start();
        }

        private void AddRoutesFrom(params object[] controllers)
        {
            foreach (var controller in controllers)
            {
                var type = controller.GetType();
                var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public);
                foreach (var method in methods)
                {
                    var route = method.GetCustomAttribute<RouteAttribute>();
                    if (route != null)
                    {
                        var parameters = method.GetParameters();
                        if (parameters.Length == route.parameterCount
                            && parameters[0].ParameterType == typeof(HttpListenerContext)
                            && parameters.Skip(1).All(p => p.ParameterType == typeof(string))
                            && method.ReturnType == typeof(void))
                        {
                            info($"Found controller {type.Name}::{method.Name} > {route.Priority}.");
                            route.source = controller;
                            route.method = method;
                            routes.Add(route);
                        }
                        else
                        {
                            error($"Method {type.Name}::{method.Name} must signature (System.Net.HttpListenerContext, string[]) => void.");
                        }
                    }
                }
            }

            routes.Sort((a, b) => a.Priority - b.Priority);
        }

        /// <summary>
        /// Stop server and dispose all functions.
        /// </summary>
        public void Stop()
        {
            serverThread.Abort();
            listener.Stop();
            Done = true;
        }

        private void Listen()
        {
            while (!Done)
            {
                try
                {
                    var context = listener.GetContext();
                    Process(context);
                }
                catch
                {
                }
            }
        }

        [Route(".*", Priority = int.MaxValue)]
        public void ServeFile(HttpListenerContext context)
        {
            var request = context.Request;
            var response = context.Response;
            var requestPath = request.Url.AbsolutePath;
            var requestFile = MassageRequestPath(requestPath);
            var filename = Path.Combine(rootDirectory, requestFile);
            var isDirectory = Directory.Exists(filename);

            if (isDirectory)
            {
                filename = FindDefaultFile(filename);
            }

            var file = new FileInfo(filename);
            var shortName = MakeShortName(rootDirectory, filename);

            if (isDirectory && requestPath[requestPath.Length - 1] != '/')
            {
                response.Redirect(requestPath + "/");
            }
            else if (file.Exists)
            {
                try
                {
                    response.SendFile(file);
                }
                catch (Exception exp)
                {
                    var message = $"ERRRRRROR: '{shortName}' > {exp.Message}";
                    warning(message);
                    response.Error(HttpStatusCode.InternalServerError, message);
                }
            }
            else
            {
                var message = $"request '{shortName}'";
                warning(message);
                response.Error(HttpStatusCode.NotFound, message);
            }
        }

        private void Process(HttpListenerContext context)
        {
            var handled = false;
            info?.Invoke($"Serving request {context.Request.Url.PathAndQuery}");
            foreach (var route in routes)
            {
                var args = route.GetParams(context);
                if (args != null)
                {
                    route.Invoke(context, args);
                    handled = true;
                    if (!route.Continue)
                    {
                        break;
                    }
                }
            }

            var response = context.Response;

            if (!handled)
            {
                var message = $"Not found: {context.Request.Url.PathAndQuery}";
                warning(message);
                response.Error(HttpStatusCode.NotFound, message);
            }

            response.OutputStream.Flush();
            response.OutputStream.Close();
            response.OutputStream.Dispose();
            response.Close();
        }

        private static string MakeShortName(string rootDirectory, string filename)
        {
            var shortName = filename.Replace(rootDirectory, "");
            if (shortName.Length > 0 && shortName[0] == Path.DirectorySeparatorChar)
            {
                shortName = shortName.Substring(1);
            }

            return shortName;
        }

        private static string FindDefaultFile(string filename)
        {
            if (Directory.Exists(filename))
            {
                for (var i = 0; i < INDEX_FILES.Length; ++i)
                {
                    var test = Path.Combine(filename, INDEX_FILES[i]);
                    if (File.Exists(test))
                    {
                        filename = test;
                        break;
                    }
                }
            }

            return filename;
        }

        private static string MassageRequestPath(string requestPath)
        {
            requestPath = requestPath.Substring(1);

            if (requestPath.Length > 0 && requestPath[requestPath.Length - 1] == '/')
            {
                requestPath = requestPath.Substring(0, requestPath.Length - 1);
            }

            requestPath = requestPath.Replace('/', Path.DirectorySeparatorChar);
            return requestPath;
        }
    }
}