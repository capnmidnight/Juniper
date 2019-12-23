using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Juniper.HTTP
{
    public static class HttpListenerResponseExt
    {
        public static void Error(this HttpListenerResponse response, HttpStatusCode code, string message)
        {
            response.SetStatus(code);

            using (var writer = new StreamWriter(response.OutputStream))
            {
                writer.WriteLine(message);
            }
        }

        public static void Redirect(this HttpListenerResponse response, string filename)
        {
            response.AddHeader("Location", filename);
            response.SetStatus(HttpStatusCode.MovedPermanently);
        }

        public static void SendFile(this HttpListenerResponse response, FileInfo file)
        {
            using (var input = file.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                SendStream(response, (MediaType)file, input);
            }
        }

        public static async Task SendFileAsync(this HttpListenerResponse response, FileInfo file)
        {
            using (var input = file.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                await SendStreamAsync(response, (MediaType)file, input).ConfigureAwait(false);
            }
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
            response.Prepare(contentType, input.Length);
            input.CopyTo(response.OutputStream);
        }

        public static Task SendStreamAsync(this HttpListenerResponse response, MediaType contentType, FileStream input)
        {
            response.Prepare(contentType, input.Length);
            return input.CopyToAsync(response.OutputStream);
        }

        public static void SendBytes(this HttpListenerResponse response, MediaType contentType, byte[] data)
        {
            response.Prepare(contentType, data.Length);
            response.OutputStream.Write(data, 0, data.Length);
        }

        public static Task SendBytesAsync(this HttpListenerResponse response, MediaType contentType, byte[] data)
        {
            response.Prepare(contentType, data.Length);
            return response.OutputStream.WriteAsync(data, 0, data.Length);
        }

        public static void SetStatus(this HttpListenerResponse response, HttpStatusCode code)
        {
            response.StatusCode = (int)code;
        }
    }
}
