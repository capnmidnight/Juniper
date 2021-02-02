using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

namespace Juniper.HTTP
{
    /// <summary>
    /// Copy a stream to the response
    /// </summary>
    public class StreamResult : IActionResult
    {
        private readonly Func<Task<Stream>> getStream;
        private readonly string contentType;
        private readonly string fileName;

        /// <summary>
        /// Creates a new ActionResult that can write a stream the the response.
        /// </summary>
        /// <param name="contentType">The content type of the stream that will be written. This should be retrieved separately.</param>
        /// <param name="fileName">The name of the file that will be sent. This should be retrieved separately.</param>
        /// <param name="getStream">A callback function to construct the stream to write.</param>
        public StreamResult(string contentType, string fileName, Func<Task<Stream>> getStream)
        {
            this.contentType = contentType;
            this.fileName = fileName;
            this.getStream = getStream;
        }

        /// <summary>
        /// Performs the database query.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual async Task ExecuteResultAsync(ActionContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var response = context.HttpContext.Response;
            try
            {
                using var stream = await getStream()
                    .ConfigureAwait(false);
                response.StatusCode = (int)HttpStatusCode.OK;
                response.ContentType = contentType;
                response.ContentLength = stream.Length;
                response.Headers["Content-Disposition"] = $"attachment; filename=\"{fileName}\"";

                await stream.CopyToAsync(context.HttpContext.Response.Body)
                    .ConfigureAwait(false);
            }
            catch (EndOfStreamException)
            {
                response.StatusCode = (int)HttpStatusCode.NotFound;
            }
        }
    }
}
