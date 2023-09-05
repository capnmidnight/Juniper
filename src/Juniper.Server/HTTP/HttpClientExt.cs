using Juniper;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace System.Net.Http;

public static class HttpClientExt
{
    private static readonly Dictionary<string, string[]> DisallowedHeaders = new()
    {
        { HttpProtocol.Http09, Array.Empty<string>() },
        { HttpProtocol.Http10, Array.Empty<string>() },
        { HttpProtocol.Http11, Array.Empty<string>() },
        {
            HttpProtocol.Http2,
            new[]{
                HeaderNames.Connection,
                HeaderNames.ProxyConnection,
                HeaderNames.TransferEncoding,
                HeaderNames.KeepAlive,
                HeaderNames.Upgrade
            }
        },
        {
            HttpProtocol.Http3,
            new[]{
                HeaderNames.Connection,
                HeaderNames.ProxyConnection,
                HeaderNames.TransferEncoding,
                HeaderNames.KeepAlive,
                HeaderNames.Upgrade
            }
        }
    };

    private static bool ForwardHeader(string protocol, string name)
    {
        return !DisallowedHeaders.ContainsKey(protocol)
            || !DisallowedHeaders[protocol].Contains(name);
    }

    private static HttpRequestMessage MakeRequest(HttpRequest request, Uri uri)
    {
        var requestMethod = request.Method;
        var requestMessage = new HttpRequestMessage(new HttpMethod(requestMethod), uri);
        if (HttpMethods.IsConnect(requestMethod)
            || HttpMethods.IsOptions(requestMethod)
            || HttpMethods.IsPatch(requestMethod)
            || HttpMethods.IsPost(requestMethod)
            || HttpMethods.IsPut(requestMethod))
        {
            requestMessage.Content = new StreamContent(request.Body);
        }

        foreach (var header in request.Headers)
        {
            var key = header.Key;
            if (ForwardHeader(request.Protocol, key))
            {
                var value = header.Value.ToArray();
                if (!requestMessage.Headers.TryAddWithoutValidation(key, value)
                    && requestMessage.Content is not null)
                {
                    requestMessage.Content.Headers.TryAddWithoutValidation(key, value);
                }
            }
        }

        requestMessage.Headers.Host = uri.Authority;

        return requestMessage;
    }

    public static async Task<IActionResult> ProxyAsync(this HttpClient http, string q, HttpRequest request, HttpResponse response)
    {
        var uri = new Uri(q);
        var requestMessage = MakeRequest(request, uri);
        var responseMessage = await http.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, request.HttpContext.RequestAborted);
        var redirectCount = 3;
        while (responseMessage.StatusCode == HttpStatusCode.Redirect
            && redirectCount > 0)
        {
            --redirectCount;
            requestMessage.Dispose();
            responseMessage.Dispose();

            if (responseMessage.Headers.Location is null)
            {
                throw new Exception("Couldn't find redirect location");
            }

            requestMessage = MakeRequest(request, responseMessage.Headers.Location);
            responseMessage = await http.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, request.HttpContext.RequestAborted);
        }
        requestMessage.Dispose();

        if (redirectCount == 0 && responseMessage.StatusCode == HttpStatusCode.Redirect)
        {
            throw new Exception("Too many redirects");
        }

        response.StatusCode = (int)responseMessage.StatusCode;
        foreach (var header in responseMessage.Headers)
        {
            var key = header.Key;
            if (ForwardHeader(request.Protocol, key))
            {
                response.Headers[key] = header.Value.ToArray();
            }
        }

        foreach (var header in responseMessage.Content.Headers)
        {
            var key = header.Key;
            if (ForwardHeader(request.Protocol, key))
            {
                response.Headers[key] = header.Value.ToArray();
            }
        }

        var responseStream = await responseMessage.Content.ReadAsStreamAsync();
        return new FileStreamResult(responseStream, responseMessage.Content.Headers.ContentType?.MediaType ?? MediaType.Application_Octet_Stream)
        {
            EnableRangeProcessing = true,
        };
    }
}
