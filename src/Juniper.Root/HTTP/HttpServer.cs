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

        private static void NoLog(string _) { }

        /// <summary>
        /// Construct server with given port.
        /// </summary>
        /// <param name="path">Directory path to serve.</param>
        /// <param name="httpPort">Port of the server.</param>
        public HttpServer(
            int httpPort,
            int httpsPort,
            Action<string> info = null,
            Action<string> warning = null,
            Action<string> error = null)
        {
            this.info = info ?? NoLog;
            this.warning = warning ?? NoLog;
            this.error = error ?? NoLog;

            listener = new HttpListener();
            listener.Prefixes.Add($"http://*:{httpPort.ToString()}/");
            listener.Prefixes.Add($"https://*:{httpsPort.ToString()}/");
            listener.AuthenticationSchemeSelectorDelegate
                = GetAuthenticationSchemeForRequest;
            serverThread = new Thread(Listen);
        }

        public HttpServer(string path,
            int httpPort,
            int httpsPort,
            Action<string> info = null,
            Action<string> warning = null,
            Action<string> error = null)
            : this(httpPort, httpsPort, info, warning, error)
        {
            AddRoutesFrom(new DefaultFileController(path, warning));
        }

        private AuthenticationSchemes GetAuthenticationSchemeForRequest(HttpListenerRequest request)
        {
            foreach (var route in routes)
            {
                if (route.IsMatch(request))
                {
                    return route.Authentication;
                }
            }

            return AuthenticationSchemes.None;
        }

        public void AddRoutesFrom(object controller)
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
                        && method.ReturnType == typeof(Task))
                    {
                        info($"Found controller {type.Name}::{method.Name} > {route.Priority.ToString()}.");
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
            listener.Close();
            serverThread.Abort();
        }

        private void Listen()
        {
            while (listener.IsListening)
            {
                try
                {
                    Process(listener.GetContext());
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch (Exception exp)
                {
                    error($"ERRROR: {exp.Message}");
                }
#pragma warning restore CA1031 // Do not catch general exception types
            }
        }

        private async void Process(HttpListenerContext context)
        {
            using (context.Response.OutputStream)
            using (context.Request.InputStream)
            {
                try
                {
                    info($"Serving request {context.Request.Url.PathAndQuery}");

                    var handled = false;
                    foreach (var route in routes)
                    {
                        if (route.IsMatch(context.Request))
                        {
                            await route.Invoke(context);
                            handled = true;
                            if (!route.Continue)
                            {
                                break;
                            }
                        }
                    }

                    if (!handled)
                    {
                        var message = $"Not found: {context.Request.Url.PathAndQuery}";
                        warning(message);
                        context.Response.Error(HttpStatusCode.NotFound, message);
                    }

                    context.Response.OutputStream.Flush();
                }
                catch
                {
                    context.Response.Error(HttpStatusCode.InternalServerError, "Internal error");
                }
                finally
                {
                    context.Response.Close();
                }
            }
        }
    }
}