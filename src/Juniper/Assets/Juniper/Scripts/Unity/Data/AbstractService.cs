using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Juniper.HTTP;
using Juniper.Json;

namespace Juniper.Data
{
    /// <summary>
    /// An abstract class for making it easier to connect to remote web services and perform queries
    /// on them.
    /// </summary>
    public abstract class AbstractService
    {
        /// <summary>
        /// Normalize new-line values to escaped UNIX newlines, and surround the value in single quotes.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Q(string value)
        {
            return "'" + value.Replace("'", "''")
                            .Replace("\r\n", "\n")
                            .Replace("\r", "\n")
                            .Replace("\n", "\\n") + "'";
        }

        /// <summary>
        /// Format a <see cref="DateTime"/> value as a string.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Q(DateTime value)
        {
            return Q(value.ToString("yyyy-MM-dd HH:mm:ss.f"));
        }

        /// <summary>
        /// Format a Nullable <see cref="DateTime"/> value as a string.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Q(DateTime? value)
        {
            if (value == null)
            {
                return "null";
            }
            else
            {
                return Q(value.Value);
            }
        }

        /// <summary>
        /// Format a key-value pair for use as a query string parameter.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Q(string name, string value)
        {
            return $"{name}={value}";
        }

        /// <summary>
        /// Format a key-value par for use as a query string parameter, where the value is a date.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Q(string name, DateTime value)
        {
            return Q(name, value.ToString("yyyy-MM-dd"));
        }

        /// <summary>
        /// Creates a reference to an HTTP endpoint service, with each of the relevant address parts
        /// broken out.
        /// </summary>
        /// <param name="scheme"></param>
        /// <param name="address"></param>
        /// <param name="port"></param>
        /// <param name="app"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        protected AbstractService(string scheme, string address, int port, string app, string userName, string password)
        {
            this.scheme = scheme;
            this.address = address;
            this.port = port;
            this.app = app;
            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
            {
                authPair = new KeyValuePair<string, string>(userName, password);
            }
            else
            {
                authPair = null;
            }
        }

        /// <summary>
        /// Creates a reference to an HTTP endpoint service, with each of the relavent address parts
        /// broken out.
        /// </summary>
        /// <param name="scheme"></param>
        /// <param name="address"></param>
        /// <param name="port"></param>
        /// <param name="app"></param>
        protected AbstractService(string scheme, string address, int port, string app)
            : this(scheme, address, port, app, null, null)
        {
        }

        /// <summary>
        /// Creates a reference to an HTTP endpoint service, with each of the relavent address parts
        /// broken out. Defaults to using port 80 if the scheme is HTTP and 443 if the scheme is HTTPS.
        /// </summary>
        /// <param name="scheme"></param>
        /// <param name="address"></param>
        /// <param name="app"></param>
        protected AbstractService(string scheme, string address, string app, string userName, string password)
            : this(scheme, address, GetDefaultPort(scheme), app, userName, password)
        {
        }

        /// <summary>
        /// Creates a reference to an HTTP endpoint service, with each of the relevant address parts
        /// broken out. Defaults to using port 80 if the scheme is HTTP and 443 if the scheme is HTTPS.
        /// </summary>
        /// <param name="scheme"></param>
        /// <param name="address"></param>
        /// <param name="app"></param>
        protected AbstractService(string scheme, string address, string app)
            : this(scheme, address, GetDefaultPort(scheme), app)
        {
        }

        /// <summary>
        /// Perform a GET request to a named endpoint with a set of parameters for a certain object.
        /// </summary>
        /// <typeparam name="ResponseT"></typeparam>
        /// <param name="path"></param>
        /// <param name="parameters"></param>
        protected async Task<Result<ResponseT>> GetObject<ResponseT>(string path, params string[] parameters)
        {
            var uri = new UriBuilder(scheme, address, port, $"{app}/{path}")
            {
                Query = string.Join("&", parameters)
            }.ToString();

            var requester = new Requester(uri)
                .Accept("application/json");

            if(authPair != null)
            {
                requester = requester.BasicAuth(authPair.Value.Key, authPair.Value.Value);
            }

            var stream = await requester.Get();
            return new Result<ResponseT>(stream.Status, stream.MIMEType, stream.Value.ReadObject<ResponseT>());
        }

        /// <summary>
        /// Perform a POST request to a named endpoint with an object as the request body, serialized to JSON.
        /// </summary>
        /// <typeparam name="ResponseT"></typeparam>
        /// <param name="name"></param>
        /// <param name="requestBody"></param>
        protected async Task<Result<ResponseT>> PostObject<ResponseT, BodyT>(string name, BodyT requestBody)
        {
            var uri = new UriBuilder(scheme, address, port, $"{app}/{name}").ToString();

            var requester = new Requester(uri)
                .Accept("application/json");

            if (authPair != null)
            {
                requester = requester.BasicAuth(authPair.Value.Key, authPair.Value.Value);
            }

            var stream = await requestBody.Write(requester.Post);

            return new Result<ResponseT>(stream.Status, stream.MIMEType, stream.Value.ReadObject<ResponseT>());
        }

        /// <summary>
        /// Username and password
        /// </summary>
        private readonly KeyValuePair<string, string>? authPair;

        /// <summary>
        /// http or https
        /// </summary>
        private readonly string scheme;

        /// <summary>
        /// The full domain name.
        /// </summary>
        private readonly string address;

        /// <summary>
        /// The server port at which to make the request.
        /// </summary>
        private readonly int port;

        /// <summary>
        /// The part past the first slash after the domain.
        /// </summary>
        private readonly string app;

        /// <summary>
        /// For a given protocol, get the default port.
        /// </summary>
        /// <param name="scheme"></param>
        /// <returns></returns>
        private static int GetDefaultPort(string scheme)
        {
            if (scheme == "http")
            {
                return 80;
            }
            else if (scheme == "https")
            {
                return 443;
            }
            else
            {
                throw new ArgumentException("Scheme must be either 'http' or 'https'.");
            }
        }
    }
}
