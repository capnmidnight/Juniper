using Microsoft.AspNetCore.Http;

using System.Linq;

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