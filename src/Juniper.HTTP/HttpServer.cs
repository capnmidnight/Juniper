using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Juniper.HTTP
{
    public class HttpServer
    {
        private readonly Thread serverThread;
        private readonly HttpListener listener;
        private readonly List<RouteAttribute> routes = new List<RouteAttribute>();

        private readonly Action<string> error;
        private readonly Action<string> info;
        private readonly Action<string> warning;

        /// <summary>
        /// Construct server with given port.
        /// </summary>
        /// <param name="path">Directory path to serve.</param>
        /// <param name="httpPort">Port of the server.</param>
        public HttpServer(int httpPort, int httpsPort, Action<string> info, Action<string> warning, Action<string> error, params object[] controllers)
        {
            this.info = info;
            this.warning = warning;
            this.error = error;

            AddRoutesFrom(controllers);

            listener = new HttpListener();
            listener.Prefixes.Add($"http://*:{httpPort}/");
            listener.Prefixes.Add($"https://*:{httpsPort}/");
            serverThread = new Thread(Listen);
        }

        public HttpServer(string path, int httpPort, int httpsPort, Action<string> info, Action<string> warning, Action<string> error, params object[] controllers)
            : this(httpPort, httpsPort, info, warning, error, controllers)
        {
            AddRoutesFrom(new DefaultFileController(path, warning));
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

        public void Start()
        {
            if (!listener.IsListening)
            {
                listener.Start();
            }

            if (!serverThread.IsAlive)
            {
                serverThread.Start();
            }
        }

        /// <summary>
        /// Stop server and dispose all functions.
        /// </summary>
        public void Stop()
        {
            listener.Stop();
            serverThread.Abort();
        }

        private void Listen()
        {
            while (listener.IsListening)
            {
                try
                {
                    Process(listener.GetContextAsync());
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch (Exception exp)
                {
                    error($"ERRROR: {exp.Message}");
                }
#pragma warning restore CA1031 // Do not catch general exception types
            }
        }

        private void Process(Task<HttpListenerContext> contextTask)
        {
            Task.Run(async () =>
            {
                var context = await contextTask;
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
                try
                {
                    using (response.OutputStream)
                    {
                        if (!handled)
                        {
                            var message = $"Not found: {context.Request.Url.PathAndQuery}";
                            warning(message);
                            response.Error(HttpStatusCode.NotFound, message);
                        }

                        response.OutputStream.Flush();
                    }
                }
                finally
                {
                    response.Close();
                }
            });
        }
    }
}