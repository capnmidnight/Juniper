using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

namespace Juniper.HTTP
{
    public abstract class AbstractStreamResult : IActionResult
    {
        private readonly string contentType;
        private readonly string fileName;
        private readonly int cacheTime;

        protected AbstractStreamResult(string contentType, string fileName, int cacheTime)
        {
            var type = MediaType.Parse(contentType);
            this.contentType = contentType;
            this.fileName = fileName?.AddExtension(type);
            this.cacheTime = cacheTime;
        }

        protected abstract Task ExecuteAsync(Func<Stream, Task> writeStream);

        protected abstract long GetStreamLength(Stream stream);

        async Task WriteStreamAsync(ActionContext context, Stream stream)
        {
            var response = context.HttpContext.Response;
            response.StatusCode = (int)HttpStatusCode.OK;
            response.ContentType = contentType;
            response.ContentLength = GetStreamLength(stream);
            if (!string.IsNullOrEmpty(fileName))
            {
                response.Headers["Content-Disposition"] = $"attachment; filename=\"{WebUtility.UrlEncode(fileName)}\"";
            }

            if (cacheTime > 0)
            {
                response.Headers["Cache-Control"] = $"public,max-age={cacheTime}";
            }

            await stream.CopyToAsync(response.Body)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Performs the stream operation.
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
                await ExecuteAsync((stream) =>
                    WriteStreamAsync(context, stream))
                    .ConfigureAwait(false);
            }
            catch (Exception)
            {
                response.StatusCode = (int)HttpStatusCode.NotFound;
            }
        }
    }
}
