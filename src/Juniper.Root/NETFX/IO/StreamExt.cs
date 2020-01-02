using System.Net;
using System.Threading.Tasks;

namespace System.IO
{
    public static class StreamExt
    {
        public static void CopyTo(this Stream inStream, FileInfo outFile)
        {
            using var outStream = outFile.Create();
            inStream.CopyTo(outStream);
        }

        public static async Task CopyToAsync(this Stream inStream, FileInfo outFile)
        {
            using var outStream = outFile.Create();
            await inStream
.CopyToAsync(outStream)
.ConfigureAwait(false);
        }

        public static void CopyTo(this Stream inStream, string outFileName)
        {
            inStream.CopyTo(new FileInfo(outFileName));
        }

        public static Task CopyToAsync(this Stream inStream, string outFileName)
        {
            return inStream.CopyToAsync(new FileInfo(outFileName));
        }

        public static void CopyTo(this FileInfo inFile, Stream outStream)
        {
            using var inStream = inFile.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
            inStream.CopyTo(outStream);
        }

        public static async Task CopyToAsync(this FileInfo inFile, Stream outStream)
        {
            using var inStream = inFile.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
            await inStream
.CopyToAsync(outStream)
.ConfigureAwait(false);
        }

        public static void CopyTo(this FileInfo inFile, FileInfo outFile)
        {
            inFile.CopyTo(outFile.FullName, true);
        }

        public static async Task ProxyAsync(this Stream stream, HttpListenerResponse response)
        {
            if (stream is null)
            {
                response.ContentType = string.Empty;
                response.StatusCode = 404;
            }
            else
            {
                using (stream)
                {
                    response.StatusCode = 200;
                    await stream
                        .CopyToAsync(response.OutputStream)
                        .ConfigureAwait(false);
                }
            }
        }

        public static Task ProxyAsync(this Stream stream, HttpListenerContext context)
        {
            return stream.ProxyAsync(context.Response);
        }
    }
}