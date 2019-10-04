using System.Collections.Generic;
using System.Net;

namespace Juniper.Google
{
    public static class StringExt
    {
        private static readonly Dictionary<string, HttpStatusCode> CODE_MAP = new Dictionary<string, HttpStatusCode>
        {
            { "OK", HttpStatusCode.OK },
            { "ZERO_RESULTS", HttpStatusCode.NoContent },
            { "NOT_FOUND", HttpStatusCode.NotFound },
            { "OVER_QUERY_LIMIT", (HttpStatusCode)429 },
            { "OVER_DAILY_LIMIT", (HttpStatusCode)429 },
            { "REQUEST_DENIED", HttpStatusCode.Forbidden },
            { "INVALID_REQUEST", HttpStatusCode.BadRequest },
            { "UNKOWN_ERROR", HttpStatusCode.InternalServerError }
        };

        private static readonly Dictionary<HttpStatusCode, string> REVERSE_CODE_MAP = CODE_MAP.Invert();

        public static HttpStatusCode MapToStatusCode(this string googleStatus)
        {
            return CODE_MAP.ContainsKey(googleStatus)
                ? CODE_MAP[googleStatus]
                : HttpStatusCode.NotImplemented;
        }

        public static string ToGoogleString(this HttpStatusCode code)
        {
            return REVERSE_CODE_MAP.ContainsKey(code)
                ? REVERSE_CODE_MAP[code]
                : "Unknown";
        }
    }
}