using System;
using System.IO;
using System.Text;

using Juniper.Progress;

namespace Juniper.HTTP
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
        public static Func<Stream, string> WriteBytes(this byte[] value, string type = null, IProgress prog = null)
        {
            return (stream) =>
            {
                using (var progStream = new ProgressStream(stream, value.Length, prog))
                {
                    progStream.Write(value, 0, value.Length);
                }

                return type ?? "application/octet-stream";
            };
        }

        /// <summary>
        /// Writes text out to a stream.
        /// </summary>
        /// <param name="value">The text to write.</param>
        /// <param name="prog">A progress tracker. Defaults to null (no progress tracking).</param>
        /// <returns>A callback function that can be used to write the text when a stream becomes available.</returns>
        public static Func<Stream, string> WriteString(this string value, string type = null, IProgress prog = null)
        {
            return (stream) =>
            {
                long len = Encoding.Unicode.GetByteCount(value);
                using (var progStream = new ProgressStream(stream, len, prog))
                using (var writer = new StreamWriter(progStream))
                {
                    writer.Write(value);
                }

                return type ?? "text/plain";
            };
        }
    }
}
