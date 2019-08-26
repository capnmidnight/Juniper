using System.IO;
using System.Text;
using System.Threading.Tasks;

using Juniper.HTTP;
using Juniper.Progress;
using Juniper.Streams;

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
            request.Header("Upgrade-Insecure-Requests", 1);
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
                valueString = b ? "1" : "0";
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
        /// Sets the method for the HTTP request.
        /// </summary>
        /// <param name="request">The request to which to add the Header</param>
        /// <param name="verb">The HTTP method to use for the request.</param>
        /// <returns>The request that was passed as the first argument, so that literate calls may be chained together.</returns>
        /// <example><![CDATA[
        /// var request = HttpWebRequestExt.Create("https://www.example.com");
        /// request.Method("GET")
        ///     .Header("Keep-Alive", 1)
        ///     .Header("DNT", 1")
        ///     .Accept(MediaType.Text.Plain");
        /// ]]></example>
        public static HttpWebRequest Method(this HttpWebRequest request, string verb)
        {
            request.Method = verb;
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
                var authBytes = Encoding.UTF8.GetBytes(authPair);
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
        private static async Task WriteBody(this HttpWebRequest request, Func<BodyInfo> getInfo, Action<Stream> writeBody)
        {
            var info = getInfo();
            if (info.Length > 0)
            {
                request.ContentLength = info.Length;
                request.ContentType = info.MIMEType;
                using (var stream = await request.GetRequestStreamAsync())
                {
                    writeBody(stream);
                }
            }
        }

        /// <summary>
        /// Perform a POST request, writing the body through a stream, and return the results as a stream.
        /// </summary>
        /// <param name="prog">Progress tracker (defaults to no progress tracking)</param>
        /// <returns>A stream that contains the response body, and an HTTP status code</returns>
        public static async Task<HttpWebResponse> Post(this HttpWebRequest request, Func<BodyInfo> getInfo, Action<Stream> writeBody)
        {
            request.Method = "POST";
            await request.WriteBody(getInfo, writeBody);
            return (HttpWebResponse)await request.GetResponseAsync();
        }

        /// <summary>
        /// Perform a POST request, writing the body through a stream, and return the results as a stream.
        /// </summary>
        /// <param name="prog">Progress tracker (defaults to no progress tracking)</param>
        /// <returns>A stream that contains the response body, and an HTTP status code</returns>
        public static async Task<HttpWebResponse> Post(this HttpWebRequest request)
        {
            request.Method = "POST";
            return (HttpWebResponse)await request.GetResponseAsync();
        }

        /// <summary>
        /// Perform a PUT request, writing the body through a stream, and return the results as a stream.
        /// </summary>
        /// <param name="prog">Progress tracker (defaults to no progress tracking)</param>
        /// <returns>A stream that contains the response body, and an HTTP status code</returns>
        public static async Task<HttpWebResponse> Put(this HttpWebRequest request, Func<BodyInfo> getInfo, Action<Stream> writeBody)
        {
            request.Method = "PUT";
            await request.WriteBody(getInfo, writeBody);
            return (HttpWebResponse)await request.GetResponseAsync();
        }

        /// <summary>
        /// Perform a PATCH request, writing the body through a stream, and return the results as a stream.
        /// </summary>
        /// <param name="prog">Progress tracker (defaults to no progress tracking)</param>
        /// <returns>A stream that contains the response body, and an HTTP status code</returns>
        public static async Task<HttpWebResponse> Patch(this HttpWebRequest request, Func<BodyInfo> getInfo, Action<Stream> writeBody)
        {
            request.Method = "PATCH";
            await request.WriteBody(getInfo, writeBody);
            return (HttpWebResponse)await request.GetResponseAsync();
        }

        /// <summary>
        /// Perform a DELETE request, writing the body through a stream, and return the results as a stream.
        /// </summary>
        /// <param name="prog">Progress tracker (defaults to no progress tracking)</param>
        /// <returns>A stream that contains the response body, and an HTTP status code</returns>
        public static async Task<HttpWebResponse> Delete(this HttpWebRequest request, Func<BodyInfo> getInfo, Action<Stream> writeBody)
        {
            request.Method = "DELETE";
            await request.WriteBody(getInfo, writeBody);
            return (HttpWebResponse)await request.GetResponseAsync();
        }

        /// <summary>
        /// Perform a DELETE request, writing the body through a stream, and return the results as a stream.
        /// </summary>
        /// <param name="prog">Progress tracker (defaults to no progress tracking)</param>
        /// <returns>A stream that contains the response body, and an HTTP status code</returns>
        public async static Task<HttpWebResponse> Delete(this HttpWebRequest request)
        {
            request.Method = "DELETE";
            return (HttpWebResponse)await request.GetResponseAsync();
        }

        /// <summary>
        /// Perform a GET request and return the results as a stream of bytes
        /// </summary>
        /// <param name="prog">Progress tracker (defaults to no progress tracking)</param>
        /// <returns>A stream that contains the response body, and an HTTP status code</returns>
        public static async Task<HttpWebResponse> Get(this HttpWebRequest request)
        {
            request.Method = "GET";
            return (HttpWebResponse)await request.GetResponseAsync();
        }

        /// <summary>
        /// Perform a HEAD request and return the results as a stream of bytes
        /// </summary>
        /// <param name="prog">Progress tracker (defaults to no progress tracking)</param>
        /// <returns>A stream that contains the response body, and an HTTP status code</returns>
        public static async Task<HttpWebResponse> Head(this HttpWebRequest request)
        {
            request.Method = "HEAD";
            return (HttpWebResponse)await request.GetResponseAsync();
        }

        public static async Task<Stream> CachedGetRaw(
            Uri uri,
            FileInfo cacheFile,
            Action<HttpWebRequest> modifyRequest = null,
            IProgress prog = null)
        {
            Stream body;
            long length;

            if (cacheFile != null
                && File.Exists(cacheFile?.FullName)
                && cacheFile.Length > 0)
            {
                length = cacheFile.Length;
                body = cacheFile.OpenRead();
            }
            else
            {
                var request = Create(uri);
                modifyRequest?.Invoke(request);

                var response = await request.Get();
                length = response.ContentLength;
                body = response.GetResponseStream();
                if (cacheFile != null)
                {
                    body = new CachingStream(body, cacheFile);
                }
            }

            body = new ProgressStream(body, length, prog);
            return body;
        }

        public static async Task<T> CachedGet<T>(
            Uri uri,
            Func<Stream, T> decode,
            FileInfo cacheFile = null,
            Action<HttpWebRequest> modifyRequest = null,
            IProgress prog = null)
        {
            using (var stream = await CachedGetRaw(uri, cacheFile, modifyRequest, prog))
            {
                return decode(stream);
            }
        }

        public static async Task CachedProxy(
            HttpListenerResponse outResponse,
            Uri uri,
            FileInfo cacheFile,
            Action<HttpWebRequest> modifyRequest = null)
        {
            if (cacheFile != null
                && File.Exists(cacheFile?.FullName)
                && cacheFile.Length > 0)
            {
                outResponse.SetStatus(HttpStatusCode.OK);
                outResponse.ContentType = cacheFile.GetContentType();
                outResponse.ContentLength64 = cacheFile.Length;
                outResponse.SendFile(cacheFile);
            }
            else
            {
                var request = Create(uri);
                modifyRequest?.Invoke(request);

                using (var inResponse = await request.Get())
                {
                    var body = inResponse.GetResponseStream();
                    if (cacheFile != null)
                    {
                        body = new CachingStream(body, cacheFile);
                    }
                    using (body)
                    {
                        outResponse.SetStatus(inResponse.StatusCode);
                        outResponse.ContentType = inResponse.ContentType;
                        if (inResponse.ContentLength >= 0)
                        {
                            outResponse.ContentLength64 = inResponse.ContentLength;
                        }
                        body.CopyTo(outResponse.OutputStream);
                    }
                }
            }
        }
    }
}