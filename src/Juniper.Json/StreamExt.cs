using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

using Juniper.HTTP;
using Juniper.Progress;

using Newtonsoft.Json;

namespace Juniper.Json
{
    /// <summary>
    /// Extension methods for reading Streams with progress tracking.
    /// </summary>
    public static class StreamExt
    {
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
        /// Writes text out to a stream.
        /// </summary>
        /// <param name="value">The text to write.</param>
        /// <param name="prog">A progress tracker. Defaults to null (no progress tracking).</param>
        /// <returns>A callback function that can be used to write the text when a stream becomes available.</returns>
        public static Task<HttpWebResponse> Write<T>(this T obj, Func<Func<BodyInfo>, Action<Stream>, Task<HttpWebResponse>> writer)
        {
            var text = JsonConvert.SerializeObject(obj);
            return text.Write(writer);
        }
    }
}
