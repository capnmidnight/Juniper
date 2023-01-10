using Juniper;

using Newtonsoft.Json;

using System.Net.Http.Headers;
using System.Text;

namespace System.Net.Http
{
    /// <summary>
    /// Utility functions and extension methods on <see cref="HttpRequestMessage"/> queries
    /// </summary>
    public static class HttpRequestMessageExt
    {
        /// <summary>
        /// Adds an HTTP header to a <see cref="HttpRequestMessage"/>, providing
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
        /// var request = HttpRequestMessageExt.Create("https://www.example.com");
        /// request.Header("Keep-Alive", 1)
        ///     .Header("DNT", 1")
        ///     .Header("Accept", MediaType.Text_Plain");
        /// ]]></example>
        public static HttpRequestMessage Header<T>(this HttpRequestMessage request, string name, T value)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (name is null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (name.Length == 0)
            {
                throw new ArgumentException("Header name must not be empty string", nameof(name));
            }

            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

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
        /// var request = HttpRequestMessageExt.Create("https://www.example.com");
        /// request.Header("Keep-Alive", 1)
        ///     .DoNotTrack()
        ///     .Header("Accept", MediaType.Text_Plain");
        /// ]]></example>
        public static HttpRequestMessage DoNotTrack(this HttpRequestMessage request)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return request.Header("DNT", 1);
        }

        public static HttpRequestMessage UserAgent(this HttpRequestMessage request, string agent)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var parts = agent.Split(' ').ToList();
            for (var i = parts.Count - 1; i > 0; i--)
            {
                if (parts[i].EndsWith(')')
                    && !parts[i].StartsWith('('))
                {
                    parts[i - 1] += " " + parts[i];
                    parts.RemoveAt(i);
                }
            }

            request.Headers.UserAgent.Clear();

            foreach (var part in parts)
            {
                if (ProductInfoHeaderValue.TryParse(part, out var value))
                {
                    request.Headers.UserAgent.Add(value);
                }
            }

            return request;
        }

        public static HttpRequestMessage ImpersonateGoogleChrome(this HttpRequestMessage request)
        {
            return request.UserAgent("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/98.0.4758.80 Safari/537.36");
        }

        public static HttpRequestMessage Referrer(this HttpRequestMessage request, string referrer)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            request.Headers.Referrer = new Uri(referrer);
            return request;
        }

        public static HttpRequestMessage FetchMode(this HttpRequestMessage request, string mode)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return request.Header("Sec-Fetch-Mode", mode);
        }

        public static HttpRequestMessage FetchSite(this HttpRequestMessage request, string site)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return request.Header("Sec-Fetch-Site", site);
        }

        /// <summary>
        /// Sets the Accept header for the HTTP request.
        /// </summary>
        /// <param name="request">The request to which to add the Header</param>
        /// <param name="type">The Content-Type to use as the Header value.</param>
        /// <returns>The request that was passed as the first argument, so that literate calls may be chained together.</returns>
        /// <example><![CDATA[
        /// var request = HttpRequestMessageExt.Create("https://www.example.com");
        /// request.Header("Keep-Alive", 1)
        ///     .Header("DNT", 1")
        ///     .Accept(MediaType.Text_Plain");
        /// ]]></example>
        public static HttpRequestMessage Accept(this HttpRequestMessage request, MediaType type)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            request.Headers.Accept.Add(type);
            return request;
        }

        /// <summary>
        /// Sets the TransferEncoding header for the HTTP request.
        /// </summary>
        /// <param name="request">The request to which to add the Header</param>
        /// <param name="type">The encoding string to use as the Header value.</param>
        /// <returns>The request that was passed as the first argument, so that literate calls may be chained together.</returns>
        /// <example><![CDATA[
        /// var request = HttpRequestMessageExt.Create("https://www.example.com");
        /// request.Header("Keep-Alive", 1)
        ///     .Header("DNT", 1")
        ///     .Accept(MediaType.Text_Plain)
        ///     .Encoding("utf-8");
        /// ]]></example>
        public static HttpRequestMessage TransferEncoding(this HttpRequestMessage request, string encoding)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            request.Headers.TransferEncodingChunked = true;
            request.Headers.TransferEncoding.Add(TransferCodingHeaderValue.Parse(encoding));
            return request;
        }

        /// <summary>
        /// Sets the Accept-Encoding header for the HTTP request.
        /// </summary>
        /// <param name="request">The request to which to add the Header</param>
        /// <param name="type">The encoding string to use as the Header value.</param>
        /// <returns>The request that was passed as the first argument, so that literate calls may be chained together.</returns>
        /// <example><![CDATA[
        /// var request = HttpRequestMessageExt.Create("https://www.example.com");
        /// request.Header("Keep-Alive", 1)
        ///     .Header("DNT", 1")
        ///     .Accept(MediaType.Text_Plain")
        ///     .Encoding("utf-8");
        /// ]]></example>
        public static HttpRequestMessage AcceptEncoding(this HttpRequestMessage request, string encoding)
        {
            return request.Header("Accept-Encoding", encoding);
        }

        public static HttpRequestMessage AcceptLanguage(this HttpRequestMessage request, string language)
        {
            return request.Header("Accept-Language", language);
        }

        public static HttpRequestMessage IfRange(this HttpRequestMessage request, string value)
        {
            return request.Header("If-Range", value);
        }

        /// <summary>
        /// Sets the KeepAlive header to True.
        /// </summary>
        /// <param name="request">The request to which to add the Header</param>
        /// <returns>The request that was passed as the first argument, so that literate calls may be chained together.</returns>
        /// <example><![CDATA[
        /// var request = HttpRequestMessageExt.Create("https://www.example.com");
        /// request.Header("Keep-Alive", 1)
        ///     .Header("DNT", 1")
        ///     .Accept(MediaType.Text_Plain")
        ///     .KeepAlive();
        /// ]]></example>
        public static HttpRequestMessage KeepAlive(this HttpRequestMessage request)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            request.Headers.Connection.Add("keep-alive");
            return request;
        }

        public static HttpRequestMessage Cookie(this HttpRequestMessage request, string key, string value, string path = null, string domain = null, DateTime? expiration = null, bool secure = true, bool httpOnly = true)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (key.Length == 0)
            {
                throw new ArgumentException("Key must have a value.", nameof(key));
            }

            if (value.Contains(';')
                || value.Contains(','))
            {
                value = JsonConvert.ToString(value);
            }

            var sb = new StringBuilder();
            sb.Append(key);
            sb.Append('=');
            sb.Append(value);

            if (path is not null)
            {
                sb.Append("; Path=");
                sb.Append(path);
            }

            if (domain is not null)
            {
                sb.Append("; Domain=");
                sb.Append(domain);
            }

            if (expiration is not null)
            {
                sb.Append("; Expires=");
                sb.Append(expiration.Value
                    .ToUniversalTime()
                    .ToString("R"));
            }

            if (secure)
            {
                sb.Append("; Secure");
            }

            if (httpOnly)
            {
                sb.Append("; HttpOnly");
            }

            return request.Header("Set-Cookie", sb.ToString());
        }

        /// <summary>
        /// Sets the method for the HTTP request.
        /// </summary>
        /// <param name="request">The request to which to add the Header</param>
        /// <param name="method">The HTTP method to use for the request.</param>
        /// <returns>The request that was passed as the first argument, so that literate calls may be chained together.</returns>
        /// <example><![CDATA[
        /// var request = HttpRequestMessageExt.Create("https://www.example.com");
        /// request.Method(HttpMethod.GET)
        ///     .Header("Keep-Alive", 1)
        ///     .Header("DNT", 1")
        ///     .Accept(MediaType.Text_Plain");
        /// ]]></example>
        public static HttpRequestMessage Method(this HttpRequestMessage request, HttpMethod method)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            request.Method = method;
            return request;
        }

        /// <summary>
        /// Sets the Authorization header for the request, using Basic HTTP auth.
        /// </summary>
        /// <param name="userName">Basic HTTP authentication user name.</param>
        /// <param name="password">Basic HTTP authentication user password.</param>
        /// <returns>The request that was passed as the first argument, so that literate calls may be chained together.</returns>
        public static HttpRequestMessage BasicAuth(this HttpRequestMessage request, string userName, string password)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
            {
                var authPair = userName + ":" + password;
                var authBytes = Encoding.UTF8.GetBytes(authPair);
                var auth64 = Convert.ToBase64String(authBytes);
                request.Header("Authorization", "Basic " + auth64);
            }

            return request;
        }

        /// <summary>
        /// Writes a body to an <see cref="HttpRequestMessage"/>. Writing HTTP bodies with HttpRequestMessage requires
        /// certain operations be done in a specific order, an order that might not make sense for every calling
        /// site. This method enforces that order.
        /// </summary>
        /// <param name="request">The request to which to write the </param>
        /// <param name="content"></param>
        /// <param name="contentType"></param>
        /// <param name="contentLength"></param>
        /// <returns></returns>
        public static HttpRequestMessage Body(this HttpRequestMessage request, HttpContent content, MediaType contentType = null, long? contentLength = null)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            request.Content = content;

            if (contentType is not null)
            {
                request.ContentType(contentType);
            }

            if (contentLength is not null)
            {
                request.ContentLength(contentLength.Value);
            }

            return request;
        }


        public static HttpContent ContentType(this HttpContent content, MediaType contentType)
        {
            if (contentType is null)
            {
                throw new ArgumentNullException(nameof(contentType));
            }

            content.Headers.ContentType = contentType;
            return content;
        }

        public static HttpRequestMessage ContentType(this HttpRequestMessage request, MediaType contentType)
        {
            request.Content.ContentType(contentType);
            return request;
        }

        public static HttpRequestMessage ContentLength(this HttpRequestMessage request, long contentLength)
        {
            request.Content.ContentLength(contentLength);
            return request;
        }

        public static HttpContent ContentLength(this HttpContent content, long contentLength)
        {
            content.Headers.ContentLength = contentLength;
            return content;
        }
    }
}