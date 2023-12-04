using Juniper.Caching;
using System.Globalization;
using System.Net;
using System.Text;

namespace Juniper.HTTP;

public static class HttpListenerResponseExt
{

    public static async Task SendContentAsync(
        this HttpListenerResponse response,
        AbstractStreamSource source)
    {
        if (response is null)
        {
            throw new ArgumentNullException(nameof(response));
        }

        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (source.FileName is not null)
        {
            response.SetFileName(source.ContentType, source.FileName);
        }

        using var stream = await source
            .GetStreamAsync()
            .ConfigureAwait(false);

        await response.SendStreamAsync(source.ContentType, stream)
            .ConfigureAwait(false);
    }
}
