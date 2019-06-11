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
        public static Func<Stream, BodyInfo> WriteBytes(this byte[] value, string type = null, IProgress prog = null)
        {
            return (stream) =>
            {
                using (var progStream = new ProgressStream(stream, value.Length, prog))
                {
                    progStream.Write(value, 0, value.Length);
                }

                return new BodyInfo(type ?? "application/octet-stream", value.Length);
            };
        }

        /// <summary>
        /// Writes text out to a stream.
        /// </summary>
        /// <param name="value">The text to write.</param>
        /// <param name="prog">A progress tracker. Defaults to null (no progress tracking).</param>
        /// <returns>A callback function that can be used to write the text when a stream becomes available.</returns>
        public static Func<Stream, BodyInfo> WriteString(this string value, string type = null, IProgress prog = null)
        {
            return (stream) =>
            {
                long len = Encoding.Unicode.GetByteCount(value);
                using (var progStream = new ProgressStream(stream, len, prog))
                using (var writer = new StreamWriter(progStream))
                {
                    writer.Write(value);
                }

                return new BodyInfo(type ?? "text/plain", len);
            };
        }
    }
}
