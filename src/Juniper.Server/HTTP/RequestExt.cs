using Juniper;

using Microsoft.Net.Http.Headers;

namespace Microsoft.AspNetCore.Http;

public static class RequestExt
{
    public static bool Accepts(this HttpRequest request, MediaType type)
    {
        return request.Headers.ContainsKey(HeaderNames.Accept)
            && request.Headers[HeaderNames.Accept].Any(type.Matches);
    }
}