using System;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

namespace Juniper.HTTP
{
    /// <summary>
    /// An HTTP response with a status message
    /// </summary>
    public class ResultWithMessage : IActionResult
    {
        private readonly int statusCode;
        private readonly string message;
        private readonly MediaType mediaType;

        public ResultWithMessage(int statusCode, string message, MediaType mediaType)
        {
            this.statusCode = statusCode;
            this.message = message;
            this.mediaType = mediaType;
        }

        public ResultWithMessage(HttpStatusCode statusCode, string message, MediaType mediaType)
            : this((int)statusCode, message, mediaType) { }

        /// <summary>
        /// Performs the writing.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task ExecuteResultAsync(ActionContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var bytes = Encoding.UTF8.GetBytes(message);
            var response = context.HttpContext.Response;
            response.ContentType = mediaType;
            response.ContentLength = bytes.Length;
            response.StatusCode = statusCode;
            await response.Body.WriteAsync(bytes)
                .ConfigureAwait(false);
        }
    }
}
