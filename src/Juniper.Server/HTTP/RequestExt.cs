using System.Linq;

using Microsoft.AspNetCore.Http;

namespace Juniper.HTTP
{
    public static class RequestExt
    {
        public static bool Accepts(this HttpRequest request, MediaType type)
        {
            return request.Headers.ContainsKey("Accept")
                && request.Headers["Accept"].Any(type.Matches);
        }
    }
}