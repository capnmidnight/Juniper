using System.Net;
using System.Text.Json;

namespace Juniper.HTTP
{
    /// <summary>
    /// Takes a chunk of JSON text and sends it down the pipe with the Content-Length set.
    /// </summary>
    public class JsonBlobResult : ResultWithMessage
    {
        private static readonly JsonSerializerOptions DefaultOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        public JsonBlobResult(string json)
            : base(HttpStatusCode.OK, json, MediaType.Application.Json)
        {
        }

        public static JsonBlobResult Create<T>(T obj, JsonSerializerOptions options = null)
        {
            return new JsonBlobResult(JsonSerializer.Serialize(obj, options ?? DefaultOptions));
        }
    }
}
