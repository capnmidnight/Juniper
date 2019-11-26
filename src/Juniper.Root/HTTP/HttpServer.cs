using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Authentication.ExtendedProtection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Juniper.HTTP
{
    public class HttpServer
    {
        private const int MAX_CONNECTIONS = 100;
        private readonly Thread serverThread;
        private readonly HttpListener listener;
        private readonly List<RouteAttribute> routes = new List<RouteAttribute>();
        private readonly List<Task> waiters = new List<Task>();

        /// <summary>
        /// Beging constructing a server.
        /// </summary>
        public HttpServer()
        {
            serverThread = new Thread(Listen);

            listener = new HttpListener
            {
                AuthenticationSchemeSelectorDelegate = GetAuthenticationSchemeForRequest
            };
        }

        public bool RedirectHttp2Https
        {
            get;
            set;
        }

        public string Domain
        {
            get;
            set;
        }

        public ushort HttpsPort
        {
            get;
            set;
        }

        public ushort HttpPort
        {
            get;
            set;
        }

#if DEBUG
        public string StartPage
        {
            get;
            set;
        }
#endif

        public void AddContentPath(string path)
        {
            if (!string.IsNullOrEmpty(path)
                && Directory.Exists(path))
            {
                OnInfo($"Serving content from path {path}");
                var defaultFileHandler = new DefaultFileController(path);
                defaultFileHandler.Warning += OnWarning;
                AddRoutesFrom(defaultFileHandler);
            }
        }

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

        public event EventHandler Update;

        private AuthenticationSchemes GetAuthenticationSchemeForRequest(HttpListenerRequest request)
        {
            return (from route in routes
                    where route.IsMatch(request)
                    select route.Authentication)
                .FirstOrDefault();
        }

        public void AddRoutesFrom(object controller)
        {
            var type = controller.GetType();
            foreach (var method in type.GetMethods(BindingFlags.Instance | BindingFlags.Public))
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

        private void SetPrefix(string protocol, ushort port)
        {
            if (!string.IsNullOrEmpty(protocol))
            {
                if (port > 0)
                {
                    OnInfo($"Listening for {protocol} on port {port}");
                    listener.Prefixes.Add($"{protocol}://*:{port}/");
                }
                else
                {
                    var prefix = listener.Prefixes.FirstOrDefault(p => p.EndsWith(":" + port));
                    if (!string.IsNullOrEmpty(prefix))
                    {
                        OnInfo($"Deleting prefix {prefix}");
                        listener.Prefixes.Remove(prefix);
                    }
                }
            }
        }

        public void Start()
        {
            if (!string.IsNullOrEmpty(Domain)
                && HttpsPort > 0)
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
                    OnInfo($"Application GUID: {guid}");
                    OnInfo($"TLS cert: {certHash}");
                    var message = AssignCertToApp(certHash, guid);

                    OnInfo(message);

                    if (message.Equals("SSL Certificate added successfully", StringComparison.InvariantCultureIgnoreCase)
                        || message.StartsWith("SSL Certificate add failed, Error: 183", StringComparison.InvariantCultureIgnoreCase))
                    {
                        SetPrefix("https", HttpsPort);
                    }
                    else if (message.Equals("The parameter is incorrect.", StringComparison.InvariantCultureIgnoreCase))
                    {
                        OnWarning(this, "Couldn't configure the certificate correctly");
                    }
                }
            }

            if (HttpPort > 0 || RedirectHttp2Https)
            {
                if (RedirectHttp2Https)
                {
                    if (HttpPort == 0)
                    {
                        HttpPort = 80;
                    }

                    AddRoutesFrom(new HttpsRedirectController());
                }
                else if (HttpPort > 0)
                {
                    OnWarning(this, "You probably shouldn't be serving raw HTTP content, unless you're behind a secure reverse proxy");
                }

                SetPrefix("http", HttpPort);
            }

            if (!listener.IsListening)
            {
                var prefixes = string.Join(", ", listener.Prefixes);
                OnInfo($"Listening on: {prefixes}");
                listener.Start();
            }

            if (!serverThread.IsAlive)
            {
                serverThread.Start();
            }

#if DEBUG
            if (!string.IsNullOrEmpty(StartPage))
            {
                var protocol = HttpsPort > 0
                    ? "https"
                    : "http";

                var port = HttpsPort > 0
                    ? HttpsPort == 443
                        ? ""
                        : ":" + HttpsPort
                    : ":" + HttpPort;

                Process.Start(new ProcessStartInfo($"explorer", $"\"{protocol}://{Domain}{port}/{StartPage}\"")
                {
                    UseShellExecute = true,
                    WindowStyle = ProcessWindowStyle.Maximized
                });
            }
#endif
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
            var procInfo = new ProcessStartInfo("netsh", $"http add sslcert ipport=0.0.0.0:{HttpsPort} certhash={certHash} appid={{{appGuid}}}")
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
                    Update?.Invoke(this, EventArgs.Empty);

                    for (int i = waiters.Count - 1; i >= 0; --i)
                    {
                        if (!waiters[i].IsRunning())
                        {
                            waiters.RemoveAt(i);
                        }
                    }

                    while(waiters.Count < MAX_CONNECTIONS)
                    {
                        waiters.Add(HandleConnection());
                    }
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch (Exception exp)
                {
                    OnError($"ERRROR: {exp.Message}");
                }
#pragma warning restore CA1031 // Do not catch general exception types
            }
        }

        private async Task HandleConnection()
        {
            var context = await listener.GetContextAsync();
            var requestID = $"{{{DateTime.Now.ToShortTimeString()}}} {context.Request.UrlReferrer} [{context.Request.HttpMethod}] {context.Request.Url.PathAndQuery} => {context.Request.RemoteEndPoint}";
            using (context.Response.OutputStream)
            using (context.Request.InputStream)
            {
                try
                {
                    OnInfo(requestID);

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