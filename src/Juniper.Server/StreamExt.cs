using System.Net;
using System.Threading.Tasks;

using Juniper.HTTP.Server;

namespace System.IO
{
    public static class StreamExt
    {

        public static async Task ProxyAsync(this Stream stream, HttpListenerResponse response)
        {
            if (response is null)
            {
                throw new ArgumentNullException(nameof(response));
            }
            if (stream is null)
            {
                response.ContentType = string.Empty;
                response.SetStatus(HttpStatusCode.NotFound);
            }
            else
            {
                using (stream)
                {
                    response.SetStatus(HttpStatusCode.Continue);
                    await stream
                        .CopyToAsync(response.OutputStream)
                        .ConfigureAwait(false);
                }
            }
        }

        public static Task ProxyAsync(this Stream stream, HttpListenerContext context)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return stream.ProxyAsync(context.Response);
        }
    }
}
