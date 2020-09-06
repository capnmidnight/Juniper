using Microsoft.AspNetCore.Mvc;

using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Juniper.HTTP.Server
{
    /// <summary>
    /// Takes a chunk of JSON text and sends it down the pipe with the Content-Length set.
    /// </summary>
    public class JsonBlobResult : IActionResult
    {
        private readonly string json;

        public JsonBlobResult(string json)
        {
            this.json = json;
        }

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

            var bytes = Encoding.UTF8.GetBytes(json);
            var response = context.HttpContext.Response;
            response.ContentType = MediaType.Application.Json;
            response.ContentLength = bytes.Length;
            response.StatusCode = (int)HttpStatusCode.OK;
            await response.Body.WriteAsync(bytes)
                .ConfigureAwait(false);
        }
    }
}
