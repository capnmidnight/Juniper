using System;

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
        public static string Q(string value) => "'" + value.Replace("'", "''")
                .Replace("\r\n", "\n")
                .Replace("\r", "\n")
                .Replace("\n", "\\n") + "'";

        /// <summary>
        /// Format a <see cref="DateTime"/> value as a string.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Q(DateTime value) =>
            Q(value.ToString("yyyy-MM-dd HH:mm:ss.f"));

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
        public static string Q(string name, string value) =>
            $"{name}={value}";

        /// <summary>
        /// Format a key-value par for use as a query string parameter, where the value is a date.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Q(string name, DateTime value) =>
            Q(name, value.ToString("yyyy-MM-dd"));

        /// <summary>
        /// Creates a reference to an HTTP endpoint service, with each of the relavent address parts
        /// broken out.
        /// </summary>
        /// <param name="scheme"></param>
        /// <param name="address"></param>
        /// <param name="port"></param>
        /// <param name="app"></param>
        protected AbstractService(string scheme, string address, int port, string app)
        {
            this.scheme = scheme;
            this.address = address;
            this.port = port;
            this.app = app;
        }

        /// <summary>
        /// Creates a reference to an HTTP endpoint service, with each of the relavent address parts
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
        /// <typeparam name="T"></typeparam>
        /// <param name="onComplete"></param>
        /// <param name="onError"></param>
        /// <param name="name"></param>
        /// <param name="parameters"></param>
        protected void GetObject<T>(Action<T> onComplete, Action<Exception> onError, string name, params string[] parameters)
        {
            var uri = new UriBuilder(scheme, address, port, $"{app}/{name}")
            {
                Query = string.Join("&", parameters)
            };

            HTTP.GetObject(uri.ToString(), onComplete, onError);
        }

        /// <summary>
        /// Perform a POST request to a named endpoint with an object as the request body, serialized
        /// to JSON.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="onComplete"></param>
        /// <param name="onError"></param>
        /// <param name="name"></param>
        /// <param name="requestBody"></param>
        protected void PostObject<T>(Action<T> onComplete, Action<Exception> onError, string name, object requestBody)
        {
            var uri = new UriBuilder(scheme, address, port, $"{app}/{name}");
            HTTP.PostObject(uri.ToString(), requestBody, onComplete, onError);
        }

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
