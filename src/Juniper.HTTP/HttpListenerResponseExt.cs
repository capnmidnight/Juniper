using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Juniper.HTTP
{
    public static class HttpListenerResponseExt
    {
        private static readonly IDictionary<string, string> MIME = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase) {

        #region extension to MIME type list

            {".asf", "video/x-ms-asf"},
            {".asx", "video/x-ms-asf"},
            {".avi", "video/x-msvideo"},
            {".bin", "application/octet-stream"},
            {".cco", "application/x-cocoa"},
            {".crt", "application/x-x509-ca-cert"},
            {".css", "text/css"},
            {".deb", "application/octet-stream"},
            {".der", "application/x-x509-ca-cert"},
            {".dll", "application/octet-stream"},
            {".dmg", "application/octet-stream"},
            {".ear", "application/java-archive"},
            {".eot", "application/octet-stream"},
            {".exe", "application/octet-stream"},
            {".flv", "video/x-flv"},
            {".gif", "image/gif"},
            {".hqx", "application/mac-binhex40"},
            {".htc", "text/x-component"},
            {".htm", "text/html"},
            {".html", "text/html"},
            {".ico", "image/x-icon"},
            {".img", "application/octet-stream"},
            {".iso", "application/octet-stream"},
            {".jar", "application/java-archive"},
            {".jardiff", "application/x-java-archive-diff"},
            {".jng", "image/x-jng"},
            {".jnlp", "application/x-java-jnlp-file"},
            {".jpeg", "image/jpeg"},
            {".jpg", "image/jpeg"},
            {".js", "application/x-javascript"},
            {".mml", "text/mathml"},
            {".mng", "video/x-mng"},
            {".mov", "video/quicktime"},
            {".mp3", "audio/mpeg"},
            {".mpeg", "video/mpeg"},
            {".mpg", "video/mpeg"},
            {".msi", "application/octet-stream"},
            {".msm", "application/octet-stream"},
            {".msp", "application/octet-stream"},
            {".pdb", "application/x-pilot"},
            {".pdf", "application/pdf"},
            {".pem", "application/x-x509-ca-cert"},
            {".pl", "application/x-perl"},
            {".pm", "application/x-perl"},
            {".png", "image/png"},
            {".prc", "application/x-pilot"},
            {".ra", "audio/x-realaudio"},
            {".rar", "application/x-rar-compressed"},
            {".rpm", "application/x-redhat-package-manager"},
            {".rss", "text/xml"},
            {".run", "application/x-makeself"},
            {".sea", "application/x-sea"},
            {".shtml", "text/html"},
            {".sit", "application/x-stuffit"},
            {".swf", "application/x-shockwave-flash"},
            {".tcl", "application/x-tcl"},
            {".tk", "application/x-tcl"},
            {".txt", "text/plain"},
            {".war", "application/java-archive"},
            {".wbmp", "image/vnd.wap.wbmp"},
            {".wmv", "video/x-ms-wmv"},
            {".xml", "text/xml"},
            {".xpi", "application/x-xpinstall"},
            {".zip", "application/zip"},
        #endregion extension to MIME type list
        };

        public static void Redirect(this HttpListenerResponse response, string filename)
        {
            response.AddHeader("Location", filename);
            response.SetStatus(HttpStatusCode.MovedPermanently);
        }

        public static void SendFile(this HttpListenerResponse response, string filename)
        {
            using (Stream input = new FileStream(filename, FileMode.Open))
            {
                var ext = Path.GetExtension(filename);
                response.ContentType = MIME.ContainsKey(ext) ? MIME[ext] : "application/octet-stream";
                response.ContentLength64 = input.Length;
                response.AddHeader("Date", DateTime.Now.ToString("r"));

                var buffer = new byte[1024 * 16];
                int nbytes;
                while ((nbytes = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    response.OutputStream.Write(buffer, 0, nbytes);
                }
                input.Close();
            }
            response.SetStatus(HttpStatusCode.OK);
        }

        public static void Error(this HttpListenerResponse response, HttpStatusCode code, string message)
        {
            Console.Write(message);
            Console.Write(" ");
            Console.WriteLine(code);

            response.SetStatus(code);

            using (var writer = new StreamWriter(response.OutputStream))
            {
                writer.WriteLine(message);
            }
        }

        public static void SetStatus(this HttpListenerResponse response, HttpStatusCode code)
        {
            response.StatusCode = (int)code;
        }
    }
}