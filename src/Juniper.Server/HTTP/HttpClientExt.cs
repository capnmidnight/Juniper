using Microsoft.AspNetCore.Mvc;

namespace System.Net.Http
{
    public static class HttpClientExt
    {
        public static async Task<IActionResult> ProxyAsync(this HttpClient http, string q, HttpRequest request, HttpResponse response)
        {
            var requestMessage = new HttpRequestMessage();
            var requestMethod = request.Method;
            if (!HttpMethods.IsGet(requestMethod) &&
                !HttpMethods.IsHead(requestMethod) &&
                !HttpMethods.IsDelete(requestMethod) &&
                !HttpMethods.IsTrace(requestMethod))
            {
                var streamContent = new StreamContent(request.Body);
                requestMessage.Content = streamContent;
            }

            // Copy the request headers
            foreach (var header in request.Headers)
            {
                if (!requestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray()) && requestMessage.Content != null)
                {
                    requestMessage.Content?.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
                }
            }

            var uri = new Uri(q);
            requestMessage.Headers.Host = uri.Authority;
            requestMessage.RequestUri = uri;
            requestMessage.Method = new HttpMethod(request.Method);

            var responseMessage = await http.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, request.HttpContext.RequestAborted);

            response.StatusCode = (int)responseMessage.StatusCode;
            foreach (var header in responseMessage.Headers)
            {
                response.Headers[header.Key] = header.Value.ToArray();
            }

            foreach (var header in responseMessage.Content.Headers)
            {
                response.Headers[header.Key] = header.Value.ToArray();
            }

            // SendAsync removes chunking from the response. This removes the header so it doesn't expect a chunked response.
            response.Headers.Remove("transfer-encoding");

            var responseStream = await responseMessage.Content.ReadAsStreamAsync();
            return new FileStreamResult(responseStream, responseMessage.Content.Headers.ContentType.MediaType)
            {
                EnableRangeProcessing = true
            };

        }
    }
}
