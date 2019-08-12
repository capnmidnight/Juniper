using System.Net;
using System.Text;
using System.Threading.Tasks;

using Juniper.HTTP;
using Juniper.Progress;
using Juniper.Streams;

namespace System.IO
{
    /// <summary>
    /// Extension methods for reading Streams with progress tracking.
    /// </summary>
    public static class StreamExt
    {
        /// <summary>
        /// Writes a series of bytes out to a stream.
        /// </summary>
        /// <param name="value">The bytes to write.</param>
        /// <param name="prog">A progress tracker. Defaults to null (no progress tracking).</param>
        /// <returns>A callback function that can be used to write the text when a stream becomes available.</returns>
        private static Action<Stream> BytesWriter(this byte[] value, IProgress prog)
        {
            return (stream) =>
            {
                var progStream = new ProgressStream(stream, value.Length, prog);
                progStream.Write(value, 0, value.Length);
            };
        }

        private static Func<BodyInfo> BytesInfoGetter(this byte[] value, string type)
        {
            return () => new BodyInfo(type ?? "application/octet-stream", value.Length);
        }

        /// <summary>
        /// Writes bytes out to a stream.
        /// </summary>
        /// <param name="value">The text to write.</param>
        /// <param name="prog">A progress tracker. Defaults to null (no progress tracking).</param>
        /// <returns>A callback function that can be used to write the text when a stream becomes available.</returns>
        public static Task<HttpWebResponse> Write(this byte[] bytes, Func<Func<BodyInfo>, Action<Stream>, Task<HttpWebResponse>> writer, string type = null, IProgress prog = null)
        {
            var infoGetter = bytes.BytesInfoGetter(type ?? "text/plain");
            var bodyWriter = bytes.BytesWriter(prog);
            return writer(infoGetter, bodyWriter);
        }

        /// <summary>
        /// Writes text out to a stream.
        /// </summary>
        /// <param name="value">The text to write.</param>
        /// <param name="prog">A progress tracker. Defaults to null (no progress tracking).</param>
        /// <returns>A callback function that can be used to write the text when a stream becomes available.</returns>
        public static Task<HttpWebResponse> Write(this string value, Func<Func<BodyInfo>, Action<Stream>, Task<HttpWebResponse>> writer, string type = null, IProgress prog = null)
        {
            var bytes = Encoding.Unicode.GetBytes(value);
            return bytes.Write(writer, type ?? "text/plain", prog);
        }

        /// <summary>
        /// Writes a file out to a stream.
        /// </summary>
        /// <param name="file">The text to write.</param>
        /// <param name="prog">A progress tracker. Defaults to null (no progress tracking).</param>
        /// <returns>A callback function that can be used to write the text when a stream becomes available.</returns>
        public static Task<HttpWebResponse> Write(this FileInfo file, Func<Func<BodyInfo>, Action<Stream>, Task<HttpWebResponse>> writer, string type, IProgress prog = null)
        {
            BodyInfo infoGetter()
            {
                return new BodyInfo(type, file.Length);
            }

            void bodyWriter(Stream outStream)
            {
                using (var inStream = file.OpenRead())
                {
                    var progress = new ProgressStream(inStream, file.Length, prog);
                    progress.CopyTo(outStream);
                }
            }

            return writer(infoGetter, bodyWriter);
        }
    }
}