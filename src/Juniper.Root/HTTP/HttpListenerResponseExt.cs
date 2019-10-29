using System;
using System.IO;
using System.Net;

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
                response.SetStatus(HttpStatusCode.OK);
                response.ContentType = (MediaType)file ;
                response.ContentLength64 = input.Length;
                response.AddHeader("Date", DateTime.Now.ToString("r"));
                input.CopyTo(response.OutputStream);
            }
        }

        public static void SetStatus(this HttpListenerResponse response, HttpStatusCode code)
        {
            response.StatusCode = (int)code;
        }
    }
}
