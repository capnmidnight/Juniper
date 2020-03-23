using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Juniper.HTTP.Server.Administration.NetSH;
using Juniper.HTTP.Server.Controllers;
using Juniper.Logging;
using Juniper.Processes;

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

        private readonly Thread listenThread;
        private readonly Thread cleanupThread;
        private readonly HttpListener listener;
        private readonly List<object> controllers = new List<object>();
        private readonly List<AbstractResponse> routes = new List<AbstractResponse>();
        private readonly ConcurrentBag<Task> waiters = new ConcurrentBag<Task>();

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
        /// The port on which to listen for HTTPS connections.
        /// </summary>
        /// <value>
        /// The HTTPS port.
        /// </value>
        public ushort? HttpsPort { get; set; }

        /// <summary>
        /// Gets or sets the maximum connections. Any connections beyond the max
        /// will wait indefinitely until a connection becomes available.
        /// </summary>
        /// <value>
        /// The maximum connections.
        /// </value>
        public int ListenerCount { get; set; }

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
        /// Set to true if the server should attempt to run netsh to assign
        /// a certificate to the application before starting the server.
        /// </summary>
        public bool AutoAssignCertificate { get; set; }

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
            listenThread = new Thread(Listen);
            cleanupThread = new Thread(Cleanup);

            listener = new HttpListener
            {
                AuthenticationSchemeSelectorDelegate = GetAuthenticationSchemeForRequest
            };
        }

        public bool SetOptions(Dictionary<string, string> options)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            // Echo options
            OnInfo("Startup parameters are:");
            foreach (var param in options)
            {
                OnInfo($"\t{param.Key} = {param.Value}");
            }

            var hasHttpsPort = options.TryGetUInt16("https", out var httpsPort);

            var hasHttpPort = options.TryGetUInt16("http", out var httpPort);

            var hasPort = Check(
                hasHttpsPort || hasHttpPort,
                "Must specify at least one of --https or --http");

            var hasDomain = Check(
                options.TryGetValue("domain", out var domain),
                "No domain specified");

            var isValidAssignCert = Check(
                options.TryGetBool("assignCert", out var assignCert)
                && hasDomain
                || !options.ContainsKey("assignCert"),
                "Must provide the --domain option to auto-assign a certificate");

            var isValidListenCount = Check(
                options.TryGetInt32("listeners", out var listenerCount)
                && listenerCount > 0
                || !options.ContainsKey("listeners"),
                "--listeners value must be greater than zero.");

            if (listenerCount == 0)
            {
                listenerCount = 10;
            }

            var hasLogPath = Check(
                options.TryGetValue("log", out var logPath),
                "No logging path");

            var hasBansPath = Check(
                options.TryGetValue("bans", out var bansPath),
                "Path to ban file does not exist.");

            var hasContentPath = options.TryGetValue("path", out var contentPath);
            var isValidContentPath = Check(
                !hasContentPath || Directory.Exists(contentPath),
                "Path to static content directory does not exist");

            if (hasContentPath && !isValidContentPath)
            {
                OnWarning($"Content directory was attempted from: {new DirectoryInfo(contentPath).FullName}");
            }

            if (hasDomain && hasPort)
            {
                // Set options on server
                Domain = domain;

                if (hasHttpsPort)
                {
                    HttpsPort = httpsPort;
#if !DEBUG
                    if (hasHttpPort)
                    {
                        Add(new HttpToHttpsRedirect(httpsPort));
                    }
#endif
                }

                if (isValidAssignCert)
                {
                    AutoAssignCertificate = assignCert;
                }

                if (!AutoAssignCertificate)
                {
                    if (hasHttpPort)
                    {
                        HttpPort = httpPort;
                    }

                    if (isValidListenCount)
                    {
                        ListenerCount = listenerCount;
                    }

                    if (isValidContentPath
                        && hasContentPath)
                    {
                        Add(new StaticFileServer(contentPath));
                    }

                    if (hasLogPath)
                    {
                        Add(new NCSALogger(logPath));
                    }

                    if (IsAdministrator)
                    {
                        var banController = hasBansPath
                            ? new BanHammer(bansPath)
                            : new BanHammer();

#if !NETCOREAPP && !NETSTANDARD
                        banController.BanAdded += BanController_BanAdded;
                        banController.BanRemoved += BanController_BanRemoved;
#endif

                        Add(banController);
                    }
                }
                return true;
            }

            return false;
        }

        private bool Check(bool isValid, string message)
        {
            if (!isValid)
            {
                OnError(new ArgumentException(message));
            }

            return isValid;
        }

        private void BanController_BanAdded(object sender, EventArgs<CIDRBlock> e)
        {
            _ = Task.Run(() => AddBanAsync(e.Value));
        }

        private static Task<bool> BanExistsAsync(string name)
        {
            return new GetFirewallRule(name).ExistsAsync();
        }

        private async Task AddBanAsync(CIDRBlock block)
        {
            OnInfo($"Adding ban to firewall: {block}");
            var name = $"Ban {block}";
            var exists = await BanExistsAsync(name).ConfigureAwait(false);
            if (!exists)
            {
                var add = new AddFirewallRule(name, FirewallRuleDirection.In, FirewallRuleAction.Block, block);
                await add.RunAsync()
                    .ConfigureAwait(false);
            }
        }

        private void BanController_BanRemoved(object sender, EventArgs<CIDRBlock> e)
        {
            _ = Task.Run(() => RemoveBanAsync(e.Value));
        }

        private static async Task RemoveBanAsync(CIDRBlock e)
        {
            var name = $"Ban {e}";
            var exists = await BanExistsAsync(name).ConfigureAwait(false);
            if (exists)
            {
                var delete = new DeleteFirewallRule(name);
                await delete.RunAsync()
                    .ConfigureAwait(false);
            }
        }

#if !NETSTANDARD && !NETCOREAPP
        private void GetTLSParameters(out Guid guid, out string certHash)
        {
            var asm = Assembly.GetExecutingAssembly();
            guid = System.Runtime.InteropServices.Marshal.GetTypeLibGuidForAssembly(asm);
            certHash = null;
            using var store = new System.Security.Cryptography.X509Certificates.X509Store(
                System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine);

            store.Open(System.Security.Cryptography.X509Certificates.OpenFlags.ReadOnly);

            certHash = (from cert in store.Certificates.Cast<System.Security.Cryptography.X509Certificates.X509Certificate2>()
                        where cert.Subject == "CN=" + Domain
                          && DateTime.TryParse(cert.GetEffectiveDateString(), out var effectiveDate)
                          && DateTime.TryParse(cert.GetExpirationDateString(), out var expirationDate)
                          && effectiveDate <= DateTime.Now
                          && DateTime.Now < expirationDate
                        select cert.Thumbprint)
                       .FirstOrDefault();
        }


        private bool TryAssignCertToApp(string certHash, Guid appGuid, out string message)
        {
            var endpoint = new IPEndPoint(IPAddress.Parse("0.0.0.0"), HttpsPort.Value);

            var showCert = new ShowSslCert(endpoint);

            showCert.Info += OnInfo;
            showCert.Warning += OnWarning;
            showCert.Err += OnError;

            _ = showCert.Run();

            showCert.Info -= OnInfo;
            showCert.Warning -= OnWarning;
            showCert.Err -= OnError;

            if (showCert.TotalStandardOutput.Contains(appGuid.ToString()))
            {
                message = null;
                return true;
            }

            if (showCert.TotalStandardOutput.Contains(endpoint.ToString()))
            {
                var deleteCert = new DeleteSslCert(endpoint);

                deleteCert.Info += OnInfo;
                deleteCert.Warning += OnWarning;
                deleteCert.Err += OnError;

                _ = deleteCert.Run();

                deleteCert.Info -= OnInfo;
                deleteCert.Warning -= OnWarning;
                deleteCert.Err -= OnError;
            }

            var addCert = new AddSslCert(endpoint, certHash, appGuid);

            addCert.Info += OnInfo;
            addCert.Warning += OnWarning;
            addCert.Err += OnError;

            _ = addCert.Run();

            addCert.Info -= OnInfo;
            addCert.Warning -= OnWarning;
            addCert.Err -= OnError;

            message = addCert.TotalStandardOutput;
            return !message.Contains("The parameter is incorrect.");
        }
#endif

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
                            var isWebSocket = contextParamType == typeof(ServerWebSocketConnection);

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
    (Juniper.HTTP.Server.ServerWebSocketConnection, string...) => Task");
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

            if (controller is IInfoDestination infoDest)
            {
                Info += infoDest.OnInfo;
            }

            if (controller is IWarningSource warningSource)
            {
                warningSource.Warning += OnWarning;
            }

            if (controller is IWarningDestination warningDest)
            {
                Warning += warningDest.OnWarning;
            }

            if (controller is IErrorSource errorSource)
            {
                errorSource.Err += OnError;
            }

            if (controller is IErrorDestination errorDest)
            {
                Err += errorDest.OnError;
            }

            if (controller is INCSALogSource nCSALogSource)
            {
                nCSALogSource.Log += OnLog;
            }

            if (controller is INCSALogDestination nCSALogDest)
            {
                Log += nCSALogDest.OnLog;
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
                    && listenThread.IsAlive;
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

                if (controller is IInfoDestination infoDest)
                {
                    Info -= infoDest.OnInfo;
                }

                if (controller is IWarningSource warningSource)
                {
                    warningSource.Warning -= OnWarning;
                }

                if (controller is IWarningDestination warningDest)
                {
                    Warning -= warningDest.OnWarning;
                }

                if (controller is IErrorSource errorSource)
                {
                    errorSource.Err -= OnError;
                }

                if (controller is IErrorDestination errorDest)
                {
                    Err -= errorDest.OnError;
                }

                if (controller is INCSALogSource nCSALogSource)
                {
                    nCSALogSource.Log -= OnLog;
                }

                if (controller is INCSALogDestination nCSALogDest)
                {
                    Log -= nCSALogDest.OnLog;
                }
            }

            listener.Stop();
            listener.Close();
            var end = DateTime.Now.AddSeconds(3);
            while (listenThread.IsAlive && DateTime.Now < end)
            {
                Thread.Yield();
            }
        }

        public virtual void Start()
        {
            OnInfo("Starting server");

#if DEBUG
            var redirector = GetController<HttpToHttpsRedirect>();
            //redirector.Enabled = false;
#endif

            if (routes.Count > 0)
            {
                routes.Sort();
                ShowRoutes();
            }

            if (!AttemptCertificateAssignment()
                && HttpPort is null
                && HttpsPort is null)
            {
                OnError(new InvalidOperationException("No HTTP or HTTPS port specified."));
            }
            else
            {
                if (HttpsPort is object)
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    SetPrefix("https", HttpsPort.Value);
                }

                if (HttpPort is object)
                {
                    SetPrefix("http", HttpPort.Value);
                    ProductionHttpUsageWarning();
                }

                if (!listener.IsListening)
                {
                    OnInfo($"Listening on:");
                    foreach (var prefix in listener.Prefixes)
                    {
                        OnInfo($"\t{prefix}");
                    }

                    listener.Start();
                    listenThread.Start();
                    cleanupThread.Start();
                }
            }
        }

        private void ProductionHttpUsageWarning()
        {
#if !DEBUG
            var httpRoutes = from route in routes
                                where !(route is HttpToHttpsRedirect)
                                && !(route is BanHammer)
                                && !(route is NCSALogger)
                                && !(route is UnhandledRequestTrap)
                                && route.Protocols.HasFlag(HttpProtocols.HTTP)
                                select route;

            if (httpRoutes.Any())
            {
                OnWarning("Maybe don't run unencrypted HTTP in production, k?");
            }
#endif
        }

        private bool AttemptCertificateAssignment()
        {
            if (!AutoAssignCertificate)
            {
                return false;
            }

            var platform = Environment.OSVersion.Platform;
            if (HttpsPort is object
                && (platform == PlatformID.Win32NT
                    || platform == PlatformID.Win32Windows
                    || platform == PlatformID.Win32S))
            {
                if (string.IsNullOrEmpty(Domain))
                {
                    OnWarning("No domain was specified. Can't auto-assign a TLS certificate.");
                }
#if !NETCOREAPP && !NETSTANDARD
                else
                {
                    GetTLSParameters(out var guid, out var certHash);

                    if (string.IsNullOrEmpty(certHash))
                    {
                        OnWarning("No TLS cert found!");
                    }
                    else if (!TryAssignCertToApp(certHash, guid, out var message))
                    {
                        OnWarning($@"Couldn't configure the certificate correctly:
    Application GUID: {guid}
    TLS cert: {certHash}
    {message}");
                    }
                    else
                    {
                        OnInfo("Certificate assigned!");
                    }
                }
#else
                else
                {
                    OnWarning($"Don't know how to assign certificates on this platform: {platform}");
                }
#endif
            }

            return true;
        }

        private void SetPrefix(string protocol, ushort port)
        {
            if (!string.IsNullOrEmpty(protocol)
                && port >= 0)
            {
                var uri = new UriBuilder
                {
                    Scheme = protocol,

#if DEBUG
                    Host = "localhost",
#else
                    Host = "*",
#endif
                    Port = port
                };

                listener.Prefixes.Add(uri.ToString());
            }
        }

        private AuthenticationSchemes GetAuthenticationSchemeForRequest(HttpListenerRequest request)
        {
            var auth = (from route in routes
                        where route.IsRequestMatch(request)
                        orderby Math.Abs(route.Priority)
                        select route.Authentication)
                .FirstOrDefault();

            if (auth == AuthenticationSchemes.None)
            {
                auth = AuthenticationSchemes.Anonymous;
            }

            return auth;
        }

        private void Listen()
        {
            while (listener.IsListening)
            {
                if (waiters.Count < ListenerCount)
                {
                    waiters.Add(HandleConnectionAsync());
                }
            }
        }

        private void Cleanup()
        {
            while (listener.IsListening)
            {
                if (!waiters.IsEmpty
                    && waiters.TryTake(out var task)
                    && task.IsRunning())
                {
                    waiters.Add(task);
                }
            }
        }

        private async Task HandleConnectionAsync()
        {
            var context = await listener.GetContextAsync()
                    .ConfigureAwait(false);
            var response = context.Response;
            var request = context.Request;
            var headers = request.Headers;

            response.Headers["X-Frame-Options"] = "sameorigin";
            response.SetStatus(HttpStatusCode.Continue);

#if DEBUG
            PrintHeader(context);
#endif

            try
            {
                await ExecuteRoutesAsync(context)
                    .ConfigureAwait(false); ;
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception exp)
            {
                OnError(exp);
                response.SetStatus(HttpStatusCode.InternalServerError);
#if DEBUG
                await response.SendTextAsync(exp.Unroll())
                    .ConfigureAwait(false);
#endif
            }
#pragma warning restore CA1031 // Do not catch general exception types
            finally
            {
#if DEBUG
                PrintFooter(context);
                OnInfo("");
#endif

                await CleanupConnectionAsync(response, request)
                    .ConfigureAwait(false);
            }
        }

        private async Task ExecuteRoutesAsync(HttpListenerContext context)
        {
            foreach (var route in routes)
            {
                if (route.IsContextMatch(context))
                {
                    await route.ExecuteAsync(context)
                        .ConfigureAwait(false);
                }
            }
        }

        private async Task HandleErrorsAsync(HttpListenerResponse response, HttpListenerRequest request)
        {
            if (response.StatusCode >= 400)
            {
                var message = $"{request.RawUrl}: {response.StatusDescription}";
                if (response.StatusCode >= 500)
                {
                    OnError(new HttpListenerException(response.StatusCode, message));
                }
                else
                {
                    OnWarning(message);
                }

#if DEBUG
                await response.SendTextAsync(message)
#else
                await Task.CompletedTask
#endif
                    .ConfigureAwait(false);
            }
        }

        private async Task CleanupConnectionAsync(HttpListenerResponse response, HttpListenerRequest request)
        {
            response.StatusDescription = HttpStatusDescription.Get(response.StatusCode);

            await HandleErrorsAsync(response, request)
                .ConfigureAwait(false);

            await response
                .OutputStream
                .FlushAsync()
                .ConfigureAwait(true);

            if (!request.IsWebSocketRequest)
            {
                response.Close();
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
            var table = new string[routes.Count + 1, 8];
            table[0, 0] = "";
            table[0, 1] = "Type";
            table[0, 2] = "Method";
            table[0, 3] = "Priority";
            table[0, 4] = "Protocol";
            table[0, 5] = "Method";
            table[0, 6] = "Status";
            table[0, 7] = "Authentication";

            for (var i = 0; i < routes.Count; i++)
            {
                var route = routes[i];

                table[i + 1, 0] = route.Enabled ? "Y" : "N";

                var sepIndex = route.Name.IndexOf("::", StringComparison.Ordinal);
                if (sepIndex < 0)
                {
                    table[i + 1, 1] = route.Name;
                }
                else
                {
                    table[i + 1, 1] = route.Name.Substring(0, sepIndex);
                    table[i + 1, 2] = route.Name.Substring(sepIndex + 2);
                }

                table[i + 1, 3] = route.Priority.ToString(CultureInfo.InvariantCulture);
                table[i + 1, 4] = route.Protocols.ToString();
                table[i + 1, 5] = route.Methods.ToString();
                table[i + 1, 6] = route.StatusCodes.ToString();

                if (route.Authentication == AbstractResponse.AllAuthSchemes)
                {
                    table[i + 1, 7] = "Any";
                }
                else
                {
                    table[i + 1, 7] = route.Authentication.ToString();
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


#if DEBUG
        public System.Diagnostics.Process StartBrowser(bool preferHttps = false, string startPage = null)
        {
            if (HttpsPort is null
                && HttpPort is null)
            {
                throw new InvalidOperationException("Server is not listening on any ports");
            }

            startPage ??= string.Empty;

            var protocol = "http";
            var port = "";

            if (HttpPort is object
                && HttpPort != 80
                && (HttpsPort is null || !preferHttps))
            {
                port = HttpPort.Value.ToString(CultureInfo.InvariantCulture);
            }
            else if ((HttpPort is null || preferHttps)
                && HttpsPort is object
                && GetController<HttpToHttpsRedirect>()?.Enabled == true)
            {
                protocol = "https";
                if (HttpsPort != 443)
                {
                    port = HttpsPort.Value.ToString(CultureInfo.InvariantCulture);
                }
            }

            if (port.Length > 0)
            {
                port = ":" + port;
            }

            var page = $"{protocol}://localhost{port}/{startPage}";

            return System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo($"explorer", $"\"{page}\"")
            {
                UseShellExecute = true,
                WindowStyle = System.Diagnostics.ProcessWindowStyle.Maximized
            });
        }

        private void PrintContext(HttpListenerContext context, string name)
        {
            OnInfo($"{name}: {context.Request.HttpMethod} {context.Request.Url}");

            var contextT = context.GetType();
            var childProp = contextT.GetProperty(name);
            var childValue = childProp.GetValue(context, null);
            var childT = childValue.GetType();
            var headersProp = childT.GetProperty("Headers");
            var headers = (NameValueCollection)headersProp.GetValue(childValue, null);

            OnInfo("\tHeaders:");
            foreach (var key in headers.AllKeys)
            {
                var value = headers[key];
                OnInfo($"\t\t{key} = {value}");
            }
        }

        private void PrintHeader(HttpListenerContext context)
        {
            PrintContext(context, nameof(context.Request));
        }

        private void PrintFooter(HttpListenerContext context)
        {
            PrintContext(context, nameof(context.Response));
            OnInfo($"\tStatus: {context.Response.GetStatus()}");
        }
#endif
    }
}