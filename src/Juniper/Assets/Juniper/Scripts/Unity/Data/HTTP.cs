using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Juniper.Progress;
using Newtonsoft.Json;

namespace Juniper.Data
{
    /// <summary>
    /// Perform HTTP queries
    /// </summary>
    public sealed class HTTP
    {
        public class Result<T>
        {
            public readonly HttpStatusCode Status;
            public readonly T Value;
            internal Result(HttpStatusCode status, T value)
            {
                Status = status;
                Value = value;
            }
        }

        public class StreamResult : Result<Stream>, IDisposable
        {
            internal StreamResult(HttpStatusCode status, Stream value) : base(status, value) { }

            #region IDisposable Support
            private bool disposedValue = false; // To detect redundant calls

            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        Value.Dispose();
                    }
                    disposedValue = true;
                }
            }

            // This code added to correctly implement the disposable pattern.
            public void Dispose()
            {
                // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
                Dispose(true);
            }
            #endregion
        }

        /// <summary>
        /// Create the basic request object.
        /// </summary>
        /// <param name="method">GET/PUT/POST/etc.</param>
        /// <param name="url"></param>
        /// <param name="userName">Basic HTTP authentication user name.</param>
        /// <param name="password">Basic HTTP authentication user password.</param>
        /// <param name="contentLength">The length of the POST body in bytes.</param>
        /// <param name="contentType">The MIME type of the POST body.</param>
        /// <param name="acceptType">The MIME type of the expected response.</param>
        /// <returns>A web request object</returns>
        private static HttpWebRequest MakeRequest(string method, string url, string userName, string password, long contentLength, string contentType, string acceptType)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);

            request.Method = method;

            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
            {
                var authPair = userName + ":" + password;
                var authBytes = Encoding.UTF8.GetBytes(authPair);
                var auth64 = Convert.ToBase64String(authBytes);
                request.Headers.Add("Authorization", "Basic " + auth64);
            }

            if (!string.IsNullOrEmpty(acceptType))
            {
                request.Accept = acceptType;
            }

            if (contentLength > 0)
            {
                request.ContentLength = contentLength;

                if (!string.IsNullOrEmpty(contentType))
                {
                    request.ContentType = contentType;
                }
            }

            return request;
        }

        /// <summary>
        /// Reads the body from an HTTP response.
        /// </summary>
        /// <param name="request">The request that initiated the response</param>
        /// <param name="prog">Progress tracker (defaults to no progress tracking)</param>
        /// <returns>A stream that contains the response body, and an HTTP status code</returns>
        private static async Task<StreamResult> HandleResponse(HttpWebRequest request, IProgress prog = null)
        {
            var response = (HttpWebResponse)await request.GetResponseAsync();
            if (response.ContentLength == 0)
            {
                return new StreamResult(response.StatusCode, null);
            }
            else
            {
                return new StreamResult(response.StatusCode, new ProgressStream(response.GetResponseStream(), response.ContentLength, prog));
            }
        }

        /// <summary>
        /// Perform a POST request, writing the body through a stream, and return the results as a stream.
        /// </summary>
        /// <param name="url">The URL to request</param>
        /// <param name="userName">Basic HTTP authentication user name.</param>
        /// <param name="password">Basic HTTP authentication user password.</param>
        /// <param name="acceptType">The MIME of the expected response</param>
        /// <param name="contentLength">The total length of the body that needs to be written</param>
        /// <param name="contentType">The MIME type of both the request and response body</param>
        /// <param name="writeBody">A callback function for writing the body to a stream</param>
        /// <param name="prog">Progress tracker (defaults to no progress tracking)</param>
        /// <returns>A stream that contains the response body, and an HTTP status code</returns>
        public static async Task<StreamResult> PostStream(string url, string userName, string password, string acceptType, long contentLength, string contentType, Action<Stream> writeBody, IProgress prog = null)
        {
            var request = MakeRequest("POST", url, userName, password, contentLength, contentType, acceptType);
            var progs = prog.Split(2);
            using (var stream = new ProgressStream(await request.GetRequestStreamAsync(), contentLength, progs[0]))
            {
                writeBody(stream);
            }

            return await HandleResponse(request, progs[1]);
        }

        /// <summary>
        /// Perform a POST request, writing the body through a stream, and return the results as a stream.
        /// </summary>
        /// <param name="url">The URL to request</param>
        /// <param name="acceptType">The MIME of the expected response</param>
        /// <param name="contentLength">The total length of the body that needs to be written</param>
        /// <param name="contentType">The MIME type of both the request and response body</param>
        /// <param name="writeBody">A callback function for writing the body to a stream</param>
        /// <param name="prog">Progress tracker (defaults to no progress tracking)</param>
        /// <returns>A stream that contains the response body, and an HTTP status code</returns>
        public static Task<StreamResult> PostStream(string url, string acceptType, long contentLength, string contentType, Action<Stream> writeBody, IProgress prog = null)
        {
            return PostStream(url, null, null, acceptType, contentLength, contentType, writeBody, prog);
        }

        /// <summary>
        /// Perform a GET request and return the results as a stream of bytes
        /// </summary>
        /// <param name="url">The URL to request</param>
        /// <param name="acceptType">The MIME of the expected response</param>
        /// <param name="resolve">The callback to perform on success, with the HTTP status code and a stream payload</param>
        /// <param name="userName">Basic HTTP authentication user name.</param>
        /// <param name="password">Basic HTTP authentication user password.</param>
        /// <param name="prog">Progress tracker (defaults to no progress tracking)</param>
        /// <returns>A stream that contains the response body, and an HTTP status code</returns>
        public static async Task<StreamResult> GetStream(string url, string userName, string password, string acceptType, IProgress prog = null)
        {
            var request = MakeRequest("GET", url, userName, password, 0, null, acceptType);
            return await HandleResponse(request, prog);
        }

        /// <summary>
        /// Perform a GET request and return the results as an array of bytes, for the
        /// `application/unknown` MIME type
        /// </summary>
        /// <param name="url">The URL to request</param>
        /// <param name="userName">Basic HTTP authentication user name.</param>
        /// <param name="password">Basic HTTP authentication user password.</param>
        /// <param name="prog">Progress tracker (defaults to no progress tracking)</param>
        /// <returns>A stream that contains the response body, and an HTTP status code</returns>
        public static Task<StreamResult> GetStream(string url, string userName, string password, IProgress prog = null)
        {
            return GetStream(url, userName, password, "application/octet-stream", prog);
        }

        /// <summary>
        /// Perform a GET request and return the results as a stream of bytes
        /// </summary>
        /// <param name="url">The URL to request</param>
        /// <param name="acceptType">The MIME of the expected response</param>
        /// <param name="prog">Progress tracker (defaults to no progress tracking)</param>
        /// <returns>A stream that contains the response body, and an HTTP status code</returns>
        public static Task<StreamResult> GetStream(string url, string acceptType, IProgress prog = null)
        {
            return GetStream(url, null, null, acceptType, prog);
        }

        /// <summary>
        /// Perform a GET request and return the results as an array of bytes, for the
        /// `application/unknown` MIME type
        /// </summary>
        /// <param name="url">The URL to request</param>
        /// <param name="prog">Progress tracker (defaults to no progress tracking)</param>
        /// <returns>A stream that contains the response body, and an HTTP status code</returns>
        public static Task<StreamResult> GetStream(string url, IProgress prog = null)
        {
            return GetStream(url, null, null, prog);
        }

        /// <summary>
        /// Perform a GET request and return the results as a stream of bytes, for the
        /// `application/json` MIME type
        /// </summary>
        /// <param name="url">The URL to request</param>
        /// <param name="userName">Basic HTTP authentication user name.</param>
        /// <param name="password">Basic HTTP authentication user password.</param>
        /// <param name="prog">Progress tracker (defaults to no progress tracking)</param>
        /// <returns>A stream that contains the response body, and an HTTP status code</returns>
        public static Task<StreamResult> GetJSONStream(string url, string userName, string password, IProgress prog = null)
        {
            return GetStream(url, userName, password, "application/json", prog);
        }

        /// <summary>
        /// Perform a GET request and return the results as a stream of bytes, for the
        /// `application/json` MIME type
        /// </summary>
        /// <param name="url">The URL to request</param>
        /// <param name="prog">Progress tracker (defaults to no progress tracking)</param>
        /// <returns>A stream that contains the response body, and an HTTP status code</returns>
        public static Task<StreamResult> GetJSONStream(string url, IProgress prog = null)
        {
            return GetJSONStream(url, null, null, prog);
        }

        /// <summary>
        /// Perform a POST request, writing the body through a stream, and return the results as a stream.
        /// </summary>
        /// <param name="url">The URL to request</param>
        /// <param name="acceptType">The MIME of the expected response</param>
        /// <param name="body">The body to write to the POST request</param>
        /// <param name="contentType">The MIME of the POST body</param>
        /// <param name="prog">Progress tracker (defaults to no progress tracking)</param>
        /// <returns>A stream that contains the response body, and an HTTP status code</returns>
        public static Task<StreamResult> PostStream(string url, string acceptType, string body, string contentType, IProgress prog = null)
        {
            return PostStream(url, null, null, acceptType, body, contentType, prog);
        }

        /// <summary>
        /// Perform a POST request, writing the body through a stream, and return the results as a stream.
        /// </summary>
        /// <param name="url">The URL to request</param>
        /// <param name="userName">Basic HTTP authentication user name.</param>
        /// <param name="password">Basic HTTP authentication user password.</param>
        /// <param name="acceptType">The MIME type of the expected response</param>
        /// <param name="body">The body to write to the POST request</param>
        /// <param name="contentType">The MIME type of the POST body</param>
        /// <param name="prog">Progress tracker (defaults to no progress tracking)</param>
        /// <returns>A stream that contains the response body, and an HTTP status code</returns>
        public static Task<StreamResult> PostStream(string url, string userName, string password, string acceptType, string body, string contentType, IProgress prog = null)
        {
            var buf = Encoding.UTF8.GetBytes(body);
            var mem = new MemoryStream(buf);
            return PostStream(url, userName, password, acceptType, buf.Length, contentType, mem.CopyTo, prog);
        }

        /// <summary>
        /// Perform a GET request and return the results as string
        /// </summary>
        /// <param name="url">The URL to request</param>
        /// <param name="userName">Basic HTTP authentication user name.</param>
        /// <param name="password">Basic HTTP authentication user password.</param>
        /// <param name="acceptType">The MIME of the expected response</param>
        /// <param name="prog">Progress tracker (defaults to no progress tracking)</param>
        /// <returns>The response body as text, and an HTTP status code</returns>
        public static async Task<Result<string>> GetText(string url, string userName, string password, string acceptType, IProgress prog = null)
        {
            var result = await GetStream(
                url,
                userName,
                password,
                acceptType,
                prog);

            using (var reader = new StreamReader(result.Value))
            {
                return new Result<string>(result.Status, reader.ReadToEnd());
            }
        }

        /// <summary>
        /// Perform a GET request and return the results as string for the `application/unknown` MIME type
        /// </summary>
        /// <param name="url">The URL to request</param>
        /// <param name="userName">Basic HTTP authentication user name.</param>
        /// <param name="password">Basic HTTP authentication user password.</param>
        /// <param name="prog">Progress tracker (defaults to no progress tracking)</param>
        /// <returns>The response body as text, and an HTTP status code</returns>
        public static Task<Result<string>> GetText(string url, string userName, string password, IProgress prog = null)
        {
            return GetText(url, userName, password, "text/plain", prog);
        }

        /// <summary>
        /// Perform a GET request and return the results as string
        /// </summary>
        /// <param name="url">The URL to request</param>
        /// <param name="acceptType">The MIME of the expected response</param>
        /// <param name="prog">Progress tracker (defaults to no progress tracking)</param>
        /// <returns>The response body as text, and an HTTP status code</returns>
        public static Task<Result<string>> GetText(string url, string acceptType, IProgress prog = null)
        {
            return GetText(url, null, null, acceptType, prog);
        }

        /// <summary>
        /// Perform a GET request and return the results as string for the `application/unknown` MIME type
        /// </summary>
        /// <param name="url">The URL to request</param>
        /// <param name="prog">Progress tracker (defaults to no progress tracking)</param>
        /// <returns>The response body as text, and an HTTP status code</returns>
        public static Task<Result<string>> GetText(string url, IProgress prog = null)
        {
            return GetText(url, null, null, prog);
        }

        /// <summary>
        /// Perform a GET request and return the results as a stream of bytes, for the
        /// `application/json` MIME type
        /// </summary>
        /// <param name="url">The URL to request</param>
        /// <param name="userName">Basic HTTP authentication user name.</param>
        /// <param name="password">Basic HTTP authentication user password.</param>
        /// <param name="prog">Progress tracker (defaults to no progress tracking)</param>
        /// <returns>The response body as text, and an HTTP status code</returns>
        public static Task<Result<string>> GetJSONText(string url, string userName, string password, IProgress prog = null)
        {
            return GetText(url, userName, password, "application/json", prog);
        }

        /// <summary>
        /// Perform a JSON GET request and return the results as a string.
        /// </summary>
        /// <param name="url">The URL to request</param>
        /// <param name="prog">Progress tracker (defaults to no progress tracking)</param>
        /// <returns>The response body as text, and an HTTP status code</returns>
        public static Task<Result<string>> GetJSONText(string url, IProgress prog = null)
        {
            return GetJSONText(url, null, null, prog);
        }

        static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            DateTimeZoneHandling = DateTimeZoneHandling.Local,
            Formatting = Formatting.Indented,
            TypeNameHandling = TypeNameHandling.None
        };

        /// <summary>
        /// Perform a POST request, serializing the body from an object to JSON, and return the
        /// results as a JSON deserialized object.
        /// </summary>
        /// <param name="url">The URL to request</param>
        /// <param name="userName">Basic HTTP authentication user name.</param>
        /// <param name="password">Basic HTTP authentication user password.</param>
        /// <param name="body">The body to object to serialize to the body of the POST request</param>
        /// <param name="prog">Progress tracker (defaults to no progress tracking)</param>
        /// <returns>The requested object and an HTTP status code</returns>
        public static async Task<Result<ResponseT>> PostObject<ResponseT, BodyT>(string url, string userName, string password, BodyT body, IProgress prog = null)
        {
            var json = JsonConvert.SerializeObject(body, SerializerSettings);
            var buf = Encoding.UTF8.GetBytes(json);
            var mem = new MemoryStream(buf);
            var result = await PostStream(
                url,
                userName,
                password,
                "application/json",
                buf.Length,
                "application/json",
                mem.CopyTo,
                prog);

            using (var reader = new StreamReader(result.Value))
            {
                var jsonText = reader.ReadToEnd();
                return new Result<ResponseT>(result.Status, JsonConvert.DeserializeObject<ResponseT>(jsonText));
            }
        }

        /// <summary>
        /// Perform a POST request, serializing the body from an object to JSON, and return the
        /// results as a JSON deserialized object.
        /// </summary>
        /// <param name="url">The URL to request</param>
        /// <param name="body">The body to object to serialize to the body of the POST request</param>
        /// <param name="prog">Progress tracker (defaults to no progress tracking)</param>
        /// <returns>The requested object and an HTTP status code</returns>
        public static Task<Result<ResponseT>> PostObject<ResponseT, BodyT>(string url, BodyT body, IProgress prog = null)
        {
            return PostObject<ResponseT, BodyT>(url, null, null, body, prog);
        }

        /// <summary>
        /// Perform a JSON GET request and return the results as deserialized object.
        /// </summary>
        /// <param name="url">The URL to request</param>
        /// <param name="userName">Basic HTTP authentication user name.</param>
        /// <param name="password">Basic HTTP authentication user password.</param>
        /// <param name="resolve">The callback to perform on success, with the HTTP status code and a deserialized object payload</param>
        /// <param name="prog">Progress tracker (defaults to no progress tracking)</param>
        /// <returns>The requested object and an HTTP status code</returns>
        public static async Task<Result<ResponseT>> GetObject<ResponseT>(string url, string userName, string password, IProgress prog = null)
        {
            var result = await GetJSONText(url, userName, password, prog);
            return new Result<ResponseT>(result.Status, JsonConvert.DeserializeObject<ResponseT>(result.Value));
        }

        /// <summary>
        /// Perform a JSON GET request and return the results as deserialized object.
        /// </summary>
        /// <param name="url">The URL to request</param>
        /// <param name="prog">Progress tracker (defaults to no progress tracking)</param>
        /// <returns>The requested object and an HTTP status code</returns>
        public static Task<Result<ResponseT>> GetObject<ResponseT>(string url, IProgress prog = null)
        {
            return GetObject<ResponseT>(url, null, null, prog);
        }
    }
}
