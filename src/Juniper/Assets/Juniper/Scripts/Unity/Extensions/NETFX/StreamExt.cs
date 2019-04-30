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
    }
}
