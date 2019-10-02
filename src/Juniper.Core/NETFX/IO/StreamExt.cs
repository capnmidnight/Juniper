using System.Net;
using System.Threading.Tasks;
using Juniper.IO;
using Juniper.Progress;

namespace System.IO
{
    public static class StreamExt
    {
        public static void CopyTo(this Stream inStream, FileInfo outFile)
        {
            using (var outStream = outFile.Create())
            {
                inStream.CopyTo(outStream);
            }
        }

        public static void CopyTo(this Stream inStream, string outFileName)
        {
            inStream.CopyTo(new FileInfo(outFileName));
        }

        public static void CopyTo(this FileInfo inFile, Stream outStream)
        {
            using (var inStream = inFile.OpenRead())
            {
                inStream.CopyTo(outStream);
            }
        }

        public static void CopyTo(this FileInfo inFile, FileInfo outFile)
        {
            inFile.CopyTo(outFile.FullName, true);
        }

        public static async Task Proxy(this Stream stream, HttpListenerResponse response)
        {
            if (stream == null)
            {
                response.StatusCode = 404;
            }
            else
            {
                using (stream)
                {
                    response.StatusCode = 200;
                    await stream.CopyToAsync(response.OutputStream);
                }
            }
        }

        public static Task Proxy(this Stream stream, HttpListenerContext context)
        {
            return stream.Proxy(context.Response);
        }

        public static async Task Proxy(this Task<Stream> streamTask, HttpListenerResponse response)
        {
            var stream = await streamTask;
            if (stream == null)
            {
                response.StatusCode = 404;
                response.ContentType = string.Empty;
            }
            else
            {
                using (stream)
                {
                    response.StatusCode = 200;
                    await stream.CopyToAsync(response.OutputStream);
                }
            }
        }

        public static Task Proxy(this Task<Stream> streamTask, HttpListenerContext context)
        {
            return streamTask.Proxy(context.Response);
        }
    }
}