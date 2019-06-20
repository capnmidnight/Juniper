using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Juniper.Progress;

namespace Juniper.HTTP
{
    /// <summary>
    /// Perform HTTP queries
    /// </summary>
    public class Requester
    {
        private readonly HttpWebRequest request;

        /// <summary>
        /// Creates a new HTTP request object that can be modified in-place before
        /// being sent across the web.
        /// </summary>
        /// <param name="url">The URL to request</param>
        public Requester(string url)
        {
            request = (HttpWebRequest)WebRequest.Create(url);
            Header("Upgrade-Insecure-Request", 1);
            Header("DNT", 1);
            request.CachePolicy = new System.Net.Cache.HttpRequestCachePolicy(System.Net.Cache.HttpRequestCacheLevel.NoCacheNoStore);
        }

        /// <summary>
        /// Set an arbitrary header value on the HTTP request.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Requester Header(string name, object value)
        {
            request.Headers.Add(name, value.ToString());

            return this;
        }

        /// <summary>
        /// Sets the Authorization header for the request, using Basic
        /// HTTP auth.
        /// </summary>
        /// <param name="userName">Basic HTTP authentication user name.</param>
        /// <param name="password">Basic HTTP authentication user password.</param>
        /// <returns>The requester object, to enable a literate interface.</returns>
        public Requester BasicAuth(string userName, string password)
        {
            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
            {
                var authPair = userName + ":" + password;
                var authBytes = Encoding.UTF8.GetBytes(authPair);
                var auth64 = Convert.ToBase64String(authBytes);
                Header("Authorization", "Basic " + auth64);
            }

            return this;
        }

        /// <summary>Sets the MIME-type that we expect to come back from the server.</summary>
        /// <param name="acceptType">The MIME type of the expected response.</param>
        /// <returns>The requester object, to enable a literate interface.</returns>
        public Requester Accept(string acceptType)
        {
            if (!string.IsNullOrEmpty(acceptType))
            {
                request.Accept = acceptType;
            }

            return this;
        }

        private void SetDefaultAcceptType()
        {
            if (string.IsNullOrEmpty(request.Accept))
            {
                request.Accept = "application/octet-stream";
            }
        }

        private async Task WriteBody(Func<BodyInfo> getInfo, Action<Stream> writeBody)
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
        public async Task<HttpWebResponse> Post(Func<BodyInfo> getInfo, Action<Stream> writeBody, IProgress prog = null)
        {
            request.Method = "POST";
            SetDefaultAcceptType();
            await WriteBody(getInfo, writeBody);
            return (HttpWebResponse)await request.GetResponseAsync();
        }

        /// <summary>
        /// Perform a PUT request, writing the body through a stream, and return the results as a stream.
        /// </summary>
        /// <param name="prog">Progress tracker (defaults to no progress tracking)</param>
        /// <returns>A stream that contains the response body, and an HTTP status code</returns>
        public async Task<StreamResult> Put(Func<BodyInfo> getInfo, Action<Stream> writeBody, IProgress prog = null)
        {
            request.Method = "PUT";
            await WriteBody(getInfo, writeBody);
            return (HttpWebResponse)await request.GetResponseAsync();
        }

        /// <summary>
        /// Perform a PATCH request, writing the body through a stream, and return the results as a stream.
        /// </summary>
        /// <param name="prog">Progress tracker (defaults to no progress tracking)</param>
        /// <returns>A stream that contains the response body, and an HTTP status code</returns>
        public async Task<StreamResult> Patch(Func<BodyInfo> getInfo, Action<Stream> writeBody, IProgress prog = null)
        {
            request.Method = "PATCH";
            SetDefaultAcceptType();
            await WriteBody(getInfo, writeBody);
            return (HttpWebResponse)await request.GetResponseAsync();
        }

        /// <summary>
        /// Perform a DELETE request, writing the body through a stream, and return the results as a stream.
        /// </summary>
        /// <param name="prog">Progress tracker (defaults to no progress tracking)</param>
        /// <returns>A stream that contains the response body, and an HTTP status code</returns>
        public async Task<StreamResult> Delete(Func<BodyInfo> getInfo, Action<Stream> writeBody, IProgress prog = null)
        {
            request.Method = "DELETE";
            SetDefaultAcceptType();
            await WriteBody(getInfo, writeBody);
            return (HttpWebResponse)await request.GetResponseAsync();
        }

        /// <summary>
        /// Perform a DELETE request, writing the body through a stream, and return the results as a stream.
        /// </summary>
        /// <param name="prog">Progress tracker (defaults to no progress tracking)</param>
        /// <returns>A stream that contains the response body, and an HTTP status code</returns>
        public async Task<StreamResult> Delete(IProgress prog = null)
        {
            request.Method = "DELETE";
            SetDefaultAcceptType();
            return (HttpWebResponse)await request.GetResponseAsync();
        }

        /// <summary>
        /// Perform a GET request and return the results as a stream of bytes
        /// </summary>
        /// <param name="prog">Progress tracker (defaults to no progress tracking)</param>
        /// <returns>A stream that contains the response body, and an HTTP status code</returns>
        public async Task<StreamResult> Get(IProgress prog = null)
        {
            request.Method = "GET";
            SetDefaultAcceptType();
            return (HttpWebResponse)await request.GetResponseAsync();
        }

        /// <summary>
        /// Perform a HEAD request and return the results as a stream of bytes
        /// </summary>
        /// <param name="prog">Progress tracker (defaults to no progress tracking)</param>
        /// <returns>A stream that contains the response body, and an HTTP status code</returns>
        public async Task<StreamResult> Head(IProgress prog = null)
        {
            request.Method = "HEAD";
            return (HttpWebResponse)await request.GetResponseAsync();
        }
    }
}
