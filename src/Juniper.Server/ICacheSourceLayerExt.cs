using System.IO;
using System.Net;
using System.Threading.Tasks;

using Juniper.IO;

namespace Juniper.HTTP.Server
{
    public static class ICacheSourceLayerExt
    {
        public static async Task ProxyAsync(
            this ICacheSourceLayer layer,
            HttpListenerResponse response,
            ContentReference fileRef)
        {
            if (layer is null)
            {
                throw new System.ArgumentNullException(nameof(layer));
            }

            var stream = await layer
                .GetStreamAsync(fileRef, null)
                .ConfigureAwait(false);
            await stream
                .ProxyAsync(response)
                .ConfigureAwait(false);
        }

        public static Task ProxyAsync(
            this ICacheSourceLayer layer,
            HttpListenerContext context,
            ContentReference fileRef)
        {
            if (context is null)
            {
                throw new System.ArgumentNullException(nameof(context));
            }

            return layer.ProxyAsync(context.Response, fileRef);
        }

        public static async Task ProxyAsync(this StreamSource source, HttpListenerResponse response)
        {
            if (source is null)
            {
                throw new System.ArgumentNullException(nameof(source));
            }

            if (response is null)
            {
                throw new System.ArgumentNullException(nameof(response));
            }

            var stream = await source
                .GetStreamAsync()
                .ConfigureAwait(false);
            response.ContentType = source.ContentType;
            await stream
                .ProxyAsync(response)
                .ConfigureAwait(false);
        }

        public static Task ProxyAsync(this StreamSource source, HttpListenerContext context)
        {
            if (context is null)
            {
                throw new System.ArgumentNullException(nameof(context));
            }

            return source.ProxyAsync(context.Response);
        }
    }
}
