using Microsoft.Net.Http.Headers;

namespace Juniper.HTTP
{
    public static class RequestExt
    {
        public static bool Accepts(this HttpRequest request, MediaType type)
        {
            return request.Headers.ContainsKey(HeaderNames.Accept)
                && request.Headers[HeaderNames.Accept].Any(type.Matches);
        }
    }
}