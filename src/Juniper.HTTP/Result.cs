using System.Net;

namespace Juniper.HTTP
{
    public class Result<T>
    {
        public readonly HttpStatusCode Status;
        public readonly string MIMEType;
        public readonly T Value;

        public Result(HttpStatusCode status, string mime, T value)
        {
            Status = status;
            MIMEType = mime;
            Value = value;
        }
    }
}
