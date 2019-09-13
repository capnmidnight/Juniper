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
    public static class HttpStreamExt
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
            return () => new BodyInfo(type ?? MediaType.Application.Octet_Stream, value.Length);
        }

        /// <summary>
        /// Writes bytes out to a stream.
        /// </summary>
        /// <param name="value">The text to write.</param>
        /// <param name="prog">A progress tracker. Defaults to null (no progress tracking).</param>
        /// <returns>A callback function that can be used to write the text when a stream becomes available.</returns>
        public static Task<HttpWebResponse> Write(this byte[] bytes, Func<Func<BodyInfo>, Action<Stream>, Task<HttpWebResponse>> writer, string type, IProgress prog)
        {
            var infoGetter = bytes.BytesInfoGetter(type ?? MediaType.Text.Plain);
            var bodyWriter = bytes.BytesWriter(prog);
            return writer(infoGetter, bodyWriter);
        }

        public static Task<HttpWebResponse> Write(this byte[] bytes, Func<Func<BodyInfo>, Action<Stream>, Task<HttpWebResponse>> writer, string type)
        {
            return bytes.Write(writer, type, null);
        }

        public static Task<HttpWebResponse> Write(this byte[] bytes, Func<Func<BodyInfo>, Action<Stream>, Task<HttpWebResponse>> writer, IProgress prog)
        {
            return bytes.Write(writer, null, prog);
        }

        public static Task<HttpWebResponse> Write(this byte[] bytes, Func<Func<BodyInfo>, Action<Stream>, Task<HttpWebResponse>> writer)
        {
            return bytes.Write(writer, null, null);
        }

        /// <summary>
        /// Writes text out to a stream.
        /// </summary>
        /// <param name="value">The text to write.</param>
        /// <param name="prog">A progress tracker. Defaults to null (no progress tracking).</param>
        /// <returns>A callback function that can be used to write the text when a stream becomes available.</returns>
        public static Task<HttpWebResponse> Write(this string value, Func<Func<BodyInfo>, Action<Stream>, Task<HttpWebResponse>> writer, string type, IProgress prog)
        {
            var bytes = Encoding.Unicode.GetBytes(value);
            return bytes.Write(writer, type ?? MediaType.Text.Plain, prog);
        }

        public static Task<HttpWebResponse> Write(this string value, Func<Func<BodyInfo>, Action<Stream>, Task<HttpWebResponse>> writer, string type)
        {
            return value.Write(writer, type, null);
        }

        public static Task<HttpWebResponse> Write(this string value, Func<Func<BodyInfo>, Action<Stream>, Task<HttpWebResponse>> writer, IProgress prog)
        {
            return value.Write(writer, null, prog);
        }

        public static Task<HttpWebResponse> Write(this string value, Func<Func<BodyInfo>, Action<Stream>, Task<HttpWebResponse>> writer)
        {
            return value.Write(writer, null, null);
        }

        /// <summary>
        /// Writes a file out to a stream.
        /// </summary>
        /// <param name="file">The text to write.</param>
        /// <param name="prog">A progress tracker. Defaults to null (no progress tracking).</param>
        /// <returns>A callback function that can be used to write the text when a stream becomes available.</returns>
        public static Task<HttpWebResponse> Write(this FileInfo file, Func<Func<BodyInfo>, Action<Stream>, Task<HttpWebResponse>> writer, string type, IProgress prog)
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

        public static Task<HttpWebResponse> Write(this FileInfo file, Func<Func<BodyInfo>, Action<Stream>, Task<HttpWebResponse>> writer, string type)
        {
            return file.Write(writer, type, null);
        }

        public static Task<HttpWebResponse> Write(this FileInfo file, Func<Func<BodyInfo>, Action<Stream>, Task<HttpWebResponse>> writer, IProgress prog)
        {
            return file.Write(writer, null, prog);
        }

        public static Task<HttpWebResponse> Write(this FileInfo file, Func<Func<BodyInfo>, Action<Stream>, Task<HttpWebResponse>> writer)
        {
            return file.Write(writer, null, null);
        }
    }
}