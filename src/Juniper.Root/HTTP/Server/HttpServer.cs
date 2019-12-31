using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Juniper.HTTP.Server.Controllers;
using Juniper.Logging;

namespace Juniper.HTTP.Server
{
    /// <summary>
    /// A wrapper around <see cref="HttpListener"/> that handles
    /// routing, HTTPS, and a default start page for DEBUG running.
    /// </summary>
    public class HttpServer : IDisposable, ILoggingSource
    {
        private readonly Thread serverThread;
        private readonly HttpListener listener;
        private readonly List<Task> waiters = new List<Task>();
        private readonly List<object> controllers = new List<object>();
        private readonly List<AbstractRouteHandler> routes = new List<AbstractRouteHandler>();
        private readonly List<WebSocketConnection> sockets = new List<WebSocketConnection>();

        /// <summary>
        /// <para>
        /// Begin constructing a server.
        ///     * MaxConnections = 100.
        ///     * ListenAddress =
        ///         * DEBUG: localhost
        ///         * RELEASE: *
        /// </para>
        /// <para>
        /// Server doesn't start listening until <see cref="Start"/>
        /// is called.
        /// </para>
        /// </summary>
        public HttpServer()
        {
            ListenerCount = 100;
            EnableWebSockets = true;

#if DEBUG
            ListenAddress = "localhost";
#else
            ListenAddress = "*";
#endif

            serverThread = new Thread(Listen);

            listener = new HttpListener
            {
                AuthenticationSchemeSelectorDelegate = GetAuthenticationSchemeForRequest
            };
        }

        /// <summary>
        /// Gets or sets the maximum connections. Any connections beyond the max
        /// will wait indefinitely until a connection becomes available.
        /// </summary>
        /// <value>
        /// The maximum connections.
        /// </value>
        public int ListenerCount { get; set; }

        /// <summary>
        /// Set to false to disable handling websockets
        /// </summary>
        public bool EnableWebSockets { get; set; }

        /// <summary>
        /// When set to true, HTTP request will automatically be handled as Redirect
        /// responses that point to the HTTPS version of the URL.
        /// </summary>
        /// <seealso cref="HttpsRedirectController"/>
        public bool RedirectHttp2Https { get; set; }

        /// <summary>
        /// The IP address at which to listen for requests. By default, this is set
        /// to "*".
        /// </summary>
        /// <value>
        /// The listen address.
        /// </value>
        public string ListenAddress { get; set; }

        /// <summary>
        /// The domain name that this server serves. This is necessary to be able
        /// to automatically assign a TLS certificate to the process to handle HTTPS
        /// connections. The TLS certifcate and authority chain must be installed
        /// in the Windows Certificate Store (certmgr).
        /// </summary>
        /// <value>
        /// The domain.
        /// </value>
        public string Domain { get; set; }

        /// <summary>
        /// The port on which to listen for HTTPS connections.
        /// </summary>
        /// <value>
        /// The HTTPS port.
        /// </value>
        public ushort? HttpsPort { get; set; }

        /// <summary>
        /// Set to true if the server should attempt to run netsh to assign
        /// a certificate to the application before starting the server.
        /// </summary>
        public bool AutoAssignCertificate { get; set; }

        /// <summary>
        /// <para>The port on which to listen for insecure HTTP connections.</para>
        /// <para>
        /// WARNING: only use this in testing, or to redirect users to
        /// the HTTPS version of the request.
        /// </para>
        /// </summary>
        /// <value>
        /// The HTTP port.
        /// </value>
        public ushort? HttpPort { get; set; }

        /// <summary>
        /// Event for handling information-level logs.
        /// </summary>
        public event EventHandler<string> Info;

        /// <summary>
        /// Event for handling error logs that don't stop execution.
        /// </summary>
        public event EventHandler<string> Warning;

        /// <summary>
        /// Event for handling error logs that prevent execution.
        /// </summary>
        public event EventHandler<Exception> Error;

        private AuthenticationSchemes GetAuthenticationSchemeForRequest(HttpListenerRequest request)
        {
            return (from route in routes
                    where route.IsMatch(request)
                    select route.Authentication)
                .FirstOrDefault();
        }

        public void AddRoutesFrom<T>()
            where T : class
        {
            AddRoutesFrom<T>(null);
        }

        public void AddRoutesFrom<T>(T controller)
            where T : class
        {
            var flags = BindingFlags.Public | BindingFlags.Static;
            if (controller is object)
            {
                flags |= BindingFlags.Instance;
                AddController(controller);
            }

            var type = typeof(T);
            if (controller is Type t)
            {
                type = t;
                controller = null;
            }

            foreach (var method in type.GetMethods(flags))
            {
                var route = method.GetCustomAttribute<RouteAttribute>();
                if (route != null)
                {
                    var parameters = method.GetParameters();
                    if (method.ReturnType == typeof(Task)
                        && parameters.Length > 0
                        && parameters.Length == route.ParameterCount
                        && parameters.Skip(1).All(p => p.ParameterType == typeof(string)))
                    {
                        AbstractRouteHandler handler = null;
                        var name = $"{type.Name}::{method.Name}";
                        var contextParamType = parameters[0].ParameterType;
                        var isHttp = contextParamType == typeof(HttpListenerContext);
                        var isWebSocket = contextParamType == typeof(WebSocketConnection);

                        T source = null;
                        if (!method.IsStatic)
                        {
                            source = controller;
                        }

                        if (!isHttp && !isWebSocket)
                        {
                            OnError(this, new InvalidOperationException($@"Method {type.Name}::{method.Name} must have a signature:
    (System.Net.HttpListenerContext, string...) => Task
or
    (Juniper.HTTP.WebSocketConnection, string...) => Task"));
                        }
                        else if (isHttp)
                        {
                            handler = new HttpRouteHandler(name, source, method, route);
                        }
                        else if (isWebSocket)
                        {
                            var wsHandler = new WebSocketRouteHandler(name, source, method, route);
                            wsHandler.SocketConnected += WsHandler_SocketConnected;
                            handler = wsHandler;
                        }

                        if (handler != null)
                        {
                            AddController(handler);
                        }
                    }
                }
            }
        }

        private void AddController<T>(T controller) where T : class
        {
            if (controller is AbstractRouteHandler handler)
            {
                OnInfo(this, $"Found controller {handler}");
                routes.Add(handler);
            }

            if (controller is IInfoSource infoSource)
            {
                infoSource.Info += OnInfo;
            }

            if (controller is IWarningSource warningSource)
            {
                warningSource.Warning += OnWarning;
            }

            if (controller is IErrorSource errorSource)
            {
                errorSource.Error += OnError;
            }

            controllers.Add(controller);
        }

        public T GetController<T>()
            where T : class
        {
            return (T)controllers
                .Find(c => c is T);
        }

        private void WsHandler_SocketConnected(WebSocketConnection socket)
        {
            socket.Closed += Socket_Closed;
            sockets.Add(socket);
        }

        private void Socket_Closed(object sender, EventArgs e)
        {
            if (sender is WebSocketConnection socket)
            {
                _ = sockets.Remove(socket);
                socket.Closed -= Socket_Closed;
                socket.Dispose();
            }
        }

        public bool IsRunning
        {
            get
            {
                return listener.IsListening
                    && serverThread.IsAlive;
            }
        }

        /// <summary>
        /// Stop server and dispose all functions.
        /// </summary>
        public void Stop()
        {
            OnInfo(this, "Stopping server");
            listener.Stop();
            listener.Close();
            serverThread.Abort();
        }

        public virtual void Start()
        {
            if (HttpsPort != null)
            {
                if (!AutoAssignCertificate)
                {
                    SetPrefix("https", HttpsPort.Value);
                }
                else if (string.IsNullOrEmpty(Domain))
                {
                    OnWarning(this, "No domain was specified. Can't auto-assign a TLS certificate.");
                }
                else
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                    GetTLSParameters(out var guid, out var certHash);

                    if (string.IsNullOrEmpty(guid))
                    {
                        OnWarning(this, "Couldn't find application GUID");
                    }
                    else if (string.IsNullOrEmpty(certHash))
                    {
                        OnWarning(this, "No TLS cert found!");
                    }
                    else
                    {
                        var message = AssignCertToApp(certHash, guid);

                        if (message.Equals("SSL Certificate added successfully", StringComparison.OrdinalIgnoreCase)
                            || message.StartsWith("SSL Certificate add failed, Error: 183", StringComparison.OrdinalIgnoreCase))
                        {
                            SetPrefix("https", HttpsPort.Value);
                        }
                        else if (message.Equals("The parameter is incorrect.", StringComparison.OrdinalIgnoreCase))
                        {
                            OnWarning(this, $@"Couldn't configure the certificate correctly:
    Application GUID: {guid}
    TLS cert: {certHash}
    {message}");
                        }
                    }
                }
            }

            if (HttpPort != null
                || RedirectHttp2Https)
            {
                if (RedirectHttp2Https)
                {
                    if (HttpPort == null
                        && HttpsPort != null)
                    {
                        if (HttpsPort == 443)
                        {
                            HttpPort = 80;
                        }
                        else if (HttpsPort == 0)
                        {
                            HttpPort = 1;
                        }
                        else
                        {
                            HttpPort = (ushort)(HttpsPort - 1);
                        }
                    }

                    AddRoutesFrom<HttpsRedirectController>();
                }

                SetPrefix("http", HttpPort.Value);
            }

            if (HttpPort == null
                && HttpsPort == null)
            {
                OnError(this, new InvalidOperationException("No HTTP or HTTPS port specified."));
            }
            else
            {
#if !DEBUG
                if (HttpPort != null
                    && routes.Any(route =>
                        !(route is IPBanController)
                        && !(route is HttpsRedirectController)
                        && route.Protocol.HasFlag(HttpProtocols.HTTP)))
                {
                    OnWarning(this, "Maybe don't run unencrypted HTTP in production, k?");
                }
#endif

                routes.Sort();

                if (!listener.IsListening)
                {
                    var prefixes = string.Join(", ", listener.Prefixes);
                    OnInfo(this, $"Listening on: {prefixes}");
                    listener.Start();
                }

                if (!serverThread.IsAlive)
                {
                    serverThread.Start();
                }
            }
        }

#if DEBUG
        public Process StartBrowser(string startPage = null)
        {
            if (HttpsPort == null
                && HttpPort == null)
            {
                throw new InvalidOperationException("Server is not listening on any ports");
            }

            startPage = startPage ?? string.Empty;

            var protocol = "http";
            var port = "";

            if (HttpsPort != null)
            {
                protocol = "https";
                if (HttpsPort != 443)
                {
                    port = HttpsPort.Value.ToString(CultureInfo.InvariantCulture);
                }
            }
            else if (HttpPort != null
                && HttpPort != 80)
            {
                port = HttpPort.Value.ToString(CultureInfo.InvariantCulture);
            }

            if (port.Length > 0)
            {
                port = ":" + port;
            }

            var page = $"{protocol}://{ListenAddress}{port}/{startPage}";

            return Process.Start(new ProcessStartInfo($"explorer", $"\"{page}\"")
            {
                UseShellExecute = true,
                WindowStyle = ProcessWindowStyle.Maximized
            });
        }
#endif

        private void SetPrefix(string protocol, ushort port)
        {
            if (!string.IsNullOrEmpty(protocol)
                && port > 0)
            {
                listener.Prefixes.Add($"{protocol}://{ListenAddress}:{port.ToString(CultureInfo.InvariantCulture)}/");
            }
        }

        private void GetTLSParameters(out string guid, out string certHash)
        {
            var asm = Assembly.GetExecutingAssembly();
            guid = Marshal.GetTypeLibGuidForAssembly(asm).ToString();
            certHash = null;
            using (var store = new X509Store(StoreLocation.LocalMachine))
            {
                store.Open(OpenFlags.ReadOnly);

                certHash = (from cert in store.Certificates.Cast<X509Certificate2>()
                            where cert.Subject == "CN=" + Domain
                              && DateTime.TryParse(cert.GetEffectiveDateString(), out var effectiveDate)
                              && DateTime.TryParse(cert.GetExpirationDateString(), out var expirationDate)
                              && effectiveDate <= DateTime.Now
                              && DateTime.Now < expirationDate
                            select cert.Thumbprint)
                           .FirstOrDefault();
            }
        }

        private string AssignCertToApp(string certHash, string appGuid)
        {
            var listenAddress = ListenAddress;
            if (listenAddress == "*")
            {
                listenAddress = "0.0.0.0";
            }

            var procInfo = new ProcessStartInfo("netsh", $"http add sslcert ipport={listenAddress}:{HttpsPort.Value.ToString(CultureInfo.InvariantCulture)} certhash={certHash} appid={{{appGuid}}}")
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                ErrorDialog = false,
                LoadUserProfile = false,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                StandardErrorEncoding = Encoding.UTF8,
                StandardOutputEncoding = Encoding.UTF8,
                WindowStyle = ProcessWindowStyle.Hidden
            };

            var proc = Process.Start(procInfo);
            var message = proc.StandardOutput.ReadToEnd().Trim();
            proc.WaitForExit();
            return message;
        }

        private void Listen()
        {
            while (listener.IsListening)
            {
                try
                {
                    for (var i = waiters.Count - 1; i >= 0; --i)
                    {
                        var waiter = waiters[i];
                        if (!waiter.IsRunning())
                        {
                            waiters.RemoveAt(i);
                        }
                    }

                    while (waiters.Count < ListenerCount)
                    {
                        waiters.Add(HandleConnectionAsync());
                    }
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch (Exception exp)
                {
                    OnError(this, exp);
                }
#pragma warning restore CA1031 // Do not catch general exception types
            }
        }

        private async Task HandleConnectionAsync()
        {
            var context = await listener.GetContextAsync()
                .ConfigureAwait(false);
            var requestID = $"{{{DateTime.Now.ToShortTimeString()}}} {context.Request.UrlReferrer} [{context.Request.HttpMethod}] {context.Request.Url.PathAndQuery} => {context.Request.RemoteEndPoint}";
            try
            {
                OnInfo(this, requestID);

                var handled = false;

                foreach (var route in routes)
                {
                    if (route.IsMatch(context.Request))
                    {
                        await route.InvokeAsync(context)
                            .ConfigureAwait(false);

                        handled = true;
                        if (!route.CanContinue(context.Request))
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

                await context
                    .Response
                    .OutputStream
                    .FlushAsync()
                    .ConfigureAwait(true);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception exp)
            {
                OnError(this, exp);
                context.Response.Error(HttpStatusCode.InternalServerError, "Internal error");
            }
#pragma warning restore CA1031 // Do not catch general exception types
            finally
            {
                context.Response.StatusDescription = HttpStatusDescription.Get(context.Response.StatusCode);

                if (!context.Request.IsWebSocketRequest)
                {
                    context.Response.Close();
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void OnInfo(object source, string message)
        {
            Info?.Invoke(source, message);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void OnWarning(object sender, string message)
        {
            Warning?.Invoke(this, message);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void OnError(object sender, Exception exp)
        {
            Error?.Invoke(sender, exp);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (IsRunning)
                    {
                        Stop();
                    }

                    using (listener) { }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~HttpServer()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}