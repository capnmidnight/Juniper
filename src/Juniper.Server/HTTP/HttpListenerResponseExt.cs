using Juniper.IO;

using System.Globalization;
using System.Net;
using System.Text;

namespace Juniper.HTTP
{
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

            response.SetFileName(source.ContentType, source.FileName);

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

            MediaType type = MediaType.Application_Octet_Stream;
            var types = MediaType.GuessByFile(file);
            if (types.Count > 0)
            {
                type = types[0];
            }

            response.SetFileName(type, file.Name);

            using var input = file.OpenRead();

            await response.SendStreamAsync(type, input)
                .ConfigureAwait(false);
        }

        public static Task SendTextAsync(
            this HttpListenerResponse response,
            string text)
        {
            return response.SendTextAsync(MediaType.Text_Plain, text);
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
            response.ContentLength64 += input.Length;
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

        public static void SetFileName(this HttpListenerResponse response, MediaType type, string fileName)
        {
            if (response is null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            if (type is not null
                && type != MediaType.Text.AnyText
                && type != MediaType.Image.AnyImage
                && type != MediaType.Application_JavaScript
                && fileName is not null)
            {
                response.Headers.Add("Content-Disposition", $"attachment; filename={fileName}");
            }
        }
    }
}
