using System;
using System.IO;
using System.Net;

namespace Juniper.HTTP
{
    public class Response : IDisposable
    {
        public readonly HttpStatusCode StatusCode;

        public readonly string ContentType;

        public readonly long ContentLength;

        public readonly Stream Content;

        private Response(HttpStatusCode statusCode, string contentType, long contentLength, Stream content)
        {
            StatusCode = statusCode;
            ContentType = contentType;
            ContentLength = contentLength;
            Content = content;
        }

        public Response(string contentType, string path)
        {
            var fileInfo = new FileInfo(path);
            StatusCode = fileInfo.Exists ? HttpStatusCode.OK : HttpStatusCode.NotFound;
            if(StatusCode == HttpStatusCode.OK)
            {
                ContentType = contentType;
                ContentLength = fileInfo.Length;
                Content = fileInfo.OpenRead();
            }
        }

        public Response(string contentType, long contentLength, Stream content)
            : this(HttpStatusCode.OK, contentType, contentLength, content)
        {

        }

        public Response(string contentType, Stream content)
            : this(contentType, 0, content)
        {

        }

        public Response(HttpWebResponse response)
            : this(
                  response.StatusCode,
                  response.ContentType,
                  response.ContentLength,
                  response.GetResponseStream())
        { 
        }

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Content.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
