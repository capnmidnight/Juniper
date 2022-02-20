using System.Net;

using Microsoft.AspNetCore.Mvc;

namespace Juniper.HTTP
{
    public class FileInfoResult : IActionResult
    {
        private readonly string contentType;
        private readonly string fileName;
        private readonly int cacheTime;
        private readonly long size;

        /// <summary>
        /// Creates a new ActionResult that sends along the file metadata.
        /// </summary>
        /// <param name="db">The Entity Framework database context through which the query will be made.</param>
        /// <param name="size">The size of the file that will be sent. This should be retrieved separately.</param>
        /// <param name="contentType">The content type of the file that will be sent. This should be retrieved separately.</param>
        /// <param name="fileName">The name of the file that will be sent. This should be retrieved separately.</param>
        /// <param name="cacheTime">The number of seconds to tell the client to cache the result.</param>
        public FileInfoResult(long size, string contentType, string fileName, int cacheTime)
            : base()
        {
            var type = MediaType.Parse(contentType);
            this.contentType = contentType;
            this.fileName = fileName?.AddExtension(type);
            this.cacheTime = cacheTime;
            this.size = size;
        }

        public FileInfoResult(long size, string contentType, string fileName)
            : this(size, contentType, fileName, 0)
        { }

        /// <summary>
        /// Performs the stream operation.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task ExecuteResultAsync(ActionContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var response = context.HttpContext.Response;
            response.StatusCode = (int)HttpStatusCode.OK;
            response.ContentType = contentType;
            response.ContentLength = size;
            if (!string.IsNullOrEmpty(fileName))
            {
                response.Headers["Content-Disposition"] = $"attachment; filename=\"{WebUtility.UrlEncode(fileName)}\"";
            }

            if (cacheTime > 0)
            {
                response.Headers["Cache-Control"] = $"public,max-age={cacheTime}";
            }

            try
            {
                await WriteBody(response);
            }
            catch (Exception)
            {
                response.StatusCode = (int)HttpStatusCode.NotFound;
            }
        }

        protected virtual Task WriteBody(HttpResponse response)
        {
            return Task.CompletedTask;
        }
    }
}