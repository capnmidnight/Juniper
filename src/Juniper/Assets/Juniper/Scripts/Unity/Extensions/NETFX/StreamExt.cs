using System.Text;
using Juniper.HTTP;
using Juniper.Progress;

using Newtonsoft.Json;

namespace System.IO
{
    /// <summary>
    /// Extension methods for reading Streams with progress tracking.
    /// </summary>
    public static class StreamExt
    {
        /// <summary>
        /// Reads all of the bytes out of a given stream.
        /// </summary>
        /// <param name="stream">The stream to read.</param>
        /// <param name="prog">A progress tracker. Defaults to null (no progress tracking).</param>
        /// <returns>The bytes read out of the stream.</returns>
        public static byte[] ReadBytes(this Stream stream, IProgress prog = null)
        {
            using (var progStream = new ProgressStream(stream, prog))
            using (var mem = new MemoryStream())
            {
                progStream.CopyTo(mem);
                return mem.GetBuffer();
            }
        }

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
        /// Reads all of the text out of a given stream.
        /// </summary>
        /// <param name="stream">The stream to read.</param>
        /// <param name="prog">A progress tracker. Defaults to null (no progress tracking).</param>
        /// <returns>The string read out of the stream.</returns>
        public static string ReadString(this Stream stream, IProgress prog = null)
        {
            using (var progStream = new ProgressStream(stream, prog))
            using (var reader = new StreamReader(progStream))
            {
                return reader.ReadToEnd();
            }
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

        /// <summary>
        /// Reads all of the text out of a stream, and interprets it as a JSON serialized object.
        /// </summary>
        /// <typeparam name="T">The type of the serialized object.</typeparam>
        /// <param name="stream">The stream to read.</param>
        /// <param name="prog">A progress tracker. Defaults to null (no progress tracking).</param>
        /// <returns>The value deserialized out of the stream.</returns>
        public static T ReadObject<T>(this Stream stream, IProgress prog = null)
        {
            using (var progStream = new ProgressStream(stream, prog))
            using (var reader = new StreamReader(progStream))
            {
                return JsonConvert.DeserializeObject<T>(reader.ReadToEnd());
            }
        }

        /// <summary>
        /// Writes an object out to a stream as JSON text.
        /// </summary>
        /// <typeparam name="T">The type of the serialized object.</typeparam>
        /// <param name="obj">The object to write.</param>
        /// <param name="prog">A progress tracker. Defaults to null (no progress tracking).</param>
        /// <returns>A callback function that can be used to write the object when a stream becomes available.</returns>
        public static Func<Stream, BodyInfo> WriteObject<T>(this T obj, IProgress prog = null)
        {
            return JsonConvert.SerializeObject(obj)
                .WriteString("application/json", prog);
        }
    }
}
