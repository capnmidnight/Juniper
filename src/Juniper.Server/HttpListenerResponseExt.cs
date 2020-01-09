using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Juniper.HTTP.Server
{
    public static class HttpListenerResponseExt
    {
        public static void Redirect(this HttpListenerResponse response, string filename)
        {
            if (response is null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            response.AddHeader("Location", filename);
            response.SetStatus(HttpStatusCode.MovedPermanently);
        }

        public static void SendFile(this HttpListenerResponse response, FileInfo file)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            using var input = file.OpenRead();
            response.SendStream((MediaType)file, input);
        }

        public static async Task SendFileAsync(this HttpListenerResponse response, FileInfo file)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            using var input = file.OpenRead();
            await response.SendStreamAsync((MediaType)file, input).ConfigureAwait(false);
        }

        private static void Prepare(this HttpListenerResponse response, MediaType contentType, long length)
        {
            response.SetStatus(HttpStatusCode.OK);
            response.ContentType = contentType;
            response.ContentLength64 = length;
            response.AddHeader("Date", DateTime.Now.ToString("r", CultureInfo.InvariantCulture));
        }

        public static void SendStream(this HttpListenerResponse response, MediaType contentType, FileStream input)
        {
            if (response is null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            if (input is null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            response.Prepare(contentType, input.Length);
            input.CopyTo(response.OutputStream);
        }

        public static Task SendStreamAsync(this HttpListenerResponse response, MediaType contentType, FileStream input)
        {
            if (response is null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            if (input is null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            response.Prepare(contentType, input.Length);
            return input.CopyToAsync(response.OutputStream);
        }

        public static void SendBytes(this HttpListenerResponse response, MediaType contentType, byte[] data)
        {
            if (response is null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            response.Prepare(contentType, data.Length);
            response.OutputStream.Write(data, 0, data.Length);
        }

        public static Task SendBytesAsync(this HttpListenerResponse response, MediaType contentType, byte[] data)
        {
            if (response is null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            response.Prepare(contentType, data.Length);
            return response.OutputStream.WriteAsync(data, 0, data.Length);
        }

        public static void SetStatus(this HttpListenerResponse response, HttpStatusCode code)
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
