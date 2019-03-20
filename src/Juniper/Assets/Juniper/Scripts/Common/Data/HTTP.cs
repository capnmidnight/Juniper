using Juniper.Progress;

using Newtonsoft.Json;

using System;
using System.IO;
using System.Net;
using System.Text;

namespace Juniper
{
    /// <summary>
    /// Perform HTTP queries
    /// </summary>
    public sealed class HTTP
    {
        /// <summary>
        /// Perform a GET request and return the results as a stream of bytes
        /// </summary>
        /// <param name="url">The URL to request</param>
        /// <param name="contentType">The MIME of the expected response</param>
        /// <param name="resolve">The callback to perform on success, with the stream payload</param>
        /// <param name="reject">The callback to perform on error</param>
        /// <param name="prog">An optional progress tracker. Defaults to null (no progress tracking).</param>
        public static void GetStream(string url, string contentType, Action<Stream> resolve, Action<Exception> reject, IProgressReceiver prog = null)
        {
            prog?.Report(0);

            Action<Stream> progResolve = (Stream resolveStream) =>
            {
                prog?.Report(1);
                resolve(resolveStream);
            };

            if (reject == null)
            {
                reject = EmptyRejection;
            }

            try
            {
                var request = MakeRequest("GET", url, contentType ?? "application/unknown");
                var http = new HTTP(request, progResolve, reject, prog);
                request.BeginGetResponse(new AsyncCallback(GetResponseCallback), http);
            }
            catch (Exception exp)
            {
                reject(new Exception($"creating request object [{url}]", exp));
            }
        }

        /// <summary>
        /// Perform a GET request and return the results as an array of bytes, for the
        /// `application/unknown` MIME type
        /// </summary>
        /// <param name="url">The URL to request</param>
        /// <param name="resolve">The callback to perform on success, with the byte array payload</param>
        /// <param name="reject">The callback to perform on error</param>
        /// <param name="prog">An optional progress tracker. Defaults to null (no progress tracking).</param>
        public static void GetByteStream(string url, Action<Stream> resolve, Action<Exception> reject, IProgressReceiver prog = null)
        {
            GetStream(url, "application/octet-stream", resolve, reject, prog);
        }

        /// <summary>
        /// Perform a GET request and return the results as an array of bytes
        /// </summary>
        /// <param name="url">The URL to request</param>
        /// <param name="contentType">The MIME of the expected response</param>
        /// <param name="resolve">The callback to perform on success, with the byte array payload</param>
        /// <param name="reject">The callback to perform on error</param>
        /// <param name="prog">An optional progress tracker. Defaults to null (no progress tracking).</param>
        public static void GetBytes(string url, string contentType, Action<byte[]> resolve, Action<Exception> reject, IProgressReceiver prog = null)
        {
            GetStream(
                url,
                contentType,
                stream => resolve(stream.ReadBytes(prog)),
                reject,
                prog);
        }

        /// <summary>
        /// Perform a GET request and return the results as an array of bytes, for the
        /// `application/unknown` MIME type
        /// </summary>
        /// <param name="url">The URL to request</param>
        /// <param name="resolve">The callback to perform on success, with the byte array payload</param>
        /// <param name="reject">The callback to perform on error</param>
        /// <param name="prog">An optional progress tracker. Defaults to null (no progress tracking).</param>
        public static void GetBytes(string url, Action<byte[]> resolve, Action<Exception> reject, IProgressReceiver prog = null)
        {
            GetBytes(url, "application/octet-stream", resolve, reject, prog);
        }

        /// <summary>
        /// Perform a GET request and return the results as string
        /// </summary>
        /// <param name="url">The URL to request</param>
        /// <param name="contentType">The MIME of the expected response</param>
        /// <param name="resolve">The callback to perform on success, with the string payload</param>
        /// <param name="reject">The callback to perform on error</param>
        /// <param name="prog">An optional progress tracker. Defaults to null (no progress tracking).</param>
        public static void GetText(string url, string contentType, Action<string> resolve, Action<Exception> reject, IProgressReceiver prog = null)
        {
            GetStream(
                url,
                contentType,
                stream => resolve(stream.ReadString(prog)),
                reject,
                prog);
        }

        /// <summary>
        /// Perform a GET request and return the results as string for the `application/unknown` MIME type
        /// </summary>
        /// <param name="url">The URL to request</param>
        /// <param name="resolve">The callback to perform on success, with the string payload</param>
        /// <param name="reject">The callback to perform on error</param>
        /// <param name="prog">An optional progress tracker. Defaults to null (no progress tracking).</param>
        public static void GetText(string url, Action<string> resolve, Action<Exception> reject, IProgressReceiver prog = null)
        {
            GetText(url, "text/plain", resolve, reject, prog);
        }

        /// <summary>
        /// Perform a GET request and return the results as a stream of bytes, for the
        /// `application/json` MIME type
        /// </summary>
        /// <param name="url">The URL to request</param>
        /// <param name="resolve">The callback to perform on success, with the stream payload</param>
        /// <param name="reject">The callback to perform on error</param>
        /// <param name="prog">An optional progress tracker. Defaults to null (no progress tracking).</param>
        public static void GetJSON(string url, Action<Stream> resolve, Action<Exception> reject, IProgressReceiver prog = null)
        {
            GetStream(url, "application/json", resolve, reject, prog);
        }

        /// <summary>
        /// Perform a JSON GET request and return the results as a string.
        /// </summary>
        /// <param name="url">The URL to request</param>
        /// <param name="resolve">The callback to perform on success, with the JSON string payload</param>
        /// <param name="reject">The callback to perform on error</param>
        /// <param name="prog">An optional progress tracker. Defaults to null (no progress tracking).</param>
        public static void GetJSON(string url, Action<string> resolve, Action<Exception> reject, IProgressReceiver prog = null)
        {
            GetText(url, "application/json", resolve, reject, prog);
        }

        /// <summary>
        /// Perform a POST request, writing the body through a stream, and return thet results as a stream.
        /// </summary>
        /// <param name="url">The URL to request</param>
        /// <param name="mime">The MIME type of both the request and response body</param>
        /// <param name="writeBody">A callback function for writing the body to a stream</param>
        /// <param name="resolve">The callback to perform on success, with the stream payload</param>
        /// <param name="reject">The callback to perform on error</param>
        /// <param name="prog">An optional progress tracker. Defaults to null (no progress tracking).</param>
        public static void PostStream(string url, string mime, long bodyLength, Action<Stream> writeBody, Action<Stream> resolve, Action<Exception> reject, IProgressReceiver prog = null)
        {
            try
            {
                var request = MakeRequest("POST", url, mime);
                var http = new HTTP(request, bodyLength, writeBody, resolve, reject, prog);
                request.BeginGetRequestStream(new AsyncCallback(GetRequestStream), http);
            }
            catch (Exception exp)
            {
                reject(new Exception($"creating request object [{url}]", exp));
            }
        }

        /// <summary>
        /// Perform a POST request, writing the body through a stream, and return thet results as a stream.
        /// </summary>
        /// <param name="url">The URL to request</param>
        /// <param name="mime">The MIME type of both the request and response body</param>
        /// <param name="body">The body to write to the POST request</param>
        /// <param name="resolve">The callback to perform on success, with the stream payload</param>
        /// <param name="reject">The callback to perform on error</param>
        /// <param name="prog">An optional progress tracker. Defaults to null (no progress tracking).</param>
        public static void PostStream(string url, string mime, string body, Action<Stream> resolve, Action<Exception> reject, IProgressReceiver prog = null)
        {
            var buf = Encoding.UTF8.GetBytes(body);
            PostStream(
                url, mime,
                buf.Length,
                stream => stream.Write(buf, 0, buf.Length),
                resolve, reject, prog);
        }

        /// <summary>
        /// Perform a JSON GET request and return the results as deserialized object.
        /// </summary>
        /// <param name="url">The URL to request</param>
        /// <param name="resolve">
        /// The callback to perform on success, with the deserialized object payload
        /// </param>
        /// <param name="reject">The callback to perform on error</param>
        /// <param name="prog">An optional progress tracker. Defaults to null (no progress tracking).</param>
        public static void GetObject<T>(string url, Action<T> resolve, Action<Exception> reject, IProgressReceiver prog = null)
        {
            try
            {
                GetJSON(
                    url,
                    jsonText => resolve(JsonConvert.DeserializeObject<T>(jsonText)),
                    reject,
                    prog);
            }
            catch (Exception exp)
            {
                reject(exp);
            }
        }

        /// <summary>
        /// Perform a POST request, serializing the body from an object to JSON, and return thet
        /// results as a JSON deserialized object.
        /// </summary>
        /// <param name="url">The URL to request</param>
        /// <param name="body">The body to object to serialize to the body of the POST request</param>
        /// <param name="resolve">
        /// The callback to perform on success, with the deserialized object payload
        /// </param>
        /// <param name="reject">The callback to perform on error</param>
        /// <param name="prog">An optional progress tracker. Defaults to null (no progress tracking).</param>
        public static void PostObject<T, U>(string url, U body, Action<T> resolve, Action<Exception> reject, IProgressReceiver prog = null)
        {
            try
            {
                byte[] buf = null;
                using (var mem = new MemoryStream())
                using (var writer = new StreamWriter(mem))
                {
                    writer.Write(JsonConvert.SerializeObject(body));
                    mem.Flush();
                    buf = mem.GetBuffer();
                }

                PostStream(
                    url, "application/json",
                    buf.Length,
                    stream => stream.Write(buf, 0, buf.Length),
                    stream =>
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            resolve(JsonConvert.DeserializeObject<T>(reader.ReadToEnd()));
                        }
                    },
                    reject,
                    prog);
            }
            catch (Exception exp)
            {
                reject(exp);
            }
        }

        /// <summary>
        /// The HTTP request that is being made to a remote server.
        /// </summary>
        private readonly HttpWebRequest request;

        /// <summary>
        /// The callback to use when a request body needs to be written.
        /// </summary>
        private Action<Stream> writeBody;

        /// <summary>
        /// The callback to use when a byte stream has been requested.
        /// </summary>
        private Action<Stream> resolve;

        /// <summary>
        /// The callback to use when there is an error.
        /// </summary>
        private readonly Action<Exception> reject;

        /// <summary>
        /// Initialize a byte stream request handler.
        /// </summary>
        /// <param name="request">The request to handle</param>
        /// <param name="resolve">The callback to execute when the request completes successfully</param>
        /// <param name="reject">The callback to execute if the request fails.</param>
        /// <param name="prog">An optional progress tracker. Defaults to null (no progress tracking).</param>
        private HTTP(HttpWebRequest request, Action<Stream> resolve, Action<Exception> reject, IProgressReceiver prog = null)
        {
            this.request = request;
            this.reject = reject;

            SetReadResponse(request, resolve, prog);
        }

        /// <summary>
        /// Initialize a byte stream request handler that first writes a request body.
        /// </summary>
        /// <param name="request">The request to handle</param>
        /// <param name="bodyLength">The length, in bytes, of the request body that needs to be written.</param>
        /// <param name="writeBody">A callback to execute when the system is ready to write the request body.</param>
        /// <param name="resolve">The callback to execute when the request completes successfully</param>
        /// <param name="reject">The callback to execute if the request fails.</param>
        /// <param name="prog">An optional progress tracker. Defaults to null (no progress tracking).</param>
        private HTTP(HttpWebRequest request, long bodyLength, Action<Stream> writeBody, Action<Stream> resolve, Action<Exception> reject, IProgressReceiver prog = null)
        {
            this.request = request;
            this.reject = reject;

            var progs = prog.Split(2);
            SetWriteBody(bodyLength, writeBody, progs[0]);
            SetReadResponse(request, resolve, progs[1]);
        }

        /// <summary>
        /// Wraps the response reading callback with a progress report.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="resolve"></param>
        /// <param name="prog"></param>
        private void SetReadResponse(HttpWebRequest request, Action<Stream> resolve, IProgressReceiver prog)
        {
            var readProgress = new StreamProgress();

            this.resolve = stream =>
            {
                readProgress.SetStream(stream, request.ContentLength, prog);
                resolve(readProgress);
            };
        }

        /// <summary>
        /// Wraps the body writing callback with a progress report.
        /// </summary>
        /// <param name="bodyLength"></param>
        /// <param name="writeBody"></param>
        /// <param name="prog"></param>
        private void SetWriteBody(long bodyLength, Action<Stream> writeBody, IProgressReceiver prog)
        {
            var writeProgress = new StreamProgress();

            this.writeBody = stream =>
            {
                writeProgress.SetStream(stream, bodyLength, prog);
                writeBody(writeProgress);
                writeProgress.Flush();
            };
        }

        /// <summary>
        /// A default error handler that does nothing.
        /// </summary>
        /// <param name="exp"></param>
        private static void EmptyRejection(Exception exp)
        {
        }

        /// <summary>
        /// Create the basic request object.
        /// </summary>
        /// <param name="method">GET/PUT/POST/etc.</param>
        /// <param name="url"></param>
        /// <param name="contentType">Also called MIME</param>
        /// <returns></returns>
        private static HttpWebRequest MakeRequest(string method, string url, string contentType)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = contentType;
            request.Method = method;
            return request;
        }

        /// <summary>
        /// If we know we need to write a request body, get a stream to which we can write the body.
        /// </summary>
        /// <param name="asynchronousResult"></param>
        private static void GetRequestStream(IAsyncResult asynchronousResult)
        {
            var http = (HTTP)asynchronousResult.AsyncState;
            try
            {
                using (var requestBody = http.request.EndGetRequestStream(asynchronousResult))
                {
                    http.writeBody(requestBody);
                    http.request.BeginGetResponse(new AsyncCallback(GetResponseCallback), http);
                }
            }
            catch (Exception exp)
            {
                http.reject(new Exception($"retrieving request stream [{http.request.RequestUri}]", exp));
            }
        }

        /// <summary>
        /// Get the response that the server sent for the request.
        /// </summary>
        /// <param name="asynchronousResult"></param>
        private static void GetResponseCallback(IAsyncResult asynchronousResult)
        {
            var http = (HTTP)asynchronousResult.AsyncState;
            try
            {
                var response = (HttpWebResponse)http.request.EndGetResponse(asynchronousResult);
                if (http.resolve != null)
                {
                    using (var stream = response.GetResponseStream())
                    {
                        http.resolve(stream);
                    }
                }
                else
                {
                    http.reject(new Exception($"Couldn't figure out how to handle the request for [{http.request.RequestUri}]"));
                }
            }
            catch (Exception exp)
            {
                http.reject(new Exception($"retrieving response [{http.request.RequestUri}]", exp));
            }
        }
    }
}
