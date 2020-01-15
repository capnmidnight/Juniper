using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using Juniper.HTTP.Server.Controllers;
using Juniper.Logging;

using System.Text;

#if !NETCOREAPP && !NETSTANDARD
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

using Juniper.HTTP.Server.Administration.NetSH;
#endif

namespace Juniper.HTTP.Server
{
    /// <summary>
    /// A wrapper around <see cref="HttpListener"/> that handles
    /// routing, HTTPS, and a default start page for DEBUG running.
    /// </summary>
    public class HttpServer :
        IDisposable,
        ILoggingSource,
        INCSALogSource
    {

        public static bool IsAdministrator
        {
            get
            {
#if !NETCOREAPP && !NETSTANDARD
                var identity = System.Security.Principal.WindowsIdentity.GetCurrent();
                var principal = new System.Security.Principal.WindowsPrincipal(identity);
                return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
#else
                return false;
#endif
            }
        }

        private readonly Thread serverThread;
        private readonly HttpListener listener;
        private readonly List<Task> waiters = new List<Task>();
        private readonly List<object> controllers = new List<object>();
        private readonly List<AbstractResponse> routes = new List<AbstractResponse>();

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
        public HttpServer(params object[] controllers)
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

            Add(controllers);
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
        /// Event for handling Common Log Format logs.
        /// </summary>
        public event EventHandler<StringEventArgs> Log;

        /// <summary>
        /// Event for handling information-level logs.
        /// </summary>
        public event EventHandler<StringEventArgs> Info;

        /// <summary>
        /// Event for handling error logs that don't stop execution.
        /// </summary>
        public event EventHandler<StringEventArgs> Warning;

        /// <summary>
        /// Event for handling error logs that prevent execution.
        /// </summary>
        public event EventHandler<ErrorEventArgs> Err;

        public void Add(params object[] controllers)
        {
            if (controllers is null)
            {
                throw new ArgumentNullException(nameof(controllers));
            }

            for (var i = 0; i < controllers.Length; i++)
            {
                var controller = controllers[i];

                if (controller is null)
                {
                    throw new NullReferenceException($"Encountered a null value at index {i}.");
                }

                var flags = BindingFlags.Public | BindingFlags.Static;
                var type = controller as Type ?? controller.GetType();
                if (controller is Type)
                {
                    controller = null;
                }
                else
                {
                    flags |= BindingFlags.Instance;
                    AddController(controller);
                }

                foreach (var method in type.GetMethods(flags))
                {
                    var route = method.GetCustomAttribute<RouteAttribute>();
                    if (route is object)
                    {
                        var parameters = method.GetParameters();
                        if (method.ReturnType == typeof(Task)
                            && parameters.Length > 0
                            && parameters.Length == route.ParameterCount
                            && parameters.Skip(1).All(p => p.ParameterType == typeof(string)))
                        {
                            var contextParamType = parameters[0].ParameterType;
                            var isHttp = contextParamType == typeof(HttpListenerContext);
                            var isWebSocket = contextParamType == typeof(WebSocketConnection);

                            object source = null;
                            if (!method.IsStatic)
                            {
                                source = controller;
                            }

                            if (isHttp)
                            {
                                AddController(new HttpRoute(source, method, route));
                            }
                            else if (isWebSocket)
                            {
                                if (route.Authentication != AuthenticationSchemes.Anonymous)
                                {
                                    throw new InvalidOperationException("WebSockets do not support authentication");
                                }

                                AddController(new WebSocketRoute(source, method, route));
                            }
                            else
                            {
                                throw new InvalidOperationException($@"Method {type.Name}::{method.Name} must have a signature:
    (System.Net.HttpListenerContext, string...) => Task
or
    (Juniper.HTTP.WebSocketConnection, string...) => Task");
                            }
                        }
                    }
                }
            }
        }

        private void AddController<T>(T controller) where T : class
        {
            if (controller is AbstractResponse handler)
            {
                handler.Server = this;
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
                errorSource.Err += OnError;
            }

            if (controller is INCSALogSource nCSALogSource)
            {
                nCSALogSource.Log += OnLog;
            }

            controllers.Add(controller);
        }

        public T GetController<T>()
            where T : class
        {
            return (T)controllers
                .Find(c => c is T);
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
            OnInfo("Stopping server");
            foreach (var controller in controllers)
            {
                if (controller is IInfoSource infoSource)
                {
                    infoSource.Info -= OnInfo;
                }

                if (controller is IWarningSource warningSource)
                {
                    warningSource.Warning -= OnWarning;
                }

                if (controller is IErrorSource errorSource)
                {
                    errorSource.Err -= OnError;
                }

                if (controller is INCSALogSource nCSALogSource)
                {
                    nCSALogSource.Log -= OnLog;
                }
            }

            listener.Stop();
            listener.Close();
            serverThread.Abort();
        }

        public virtual void Start()
        {
            OnInfo("Starting server");


            if (routes.Count > 0)
            {
                routes.Sort();
                ShowRoutes();
            }

            if (HttpsPort is object)
            {
                if (!AutoAssignCertificate)
                {
                    SetPrefix("https", HttpsPort.Value);
                }

#if !NETCOREAPP && !NETSTANDARD
                else if (string.IsNullOrEmpty(Domain))
                {
                    OnWarning("No domain was specified. Can't auto-assign a TLS certificate.");
                }
                else
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                    GetTLSParameters(out var guid, out var certHash);

                    if (string.IsNullOrEmpty(certHash))
                    {
                        OnWarning("No TLS cert found!");
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
                            OnWarning($@"Couldn't configure the certificate correctly:
    Application GUID: {guid}
    TLS cert: {certHash}
    {message}");
                        }
                    }
                }
#endif
            }

            if (HttpPort is object)
            {
                SetPrefix("http", HttpPort.Value);
            }

            if (HttpPort is null
                && HttpsPort is null)
            {
                OnError(new InvalidOperationException("No HTTP or HTTPS port specified."));
            }
            else
            {
#if !DEBUG
                if (HttpPort is object
                    && routes.Any(route =>
                        !(route is BanHammer)
                        && !(route is HttpToHttpsRedirect)
                        && route.Protocol.HasFlag(HttpProtocols.HTTP)))
                {
                    OnWarning("Maybe don't run unencrypted HTTP in production, k?");
                }
#endif


                if (!listener.IsListening)
                {
                    OnInfo($"Listening on:");
                    foreach (var prefix in listener.Prefixes)
                    {
                        OnInfo($"\t{prefix}");
                    }

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
            if (HttpsPort is null
                && HttpPort is null)
            {
                throw new InvalidOperationException("Server is not listening on any ports");
            }

            startPage ??= string.Empty;

            var protocol = "http";
            var port = "";

            if (HttpsPort is object)
            {
                protocol = "https";
                if (HttpsPort != 443)
                {
                    port = HttpsPort.Value.ToString(CultureInfo.InvariantCulture);
                }
            }
            else if (HttpPort is object
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

#if !NETCOREAPP && !NETSTANDARD
        private void GetTLSParameters(out Guid guid, out string certHash)
        {
            var asm = Assembly.GetExecutingAssembly();
            guid = Marshal.GetTypeLibGuidForAssembly(asm);
            certHash = null;
            using var store = new X509Store(StoreLocation.LocalMachine);
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


        private string AssignCertToApp(string certHash, Guid appGuid)
        {
            var listenAddress = ListenAddress;
            if (listenAddress == "*")
            {
                listenAddress = "0.0.0.0";
            }

            var addCert = new AddSslCert(
                listenAddress,
                HttpsPort.Value,
                certHash,
                appGuid);

            addCert.Info += OnInfo;
            addCert.Warning += OnWarning;
            addCert.Err += OnError;

            _ = addCert.Run();

            addCert.Info -= OnInfo;
            addCert.Warning -= OnWarning;
            addCert.Err -= OnError;

            return addCert.TotalStandardOutput;
        }
#endif

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
                    OnError(exp);
                }
#pragma warning restore CA1031 // Do not catch general exception types
            }
        }

        private AuthenticationSchemes GetAuthenticationSchemeForRequest(HttpListenerRequest request)
        {
            var auth = (from route in routes
                        where route.IsMatch(request)
                        orderby Math.Abs(route.Priority)
                        select route.Authentication)
                .FirstOrDefault();

            if (auth == AuthenticationSchemes.None)
            {
                auth = AuthenticationSchemes.Anonymous;
            }

            return auth;
        }

        private async Task HandleConnectionAsync()
        {
            var context = await listener.GetContextAsync()
                .ConfigureAwait(false);
            var response = context.Response;
            var request = context.Request;
            var headers = request.Headers;

#if DEBUG
            OnInfo(NCSALogger.FormatLogMessage(context));
            OnInfo("Headers:");
            foreach (var key in headers.AllKeys)
            {
                var value = headers[key];
                OnInfo($"\t{key} = {value}");
            }
#endif

            try
            {
                foreach (var route in routes)
                {
                    if (route.IsMatch(context))
                    {
                        await route.InvokeAsync(context)
                            .ConfigureAwait(false);
                    }
                }
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception exp)
            {
                OnError(exp);
                context.Response.SetStatus(HttpStatusCode.InternalServerError);
            }
#pragma warning restore CA1031 // Do not catch general exception types
            finally
            {
                response.StatusDescription = HttpStatusDescription.Get(response.StatusCode);

                await response
                    .OutputStream
                    .FlushAsync()
                    .ConfigureAwait(true);

                if (response.StatusCode >= 400
                    || (headers["Connection"] != "Keep-Alive"
                        && !request.IsWebSocketRequest))
                {
                    response.Close();
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnLog(object sender, StringEventArgs e)
        {
            Log?.Invoke(sender, e);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void OnInfo(string message)
        {
            Info?.Invoke(this, new StringEventArgs(message));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void OnInfo(object sender, StringEventArgs e)
        {
            Info?.Invoke(sender, e);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void OnWarning(string message)
        {
            Warning?.Invoke(this, new StringEventArgs(message));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void OnWarning(object sender, StringEventArgs e)
        {
            Warning?.Invoke(sender, e);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void OnError(Exception exp)
        {
            Err?.Invoke(this, new ErrorEventArgs(exp));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void OnError(object sender, ErrorEventArgs e)
        {
            Err?.Invoke(sender, e);
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

                    foreach (var controller in controllers)
                    {
                        if (controller is IDisposable disposable)
                        {
                            disposable.Dispose();
                        }
                    }

                    controllers.Clear();
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        private void ShowRoutes()
        {
            OnInfo("Found routes:");
            var table = new string[routes.Count + 1, 7];
            table[0, 0] = "Type";
            table[0, 1] = "Method";
            table[0, 2] = "Priority";
            table[0, 3] = "Protocol";
            table[0, 4] = "Verb";
            table[0, 5] = "Status";
            table[0, 6] = "Authentication";

            for (var i = 0; i < routes.Count; i++)
            {
                var route = routes[i];
                var sepIndex = route.Name.IndexOf("::", StringComparison.Ordinal);
                if (sepIndex < 0)
                {
                    table[i + 1, 0] = route.Name;
                }
                else
                {
                    table[i + 1, 0] = route.Name.Substring(0, sepIndex);
                    table[i + 1, 1] = route.Name.Substring(sepIndex + 2);
                }

                table[i + 1, 2] = route.Priority.ToString(CultureInfo.InvariantCulture);
                table[i + 1, 3] = route.Protocol.ToString();
                table[i + 1, 4] = route.Verb.ToString();
                table[i + 1, 5] = route.ExpectedStatuses.ToString();

                if (route.Authentication == AbstractResponse.AnyAuth)
                {
                    table[i + 1, 6] = "Any";
                }
                else
                {
                    table[i + 1, 6] = route.Authentication.ToString();
                }
            }

            var columnSizes = new int[table.GetLength(1)];
            for (var x = 0; x < table.GetLength(0); ++x)
            {
                for (var y = 0; y < table.GetLength(1); ++y)
                {
                    if (table[x, y] is null)
                    {
                        table[x, y] = string.Empty;
                    }
                    columnSizes[y] = Math.Max(columnSizes[y], table[x, y].Length);
                }
            }

            var totalWidth = columnSizes.Sum() + (columnSizes.Length * 3) + 1;

            var sb = new StringBuilder();
            for (var x = 0; x < table.GetLength(0); ++x)
            {
                _ = sb.Clear();
                for (var y = 0; y < table.GetLength(1); ++y)
                {
                    var formatter = $"| {{0,-{columnSizes[y]}}} ";
                    _ = sb.AppendFormat(CultureInfo.InvariantCulture, formatter, table[x, y]);
                }

                _ = sb.Append("|");
                OnInfo(sb.ToString());

                if (x == 0)
                {
                    _ = sb.Clear();
                    for (var y = 0; y < totalWidth; ++y)
                    {
                        _ = sb.Append("-");
                    }
                    OnInfo(sb.ToString());
                }
            }
        }
    }
}