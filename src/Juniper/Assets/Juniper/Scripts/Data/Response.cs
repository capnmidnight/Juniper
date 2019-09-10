using System;
using System.IO;
using System.Net;

using Juniper.Progress;
using Juniper.Streams;

namespace Juniper.HTTP
{
    public class Response : IDisposable
    {
        public readonly HttpStatusCode StatusCode;

        public readonly string ContentType;

        public readonly long ContentLength;

        public readonly Stream Content;

        public Response(Stream content, HttpStatusCode statusCode, string contentType, long contentLength, IProgress prog)
        {
            StatusCode = statusCode;
            ContentType = contentType;
            ContentLength = contentLength;
            if (prog != null)
            {
                Content = new ProgressStream(content, contentLength, prog);
            }
            else
            {
                Content = content;
            }
        }

        public Response(FileInfo file, string contentType, IProgress prog)
            : this(file.OpenRead(), file.Exists ? HttpStatusCode.OK : HttpStatusCode.NotFound, contentType, file.Length, prog)
        { }

        public Response(string path, string contentType, IProgress prog)
            : this(new FileInfo(path), contentType, prog)
        { }

        public Response(HttpWebResponse response, IProgress prog)
            : this(response.GetResponseStream(), response.StatusCode, response.ContentType, response.ContentLength, prog)
        { }

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