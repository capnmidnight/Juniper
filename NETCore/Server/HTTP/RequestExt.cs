using Juniper;

using Microsoft.Net.Http.Headers;

namespace Microsoft.AspNetCore.Http;

public static class RequestExt
{
    public static bool Accepts(this HttpRequest request, MediaType type)
    {
        return request.Headers.TryGetValue(HeaderNames.Accept, out var value)
            && value
            .ToArray()!
            .Join(",")
            .Split(",")
            .Any(type.Matches);
    }
}