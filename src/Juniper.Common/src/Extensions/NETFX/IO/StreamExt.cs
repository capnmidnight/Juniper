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
        /// Default read buffer size.
        /// </summary>
        private const int BLOCK_SIZE = 4096;

        /// <summary>
        /// Pipe the output of one stream into the input of another.
        /// </summary>
        /// <param name="inStream">The stream to pipe out of.</param>
        /// <param name="outStream">The stream to pipe into.</param>
        /// <param name="prog">A progress tracker. Defaults to null (no progress tracking).</param>
        public static void Pipe(this Stream inStream, Stream outStream, IProgress prog = null)
        {
            prog?.Report(0);
            inStream = new ProgressStream(inStream, prog);
            var read = int.MaxValue;
            var buf = new byte[BLOCK_SIZE];
            while (read > 0)
            {
                read = inStream.Read(buf, 0, BLOCK_SIZE);
                if (read > 0)
                {
                    outStream.Write(buf, 0, read);
                }
            }
            outStream.Flush();
            prog?.Report(1);
        }

        /// <summary>
        /// Reads all of the bytes out of a given stream.
        /// </summary>
        /// <param name="stream">The stream to read.</param>
        /// <param name="prog">A progress tracker. Defaults to null (no progress tracking).</param>
        /// <returns>The bytes read out of the stream.</returns>
        public static byte[] ReadBytes(this Stream stream, IProgress prog = null)
        {
            prog?.Report(0);
            var streamProg = new ProgressStream(stream, prog);
            var buf = new byte[stream.Length];
            for (var i = 0; i < buf.Length; i += BLOCK_SIZE)
            {
                streamProg.Read(buf, i, BLOCK_SIZE);
                prog?.Report(i, stream.Length);
            }
            prog?.Report(1);
            return buf;
        }

        /// <summary>
        /// Reads all of the text out of a given stream.
        /// </summary>
        /// <param name="stream">The stream to read.</param>
        /// <param name="prog">A progress tracker. Defaults to null (no progress tracking).</param>
        /// <returns>The string read out of the stream.</returns>
        public static string ReadString(this Stream stream, IProgress prog = null)
        {
            var streamProg = new ProgressStream(stream, prog);
            using (var reader = new StreamReader(streamProg))
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
            using (var reader = new StreamReader(new ProgressStream(stream, prog)))
            {
                return JsonConvert.DeserializeObject<T>(reader.ReadToEnd());
            }
        }
    }
}
