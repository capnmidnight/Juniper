using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Juniper.IO;

namespace Juniper.HTTP.Server
{
    public static class HttpListenerResponseExt
    {
        public static void Redirect(
            this HttpListenerResponse response,
            string fileName)
        {
            if (response is null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            if (fileName is null)
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            response.RedirectLocation = fileName;
            response.SetStatus(HttpStatusCode.Redirect);
        }

        public static async Task SendContentAsync(
            this HttpListenerResponse response,
            ICacheSourceLayer layer,
            ContentReference fileRef)
        {
            if (layer is null)
            {
                throw new ArgumentNullException(nameof(layer));
            }

            if (fileRef is null)
            {
                throw new ArgumentNullException(nameof(fileRef));
            }

            using var stream = await layer
                .GetStreamAsync(fileRef, null)
                .ConfigureAwait(false);

            await response
                .SendStreamAsync(fileRef.ContentType, stream)
                .ConfigureAwait(false);
        }

        public static async Task SendContentAsync(
            this HttpListenerResponse response,
            StreamSource source)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            using var stream = await source
                .GetStreamAsync()
                .ConfigureAwait(false);

            await response.SendStreamAsync(source.ContentType, stream)
                .ConfigureAwait(false);
        }

        public static async Task SendFileAsync(
            this HttpListenerResponse response,
            FileInfo file)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (!file.Exists)
            {
                throw new FileNotFoundException("File not found: " + file.FullName, file.FullName);
            }

            using var input = file.OpenRead();
            await response.SendStreamAsync((MediaType)file, input)
                .ConfigureAwait(false);
        }

        public static Task SendTextAsync(
            this HttpListenerResponse response,
            string text)
        {
            return response.SendTextAsync(MediaType.Text.Plain, text);
        }

        public static Task SendTextAsync(
            this HttpListenerResponse response,
            MediaType contentType,
            string text)
        {
            if (text is null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            return response.SendBytesAsync(contentType, Encoding.UTF8.GetBytes(text));
        }

        public static Task SendBytesAsync(
            this HttpListenerResponse response,
            MediaType contentType,
            byte[] data)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            using var mem = new MemoryStream(data);
            return response.SendStreamAsync(contentType, mem);
        }

        public static Task SendStreamAsync(
            this HttpListenerResponse response,
            MediaType contentType,
            Stream input)
        {
            if (response is null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            if (contentType is null)
            {
                throw new ArgumentNullException(nameof(contentType));
            }

            if (input is null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            response.ContentType = contentType;
            response.ContentLength64 = input.Length;
            response.AddHeader("Date", DateTime.Now.ToString("r", CultureInfo.InvariantCulture));
            return input.CopyToAsync(response.OutputStream);
        }

        public static void SetStatus(
            this HttpListenerResponse response,
            HttpStatusCode code)
        {
            if (response is null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            response.StatusCode = (int)code;
        }

        public static HttpStatusCode GetStatus(this HttpListenerResponse response)
        {
            if (response is null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            return (HttpStatusCode)response.StatusCode;
        }
    }
}
