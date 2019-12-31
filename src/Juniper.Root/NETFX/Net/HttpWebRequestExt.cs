using System.IO;
using System.Threading.Tasks;

using Juniper.HTTP;
using Juniper.HTTP.Client;
using Juniper.Progress;

namespace System.Net
{
    /// <summary>
    /// Utility functions and extension methods on <see cref="HttpWebRequest"/> queries
    /// </summary>
    public static class HttpWebRequestExt
    {
        /// <summary>
        /// Creates a new <see cref="HttpWebRequest"/> object for a given URI
        /// with the "Upgrade-Insecure-Requests" header already set to True.
        /// </summary>
        /// <param name="uri">The <see cref="Uri"/> object for which to create the request.</param>
        /// <returns>An <see cref="HttpWebRequest"/> object.</returns>
        public static HttpWebRequest Create(Uri uri)
        {
            var request = (HttpWebRequest)WebRequest.Create(uri);
            if (uri.Scheme == "http")
            {
                request.Header("Upgrade-Insecure-Requests", 1);
            }

            return request;
        }

        /// <summary>
        /// Creates a new <see cref="HttpWebRequest"/> object for a given URI
        /// with the "Upgrade-Insecure-Requests" header already set to True.
        /// </summary>
        /// <param name="uri">A string value that represents a URI.</param>
        /// <returns>An <see cref="HttpWebRequest"/> object.</returns>
        public static HttpWebRequest Create(string url)
        {
            return Create(new Uri(url));
        }

        /// <summary>
        /// Adds an HTTP header to a <see cref="HttpWebRequest"/>, providing
        /// a literate interface for adding headers to requests. The header
        /// value will be converted to a string with its type's default
        /// ToString method.
        /// </summary>
        /// <typeparam name="T">The type of the value for the header that is being added to the request.</typeparam>
        /// <param name="request">The request to which to add the header</param>
        /// <param name="name">The name of the header that is being added to the request</param>
        /// <param name="value">The value of the header that is being added to the request. Boolean values get converted to "1" for True and "0" for False.</param>
        /// <returns>The request that was passed as the first argument, so that literate calls may be chained together.</returns>
        /// <example><![CDATA[
        /// var request = HttpWebRequestExt.Create("https://www.example.com");
        /// request.Header("Keep-Alive", 1)
        ///     .Header("DNT", 1")
        ///     .Header("Accept", MediaType.Text.Plain");
        /// ]]></example>
        public static HttpWebRequest Header<T>(this HttpWebRequest request, string name, T value)
        {
            string valueString;
            if (value is bool b)
            {
                valueString = b
                    ? "1"
                    : "0";
            }
            else
            {
                valueString = value.ToString();
            }

            request.Headers.Add(name, valueString);
            return request;
        }

        /// <summary>
        /// Adds the "DNT" header, set to True (1), for a given request.
        /// </summary>
        /// <param name="request">The request to which to add the header</param>
        /// <returns>The request that was passed as the first argument, so that literate calls may be chained together.</returns>
        /// <example><![CDATA[
        /// var request = HttpWebRequestExt.Create("https://www.example.com");
        /// request.Header("Keep-Alive", 1)
        ///     .DoNotTrack()
        ///     .Header("Accept", MediaType.Text.Plain");
        /// ]]></example>
        public static HttpWebRequest DoNotTrack(this HttpWebRequest request)
        {
            request.Header("DNT", 1);
            return request;
        }

        public static HttpWebRequest Host(this HttpWebRequest request, string host)
        {
            request.Host = host;
            return request;
        }

        public static HttpWebRequest UserAgent(this HttpWebRequest request, string agent)
        {
            request.UserAgent = agent;
            return request;
        }

        public static HttpWebRequest Referer(this HttpWebRequest request, string referer)
        {
            request.Referer = referer;
            return request;
        }

        public static HttpWebRequest FetchMode(this HttpWebRequest request, string mode)
        {
            request.Header("Sec-Fetch-Mode", mode);
            return request;
        }

        public static HttpWebRequest FetchSite(this HttpWebRequest request, string site)
        {
            request.Header("Sec-Fetch-Site", site);
            return request;
        }

        /// <summary>
        /// Sets the Accept header for the HTTP request.
        /// </summary>
        /// <param name="request">The request to which to add the Header</param>
        /// <param name="type">The Content-Type to use as the Header value.</param>
        /// <returns>The request that was passed as the first argument, so that literate calls may be chained together.</returns>
        /// <example><![CDATA[
        /// var request = HttpWebRequestExt.Create("https://www.example.com");
        /// request.Header("Keep-Alive", 1)
        ///     .Header("DNT", 1")
        ///     .Accept(MediaType.Text.Plain");
        /// ]]></example>
        public static HttpWebRequest Accept(this HttpWebRequest request, string type)
        {
            request.Accept = type;
            return request;
        }

        /// <summary>
        /// Sets the TransferEncoding header for the HTTP request.
        /// </summary>
        /// <param name="request">The request to which to add the Header</param>
        /// <param name="type">The encoding string to use as the Header value.</param>
        /// <returns>The request that was passed as the first argument, so that literate calls may be chained together.</returns>
        /// <example><![CDATA[
        /// var request = HttpWebRequestExt.Create("https://www.example.com");
        /// request.Header("Keep-Alive", 1)
        ///     .Header("DNT", 1")
        ///     .Accept(MediaType.Text.Plain")
        ///     .Encoding("utf-8");
        /// ]]></example>
        public static HttpWebRequest TransferEncoding(this HttpWebRequest request, string encoding)
        {
            request.SendChunked = true;
            request.TransferEncoding = encoding;
            return request;
        }

        /// <summary>
        /// Sets the Accept-Encoding header for the HTTP request.
        /// </summary>
        /// <param name="request">The request to which to add the Header</param>
        /// <param name="type">The encoding string to use as the Header value.</param>
        /// <returns>The request that was passed as the first argument, so that literate calls may be chained together.</returns>
        /// <example><![CDATA[
        /// var request = HttpWebRequestExt.Create("https://www.example.com");
        /// request.Header("Keep-Alive", 1)
        ///     .Header("DNT", 1")
        ///     .Accept(MediaType.Text.Plain")
        ///     .Encoding("utf-8");
        /// ]]></example>
        public static HttpWebRequest AcceptEncoding(this HttpWebRequest request, string encoding)
        {
            request.Header("Accept-Encoding", encoding);
            return request;
        }

        public static HttpWebRequest AcceptLanguage(this HttpWebRequest request, string language)
        {
            request.Header("Accept-Language", language);
            return request;
        }

        public static HttpWebRequest IfRange(this HttpWebRequest request, string value)
        {
            request.Header("If-Range", value);
            return request;
        }

        /// <summary>
        /// Sets the KeepAlive header to True.
        /// </summary>
        /// <param name="request">The request to which to add the Header</param>
        /// <returns>The request that was passed as the first argument, so that literate calls may be chained together.</returns>
        /// <example><![CDATA[
        /// var request = HttpWebRequestExt.Create("https://www.example.com");
        /// request.Header("Keep-Alive", 1)
        ///     .Header("DNT", 1")
        ///     .Accept(MediaType.Text.Plain")
        ///     .KeepAlive();
        /// ]]></example>
        public static HttpWebRequest KeepAlive(this HttpWebRequest request)
        {
            request.KeepAlive = true;
            return request;
        }

        public static HttpWebRequest Cookie(this HttpWebRequest request, string keyValue, string domain)
        {
            var parts = keyValue.SplitX('=');
            return request.Cookie(parts[0], parts[1], domain);
        }

        public static HttpWebRequest Cookie(this HttpWebRequest request, string key, string value, string domain)
        {
            var cookie = new Cookie(key, value, string.Empty, domain);
            request.CookieContainer ??= new CookieContainer();
            request.CookieContainer.Add(cookie);
            return request;
        }

        /// <summary>
        /// Sets the method for the HTTP request.
        /// </summary>
        /// <param name="request">The request to which to add the Header</param>
        /// <param name="verb">The HTTP method to use for the request.</param>
        /// <returns>The request that was passed as the first argument, so that literate calls may be chained together.</returns>
        /// <example><![CDATA[
        /// var request = HttpWebRequestExt.Create("https://www.example.com");
        /// request.Method(HttpMethod.GET)
        ///     .Header("Keep-Alive", 1)
        ///     .Header("DNT", 1")
        ///     .Accept(MediaType.Text.Plain");
        /// ]]></example>
        public static HttpWebRequest Method(this HttpWebRequest request, HttpMethods verb)
        {
            request.Method = verb.ToString();
            return request;
        }

        /// <summary>
        /// Sets the Authorization header for the request, using Basic HTTP auth.
        /// </summary>
        /// <param name="userName">Basic HTTP authentication user name.</param>
        /// <param name="password">Basic HTTP authentication user password.</param>
        /// <returns>The request that was passed as the first argument, so that literate calls may be chained together.</returns>
        public static HttpWebRequest BasicAuth(this HttpWebRequest request, string userName, string password)
        {
            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
            {
                var authPair = userName + ":" + password;
                var authBytes = Text.Encoding.UTF8.GetBytes(authPair);
                var auth64 = Convert.ToBase64String(authBytes);
                request.Header("Authorization", "Basic " + auth64);
            }

            return request;
        }

        /// <summary>
        /// Writes a body to an <see cref="HttpWebRequest"/>. Writing HTTP bodies with HttpWebRequest requires
        /// certain operations be done in a specific order, an order that might not make sense for every calling
        /// site. This method enforces that order.
        /// </summary>
        /// <param name="request">The request to which to write the </param>
        /// <param name="getInfo"></param>
        /// <param name="writeBody"></param>
        /// <returns></returns>
        private static async Task WriteBodyAsync(this HttpWebRequest request, Func<BodyInfo> getInfo, Action<Stream> writeBody, IProgress prog = null)
        {
            if (getInfo != null)
            {
                var info = getInfo();
                if (info != null)
                {
                    if (info.MIMEType != null)
                    {
                        request.ContentType = info.MIMEType;
                    }

                    if (info.Length >= 0)
                    {
                        request.ContentLength = info.Length;
                        if (info.Length > 0)
                        {
                            using var stream = await request.GetRequestStreamAsync()
                                   .ConfigureAwait(false);
                            WiteContent(writeBody, prog, info, stream);
                        }
                    }
                }
            }
        }

        private static void WiteContent(Action<Stream> writeBody, IProgress prog, BodyInfo info, Stream stream)
        {
            if (prog != null)
            {
                stream = new ProgressStream(stream, info.Length, prog);
            }

            writeBody(stream);
        }

        /// <summary>
        /// Perform a DELETE request, writing the body through a stream, and return the results as a stream.
        /// </summary>
        /// <param name="prog">Progress tracker (defaults to no progress tracking)</param>
        /// <returns>A stream that contains the response body, and an HTTP status code</returns>
        public static async Task<HttpWebResponse> DeleteAsync(this HttpWebRequest request, Func<BodyInfo> getInfo, Action<Stream> writeBody, IProgress prog)
        {
            request = request.Method(HttpMethods.DELETE);
            await request
                .WriteBodyAsync(getInfo, writeBody, prog)
                .ConfigureAwait(false);
            return (HttpWebResponse)await request.GetResponseAsync()
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Perform a DELETE request, writing the body through a stream, and return the results as a stream.
        /// </summary>
        /// <param name="prog">Progress tracker (defaults to no progress tracking)</param>
        /// <returns>A stream that contains the response body, and an HTTP status code</returns>
        public static Task<HttpWebResponse> DeleteAsync(this HttpWebRequest request)
        {
            return request.DeleteAsync(null, null, null);
        }

        /// <summary>
        /// Perform a GET request and return the results as a stream of bytes
        /// </summary>
        /// <param name="prog">Progress tracker (defaults to no progress tracking)</param>
        /// <returns>A stream that contains the response body, and an HTTP status code</returns>
        public static async Task<HttpWebResponse> GetAsync(this HttpWebRequest request)
        {
            return (HttpWebResponse)await request
                .Method(HttpMethods.GET)
                .GetResponseAsync()
                .ConfigureAwait(false);
        }
    }
}