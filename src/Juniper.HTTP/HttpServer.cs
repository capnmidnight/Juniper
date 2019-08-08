using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;

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
        public HttpServer(int port, Action<string> info, Action<string> warning, Action<string> error, params object[] controllers)
        {
            this.info = info;
            this.warning = warning;
            this.error = error;

            Port = port;

            AddRoutesFrom(controllers);

            listener = new HttpListener();
            listener.Prefixes.Add(string.Format("http://*:{0}/", Port));
            listener.Start();
            serverThread = new Thread(Listen);
            serverThread.Start();
        }

        public HttpServer(string path, int port, Action<string> info, Action<string> warning, Action<string> error, params object[] controllers)
            : this(port, info, warning, error, controllers)
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

            Done = false;
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
#pragma warning disable CA1031 // Do not catch general exception types
                catch (Exception exp)
                {
                    error($"ERRROR: {exp.Message}");
                }
#pragma warning restore CA1031 // Do not catch general exception types
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
    }
}