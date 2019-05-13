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
    public static class Requester
    {
        /// <summary>
        /// Create the basic request object.
        /// </summary>
        /// <param name="method">GET/PUT/POST/etc.</param>
        /// <param name="url"></param>
        /// <param name="userName">Basic HTTP authentication user name.</param>
        /// <param name="password">Basic HTTP authentication user password.</param>
        /// <param name="acceptType">The MIME type of the expected response.</param>
        /// <returns>A web request object</returns>
        private static HttpWebRequest MakeRequest(string method, string url, string userName, string password, string acceptType)
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
            using (var response = (HttpWebResponse)await request.GetResponseAsync())
            {
                if (response.ContentLength == 0)
                {
                    return new StreamResult(
                        response.StatusCode,
                        null,
                        null);
                }
                else
                {
                    return new StreamResult(
                        response.StatusCode,
                        response.ContentType,
                        new ProgressStream(response.GetResponseStream(), response.ContentLength, prog));
                }
            }
        }

        /// <summary>
        /// Perform a POST request, writing the body through a stream, and return the results as a stream.
        /// </summary>
        /// <param name="url">The URL to request</param>
        /// <param name="userName">Basic HTTP authentication user name.</param>
        /// <param name="password">Basic HTTP authentication user password.</param>
        /// <param name="writeBody">A callback function for writing the body to a stream</param>
        /// <param name="acceptType">The MIME of the expected response</param>
        /// <param name="prog">Progress tracker (defaults to no progress tracking)</param>
        /// <returns>A stream that contains the response body, and an HTTP status code</returns>
        public static async Task<StreamResult> PostStream(string url, string userName, string password, Func<Stream, BodyInfo> writeBody, string acceptType, IProgress prog = null)
        {
            var request = MakeRequest("POST", url, userName, password, acceptType);
            using (var stream = await request.GetRequestStreamAsync())
            {
                var contentInfo = writeBody(stream);

                if (contentInfo.Length > 0)
                {
                    request.ContentLength = contentInfo.Length;

                    if (!string.IsNullOrEmpty(contentInfo.MIMEType))
                    {
                        request.ContentType = contentInfo.MIMEType;
                    }
                }
            }

            return await HandleResponse(request, prog);
        }

        /// <summary>
        /// Perform a POST request, writing the body through a stream, and return the results as a stream.
        /// </summary>
        /// <param name="url">The URL to request</param>
        /// <param name="userName">Basic HTTP authentication user name.</param>
        /// <param name="password">Basic HTTP authentication user password.</param>
        /// <param name="writeBody">A callback function for writing the body to a stream</param>
        /// <param name="prog">Progress tracker (defaults to no progress tracking)</param>
        /// <returns>A stream that contains the response body, and an HTTP status code</returns>
        public static Task<StreamResult> PostStream(string url, string userName, string password, Func<Stream, BodyInfo> writeBody, IProgress prog = null)
        {
            return PostStream(url, userName, password, writeBody, "application/octet-stream", prog);
        }

        /// <summary>
        /// Perform a POST request, writing the body through a stream, and return the results as a stream.
        /// </summary>
        /// <param name="url">The URL to request</param>
        /// <param name="writeBody">A callback function for writing the body to a stream</param>
        /// <param name="acceptType">The MIME of the expected response</param>
        /// <param name="prog">Progress tracker (defaults to no progress tracking)</param>
        /// <returns>A stream that contains the response body, and an HTTP status code</returns>
        public static Task<StreamResult> PostStream(string url, Func<Stream, BodyInfo> writeBody, string acceptType, IProgress prog = null)
        {
            return PostStream(url, null, null, writeBody, acceptType, prog);
        }

        /// <summary>
        /// Perform a POST request, writing the body through a stream, and return the results as a stream.
        /// </summary>
        /// <param name="url">The URL to request</param>
        /// <param name="contentLength">The total length of the body that needs to be written</param>
        /// <param name="contentType">The MIME type of both the request and response body</param>
        /// <param name="writeBody">A callback function for writing the body to a stream</param>
        /// <param name="prog">Progress tracker (defaults to no progress tracking)</param>
        /// <returns>A stream that contains the response body, and an HTTP status code</returns>
        public static Task<StreamResult> PostStream(string url, Func<Stream, BodyInfo> writeBody, IProgress prog = null)
        {
            return PostStream(url, null, null, writeBody, "application/octet-stream", prog);
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
            var request = MakeRequest("GET", url, userName, password, acceptType);
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
            return GetStream(url, null, null, "application/octet-stream", prog);
        }
    }
}
