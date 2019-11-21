using System;
using System.Collections.Generic;
using System.IO;
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

        /// <summary>
        /// Construct server with given port.
        /// </summary>
        /// <param name="path">Directory path to serve.</param>
        /// <param name="httpPort">Insecure port of the server.</param>
        /// <param name="httpsPort">Secure port of the server.</param>
        public HttpServer(string path, int httpPort, int httpsPort)
        {
            serverThread = new Thread(Listen);

            listener = new HttpListener
            {
                AuthenticationSchemeSelectorDelegate = GetAuthenticationSchemeForRequest
            };

            if (httpPort > -1)
            {
                listener.Prefixes.Add($"http://*:{httpPort}/");
            }

            if (httpsPort > -1)
            {
                listener.Prefixes.Add($"https://*:{httpsPort}/");
            }

            if (!string.IsNullOrEmpty(path)
                && Directory.Exists(path))
            {
                var defaultFileHandler = new DefaultFileController(path);
                defaultFileHandler.Warning += OnWarning;
                AddRoutesFrom(defaultFileHandler);
            }
        }

        public HttpServer(string path, int httpsPort)
            : this(path, -1, httpsPort)
        { }

        public HttpServer(int httpPort, int httpsPort)
            : this(null, httpPort, httpsPort)
        { }

        public HttpServer(int httpsPort)
            : this(null, -1, httpsPort)
        { }

        public event EventHandler<string> Info;
        private void OnInfo(string message)
        {
            Info?.Invoke(this, message);
        }

        public event EventHandler<string> Warning;
        private void OnWarning(object sender, string message)
        {
            Warning?.Invoke(this, message);
        }

        public event EventHandler<string> Error;
        private void OnError(string message)
        {
            Error?.Invoke(this, message);
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
                        OnInfo($"Found controller {type.Name}::{method.Name} > {route.Priority.ToString()}.");
                        route.source = controller;
                        route.method = method;
                        routes.Add(route);
                    }
                    else
                    {
                        OnError($"Method {type.Name}::{method.Name} must signature (System.Net.HttpListenerContext, string[]) => void.");
                    }
                }
            }

            routes.Sort();
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
                    OnError($"ERRROR: {exp.Message}");
                }
#pragma warning restore CA1031 // Do not catch general exception types
            }
        }

        private async void Process(HttpListenerContext context)
        {
            var requestID = $"[{context.Request.HttpMethod}] {context.Request.Url.PathAndQuery}";
            using (context.Response.OutputStream)
            using (context.Request.InputStream)
            {
                try
                {
                    OnInfo($"Serving request {requestID}");

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
                        var message = $"Not found: {requestID}";
                        OnWarning(this, message);
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