using System;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

namespace Juniper.HTTP
{
    /// <summary>
    /// Takes a chunk of JSON text and sends it down the pipe with the Content-Length set.
    /// </summary>
    public class JsonBlobResult : IActionResult
    {
        private static readonly JsonSerializerOptions DefaultOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        private readonly string json;

        public JsonBlobResult(string json)
        {
            this.json = json;
        }

        public static JsonBlobResult Create<T>(T obj, JsonSerializerOptions options = null)
        {
            return new JsonBlobResult(JsonSerializer.Serialize(obj, options ?? DefaultOptions));
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
