using System.IO;
using System.Text;
using System.Threading.Tasks;

using Juniper.HTTP;
using Juniper.Progress;
using Juniper.Streams;

namespace System.Net
{
    /// <summary>
    /// Perform HTTP queries
    /// </summary>
    public static class HttpWebRequestExt
    {
        public static HttpWebRequest Header(this HttpWebRequest request, string name, object value)
        {
            request.Headers.Add(name, value.ToString());
            return request;
        }

        public static HttpWebRequest Create(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Header("Upgrade-Insecure-Requests", 1);
            return request;
        }

        public static HttpWebRequest Create(Uri uri)
        {
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.Header("Upgrade-Insecure-Requests", 1);
            return request;
        }

        public static HttpWebRequest DoNotTrack(this HttpWebRequest request)
        {
            request.Header("DNT", 1);
            return request;
        }

        public static HttpWebRequest Accept(this HttpWebRequest request, string type)
        {
            request.Accept = type;
            return request;
        }

        /// <summary>
        /// Sets the Authorization header for the request, using Basic
        /// HTTP auth.
        /// </summary>
        /// <param name="userName">Basic HTTP authentication user name.</param>
        /// <param name="password">Basic HTTP authentication user password.</param>
        /// <returns>The requester object, to enable a literate interface.</returns>
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

        public static Task<T> CachedGet<T>(
            Uri uri,
            Func<Stream, T> decode,
            string cacheFileName = null,
            Action<HttpWebRequest> modifyRequest = null,
            IProgress prog = null)
        {
            FileInfo cacheFile = null;
            if (!string.IsNullOrEmpty(cacheFileName))
            {
                cacheFile = new FileInfo(cacheFileName);
            }

            return CachedGet(uri, decode, cacheFile, modifyRequest, prog);
        }

        public static Task<T> CachedGet<T>(
            Uri uri,
            Func<Stream, T> decode,
            FileInfo cacheFile,
            Action<HttpWebRequest> modifyRequest = null,
            IProgress prog = null)
        {
            return Task.Run(async () =>
            {
                Stream body = null;
                long length = 0;

                if (cacheFile?.Exists == true)
                {
                    body = cacheFile.OpenRead();
                    length = body.Length;
                }
                else
                {
                    var request = Create(uri);
                    modifyRequest?.Invoke(request);

                    var response = await request.Get();
                    body = response.GetResponseStream();
                    if (cacheFile != null)
                    {
                        body = new CachingStream(body, cacheFile);
                    }
                    length = response.ContentLength;
                }

                if (body == null)
                {
                    return default;
                }
                else
                {
                    body = new ProgressStream(body, length, prog);

                    using (body)
                    {
                        return decode(body);
                    }
                }
            });
        }

        public static Task<Stream> CachedGetRaw(
            Uri uri,
            FileInfo cacheFile,
            Action<HttpWebRequest> modifyRequest = null,
            IProgress prog = null)
        {
            return Task.Run(async () =>
            {
                Stream body;
                long length;
                if (cacheFile?.Exists == true)
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
            });
        }

        public static Task CachedProxy(
            HttpListenerResponse outResponse,
            Uri uri,
            FileInfo cacheFile,
            Action<HttpWebRequest> modifyRequest = null)
        {
            return Task.Run(async () =>
            {
                if (cacheFile?.Exists == true)
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

                    var inResponse = await request.Get();
                    var body = inResponse.GetResponseStream();
                    if (cacheFile != null)
                    {
                        body = new CachingStream(body, cacheFile);
                    }
                    using (body)
                    {
                        outResponse.SetStatus(inResponse.StatusCode);
                        outResponse.ContentType = inResponse.ContentType;
                        outResponse.ContentLength64 = inResponse.ContentLength;
                        foreach (var header in inResponse.Headers.AllKeys)
                        {
                            outResponse.AddHeader(header, inResponse.Headers[header]);
                        }
                        body.CopyTo(outResponse.OutputStream);
                    }
                }
            });
        }
    }
}